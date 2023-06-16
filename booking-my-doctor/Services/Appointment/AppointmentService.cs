using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Appointment;
using booking_my_doctor.Repositories;
using booking_my_doctor.Repositories.Appoiment;

namespace booking_my_doctor.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IDoctorRepository _doctorRepository;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserRepository userRepository,
            IScheduleRepository scheduleRepository,
            IDoctorRepository doctorRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
            _scheduleRepository = scheduleRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<ApiResponse> AdminHandleReport(int id, string violator)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentById(id);
                if (appointment == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại cuộc hẹn này"
                    };
                }
                if (appointment.Status != "Report")
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Trạng thái của cuộc hẹn không phải là bị báo cáo"
                    };
                }
                if(!violator.Equals("doctor") && !violator.Equals("patient"))
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Người vi phạm phải là bác sĩ hoặc bệnh nhân"
                    };
                }
                // Nếu bệnh nhân không đến khám thì tăng số lần vi phạm của bệnh nhân lên và chuyển trạng thái 
                // của appointment sang NotCome
                if(violator.Equals("patient"))
                {
                    appointment.Patient.countViolation++;
                    if(appointment.Patient.countViolation > 1)
                    {
                        appointment.Patient.isDelete = true;
                    }
                    appointment.Status = "NotCome";
                }
                // Nếu bác sĩ không đến khám thì tăng số lần vi phạm của bác sĩ lên và chuyển trạng thái 
                // của appointment sang Done
                if (violator.Equals("doctor"))
                {
                    appointment.Schedule.Doctor.user.countViolation++;
                    if(appointment.Schedule.Doctor.user.countViolation > 1)
                    {
                        appointment.Schedule.Doctor.user.isDelete = true;
                    }
                    appointment.Status = "Done";
                }
                await _appointmentRepository.UpdateAppointment(appointment);
                await _appointmentRepository.IsSaveChange();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> CancelAppointment(int appointmentId, int userId, string role)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentById(appointmentId);
                if (appointment == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại cuộc hẹn này"
                    };
                }
                if(role == "ROLE_PATIENT" && appointment.PatientId != userId)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Bạn không thể hủy cuộc hẹn của bệnh nhân khác"
                    };
                }
                if (role == "ROLE_DOCTOR" && appointment.Schedule.Doctor.user.Id != userId)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Bạn không thể hủy cuộc hẹn của bác sĩ khác"
                    };
                }
                if (appointment.Status != "Pending" && appointment.Status != "Confirm") return new ApiResponse
                {
                    statusCode = 400,
                    message = "Chỉ có thể hủy các cuộc hẹn đang chờ hoặc đã xác nhận nhưng chưa diễn ra trước 1 ngày"
                };
                // Nếu status là pending thì cho cancel luôn
                if (appointment.Status == "Pending")
                {
                    appointment.Status = "Cancel";
                } 
                // Nếu status là confirm thì chỉ cho hủy trước 1 ngày khám
                else if(appointment.Status == "Confirm")
                {
                    if (appointment.date < DateTime.Now.AddDays(1)) return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Chỉ có thể hủy cuộc hẹn trước 1 ngày khám"
                    };
                    appointment.Status = "Cancel";
                    //appointment.Schedule.Status = "Available";
                }
                await _appointmentRepository.UpdateAppointment(appointment);
                await _appointmentRepository.IsSaveChange();
                // Cập nhật status cho schedule của appointment bị hủy
                var schedule = await _scheduleRepository.GetScheduleById(appointment.ScheduleId);
                await _scheduleRepository.UpdateStatusSchedule(schedule);
                await _scheduleRepository.IsSaveChanges();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> CreateAppointment(AppointmentCreate appointmentCreate)
        {
            try
            {
                var patient = _userRepository.GetUserById(appointmentCreate.PatientId.Value);
                var schedule = _scheduleRepository.GetScheduleById(appointmentCreate.ScheduleId);
                var appointments = _appointmentRepository.GetAppointments(null, null, appointmentCreate.ScheduleId, null, null, appointmentCreate.PatientId);
                await Task.WhenAll(patient, schedule, appointments);
                if(patient.Result == null || patient.Result.role.Name != "ROLE_PATIENT")
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tìm thấy bệnh nhân có id này"
                    };
                }
                if (patient.Result.isDelete == true)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Tài khoản của bệnh nhân đã bị khóa"
                    };
                }
                if (schedule.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Lịch khám không tồn tại"
                };
                var count = appointments.Result.ListItem.Where(a => a.Status != "Cancel").Count();
                if (count > 0)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Bạn đã đặt lịch khám này"
                    };
                }
                if (schedule.Result.Status.Equals("Booked")) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Lịch khám đã được đặt"
                };
                // Tạo appointment
                var appointment = _mapper.Map<AppointmentCreate, Appointment>(appointmentCreate);
                appointment.date = schedule.Result.StartTime;
                appointment.Status = "Pending";
                schedule.Result.Status = "Pending";
                await _scheduleRepository.UpdateSchedule(schedule.Result);
                await _scheduleRepository.IsSaveChanges();
                await _appointmentRepository.CreateAppointmentAsync(appointment);
                await _appointmentRepository.IsSaveChange();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }

        }

        public async Task<ApiResponse> DeleteAppointment(int id, string role, int userId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentById(id);
                if (appointment == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại cuộc hẹn này"
                    };
                }
                if (role == "ROLE_PATIENT" && appointment.PatientId != userId) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Bạn không thể xóa lịch khám của bệnh nhân khác"
                };
                if (appointment.Status != "Cancel")
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Chỉ có thể xóa các cuộc hẹn bị hủy"
                    };
                }
                await _appointmentRepository.DeleteAppointment(appointment);
                await _appointmentRepository.IsSaveChange();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> DoctorAcceptAppointment(int doctorId, int appointmentId)
        {
            try
            {
                var appointment = _appointmentRepository.GetAppointmentById(appointmentId);
                var doctor = _doctorRepository.GetDoctorById(doctorId);
                await Task.WhenAll(appointment, doctor);
                if (appointment.Result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại cuộc hẹn này"
                    };
                }
                if (appointment.Result.Status != "Pending")
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Chỉ có thể chấp nhận các cuộc hẹn trạng thái đang chờ"
                    };
                }
                if (doctor.Result == null || doctor.Result.user.isDelete == true)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Tài khoản của bạn đã bị khóa"
                    };
                }
                if (doctor.Result.Id != appointment.Result.Schedule.DoctorId)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Bạn không có quyền chấp nhận lịch hẹn của bác sĩ khác"
                    };
                }
                // Chuyển trạng thái của schedule sang booked (đã đặt)
                var schedule = appointment.Result.Schedule;
                schedule.Status = "Booked";
                await _scheduleRepository.UpdateSchedule(schedule);
                await _scheduleRepository.IsSaveChanges();
                // Chuyển trạng thái các appointment còn lại đã đặt schedule này sang Cancel
                var appointmentsCancel = await _appointmentRepository.GetAppointments(null, null, appointment.Result.ScheduleId);
                foreach (var item in appointmentsCancel.ListItem)
                {
                    item.Status = "Cancel";
                    await _appointmentRepository.UpdateAppointment(item);
                }
                // Chuyển trạng thái appointment sang Confirm
                appointment.Result.Status = "Confirm";
                await _appointmentRepository.UpdateAppointment(appointment.Result);
                await _appointmentRepository.IsSaveChange();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> DoctorReportAppointment(int doctorId, int appointmentId)
        {
            try
            {
                var appointment = _appointmentRepository.GetAppointmentById(appointmentId);
                var doctor = _doctorRepository.GetDoctorById(doctorId);
                await Task.WhenAll(appointment, doctor);
                if (appointment.Result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại cuộc hẹn này"
                    };
                }
                if (appointment.Result.Status != "Done")
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Chỉ có thể báo cáo các cuộc hẹn đã hoàn thành"
                    };
                }
                if (doctor.Result == null || doctor.Result.user.isDelete == true)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Tài khoản của bạn đã bị khóa"
                    };
                }
                if (doctor.Result.Id != appointment.Result.Schedule.DoctorId)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Bạn không có quyền báo cáo lịch hẹn của bác sĩ khác"
                    };
                }

                // Chuyển trạng thái appointment sang Report
                appointment.Result.Status = "Report";
                await _appointmentRepository.UpdateAppointment(appointment.Result);
                await _appointmentRepository.IsSaveChange();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> GetAppointmentById(int id)
        {

            try
            {
                var res = await _appointmentRepository.GetAppointmentById(id);
                var resDto = _mapper.Map<Appointment, AppointmentView>(res);
                if (res == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = $"Không tồn tại cuộc hẹn có id = {id}"
                };
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = resDto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> GetAppointments(int? page = null, int? pageSize = null, int? scheduleId = null, DateTime? date = null, string? status = null, int? patientId = null, int? doctorId = null, string? sortBy = "Date", bool? hiddenCancel = false)
        {
            try
            {
                var result = await _appointmentRepository.GetAppointments(page, pageSize, scheduleId, date, status, patientId, doctorId, sortBy, hiddenCancel);
                var resultView = _mapper.Map<PaginationDTO<Appointment>, PaginationDTO<AppointmentView>>(result);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = resultView
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
            
        }

        public async Task<ApiResponse> PatientRateAppointment(int id, int patientId, int rate)
        {
            try
            {
                var appointment = _appointmentRepository.GetAppointmentById(id);
                var patient = _userRepository.GetUserById(patientId);
                await Task.WhenAll(appointment, patient);
                if (appointment.Result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại cuộc hẹn này"
                    };
                }
                if (appointment.Result.Status != "Done")
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Chỉ có thể đánh giá cuộc hẹn đã hoàn thành"
                    };
                }
                if (patient.Result == null || patient.Result.isDelete == true)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Tài khoản của bạn đã bị khóa"
                    };
                }
                if (patient.Result.Id != appointment.Result.PatientId)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Bạn không có quyền đánh giá cuộc hẹn của bệnh nhân khác"
                    };
                }

                
                appointment.Result.Rating = rate;
                await _appointmentRepository.UpdateAppointment(appointment.Result);
                await _appointmentRepository.IsSaveChange();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }
    }
}
