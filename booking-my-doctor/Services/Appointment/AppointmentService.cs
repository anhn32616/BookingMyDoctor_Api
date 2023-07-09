using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Appointment;
using booking_my_doctor.DTOs.Notification;
using booking_my_doctor.DTOs.Rate;
using booking_my_doctor.Repositories;
using booking_my_doctor.Repositories.Appoiment;
using System.Numerics;

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
        private readonly IRateRepository _rateRepository;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserRepository userRepository,
            IScheduleRepository scheduleRepository,
            IDoctorRepository doctorRepository,
            IRateRepository rateRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _emailService = emailService;
            _userRepository = userRepository;
            _scheduleRepository = scheduleRepository;
            _doctorRepository = doctorRepository;
            _rateRepository = rateRepository;
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
                if (!violator.Equals("doctor") && !violator.Equals("patient"))
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Người vi phạm phải là bác sĩ hoặc bệnh nhân"
                    };
                }
                var notifications = new List<NotificationDto>();
                string messageForPatient = "";
                string messageForDoctor = "";
                // Nếu bệnh nhân không đến khám thì tăng số lần vi phạm của bệnh nhân lên và chuyển trạng thái 
                // của appointment sang NotCome
                if (violator.Equals("patient"))
                {
                    appointment.Patient.countViolation++;
                    if (appointment.Patient.countViolation > 1)
                    {
                        appointment.Patient.isDelete = true;
                    }
                    appointment.Status = "NotCome";
                    string messageDetail = appointment.Patient.countViolation > 1 ? "Tài khoản của bạn đã bị khóa vì vượt qua số lần vi phạm" : "Thêm 1 lần vi phạm nữa tải khoản của bạn sẽ bị khóa";
                    messageForPatient = $"Admin đã xác nhận bạn không đến khám ngày {appointment.date.ToString("%d/MM/yyyy HH'h'mm")}. Bạn đã vi phạm {appointment.Patient.countViolation}. " + messageDetail;
                    var notificationPatient = new NotificationDto
                    {
                        UserId = appointment.PatientId,
                        Message = messageForPatient,
                        DateCreated = DateTime.Now,
                        Read = false
                    };
                    messageForDoctor = $"Admin đã xác nhận bệnh nhân {appointment.Patient.fullName} không đến khám ngày {appointment.date.ToString("%d/MM/yyyy HH'h'mm")}";
                    var notificationDoctor = new NotificationDto
                    {
                        UserId = appointment.Schedule.Doctor.user.Id,
                        Message = messageForDoctor,
                        DateCreated = DateTime.Now,
                        Read = false
                    };
                    notifications.Add(notificationPatient);
                    notifications.Add(notificationDoctor);
                }
                // Nếu bác sĩ không đến khám thì tăng số lần vi phạm của bác sĩ lên và chuyển trạng thái 
                // của appointment sang Done
                if (violator.Equals("doctor"))
                {
                    appointment.Schedule.Doctor.user.countViolation++;
                    if (appointment.Schedule.Doctor.user.countViolation > 1)
                    {
                        appointment.Schedule.Doctor.user.isDelete = true;
                    }
                    appointment.Status = "Done";
                    string messageDetail = appointment.Schedule.Doctor.user.countViolation > 1 ? "Tài khoản của bạn đã bị khóa vì vượt qua số lần vi phạm" : "Thêm 1 lần vi phạm nữa tải khoản của bạn sẽ bị khóa";
                    messageForPatient = $"Admin đã xác nhận bạn đã đến khám ngày {appointment.date.ToString("%d/MM/yyyy HH'h'mm")}";
                    var notificationPatient = new NotificationDto
                    {
                        UserId = appointment.PatientId,
                        Message = messageForPatient,
                        DateCreated = DateTime.Now,
                        Read = false
                    };
                    messageForDoctor = $"Admin đã xác nhận bệnh nhân {appointment.Patient.fullName} đã đến khám ngày {appointment.date.ToString("%d/MM/yyyy HH'h'mm")}. Bạn đã vi phạm {appointment.Patient.countViolation}. " + messageDetail;
                    var notificationDoctor = new NotificationDto
                    {
                        UserId = appointment.Schedule.Doctor.user.Id,
                        Message = messageForDoctor,
                        DateCreated = DateTime.Now,
                        Read = false
                    };
                    notifications.Add(notificationPatient);
                    notifications.Add(notificationDoctor);
                }
                await _appointmentRepository.UpdateAppointment(appointment);
                await _appointmentRepository.IsSaveChange();
                _emailService.SendEmail(appointment.Patient.email, "Thông báo về kết quả xử lý vi phạm không đến khám bệnh", messageForPatient);
                _emailService.SendEmail(appointment.Schedule.Doctor.user.email, "Thông báo về kết quả xử lý vi phạm không đến khám bệnh", messageForDoctor);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = notifications
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
                if (role == "ROLE_PATIENT" && appointment.PatientId != userId)
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
                    message = "Chỉ có thể hủy các cuộc hẹn đang chờ hoặc đã xác nhận nhưng chưa diễn ra trước 24 giờ"
                };
                // Nếu status là pending thì cho cancel luôn
                if (appointment.Status == "Pending")
                {
                    appointment.Status = "Cancel";
                }
                // Nếu status là confirm thì chỉ cho hủy trước 1 ngày khám
                else if (appointment.Status == "Confirm")
                {
                    if (appointment.date < DateTime.Now.AddDays(1)) return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Chỉ có thể hủy cuộc hẹn trước 24 giờ khám bệnh"
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
                if (patient.Result == null || patient.Result.role.Name != "ROLE_PATIENT")
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
                if (doctor.Result.monthPaid.Value.AddDays(30) < DateTime.Now)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Tài khoản cần gia hạn đăng ký tháng"
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
                appointmentsCancel.ListItem = appointmentsCancel.ListItem.Where(a => a.Id != appointmentId).ToList();
                var listNotification = new List<NotificationDto>();
                foreach (var item in appointmentsCancel.ListItem)
                {
                    item.Status = "Cancel";
                    await _appointmentRepository.UpdateAppointment(item);
                    var notification = new NotificationDto
                    {
                        UserId = item.PatientId,
                        Message = $"Bác sĩ {item.Schedule.Doctor.user.fullName} đã từ chối lịch khám ngày {item.date.ToString("%d/MM/yyyy HH'h'mm")}",
                        DateCreated = DateTime.Now,
                        Read = false
                    };
                    listNotification.Add(notification);
                }
                // Chuyển trạng thái appointment sang Confirm
                appointment.Result.Status = "Confirm";
                await _appointmentRepository.UpdateAppointment(appointment.Result);
                await _appointmentRepository.IsSaveChange();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = listNotification.Count != 0 ? listNotification : null
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

        public async Task<ApiResponse> PatientRateAppointment(int id, int patientId, RateDto rateDto)
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

                // Tạo rate mới nếu bệnh nhân đánh giá lần đâu
                // Update rate nếu bệnh nhân đánh giá lại
                var doctor = appointment.Result.Schedule.Doctor;
                var rateCurrent = await _rateRepository.GetRateByAppointmentId(appointment.Result.Id);
                if (rateCurrent == null)
                // create
                {
                    var rate = _mapper.Map<RateDto, Rate>(rateDto);
                    await _rateRepository.CreateRate(rate);
                    await _rateRepository.IsSaveChange();
                    if(doctor.numberOfReviews == null)
                    {
                        doctor.rate = rate.Point;
                        doctor.numberOfReviews = 1;
                    } else
                    {
                        doctor.rate = (doctor.rate * doctor.numberOfReviews + rate.Point) / (doctor.numberOfReviews + 1);
                        doctor.numberOfReviews++;
                    }
                } else
                // update
                {
                    doctor.rate = (doctor.rate * doctor.numberOfReviews - rateCurrent.Point + rateDto.Point) / doctor.numberOfReviews;
                    rateCurrent.Point = rateDto.Point;
                    rateCurrent.Comment= rateDto.Comment;
                    await _rateRepository.UpdateRate(rateCurrent);
                    await _rateRepository.IsSaveChange();
                }
                await _doctorRepository.UpdateDoctor(doctor);
                await _doctorRepository.IsSaveChanges();
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
