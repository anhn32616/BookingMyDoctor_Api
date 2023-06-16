using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Repositories;


namespace booking_my_doctor.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _ScheduleRepository;
        private readonly IMapper _mapper;
        private readonly IDoctorRepository _doctorRepository;
        public ScheduleService(IScheduleRepository ScheduleRepository,
            IMapper mapper,
            IDoctorRepository doctorRepository)
        {
            _ScheduleRepository = ScheduleRepository;
            _mapper = mapper;
            _doctorRepository = doctorRepository;
        }

        public async Task<ApiResponse> GetSchedules(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null, string? status = null, DateTime? date = null, string? sortColumn = "StartTime")
        {
            try
            {
                var result = await _ScheduleRepository.GetSchedules(page, pageSize, doctorId, status, date, sortColumn);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = result
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
        public async Task<ApiResponse> GetScheduleById(int id)
        {
            try
            {
                var result = await _ScheduleRepository.GetScheduleById(id);
                if (result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại lịch khám có id này"
                    };
                }
                var resultDto = _mapper.Map<Schedule, ScheduleView>(result);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = resultDto
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

        public async Task<ApiResponse> CreateSchedule(ScheduleCreateDto scheduleCreateDto)
        {
            try
            {
                if (scheduleCreateDto.StartTime > scheduleCreateDto.EndTime) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Thời gian bắt đầu phải bé hơn thời gian kết thúc"
                };
                TimeSpan averageDuration = (scheduleCreateDto.EndTime - scheduleCreateDto.StartTime) / scheduleCreateDto.Count;
                if (averageDuration.TotalMinutes < 20) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Thời gian mỗi ca khám phải lớn hơn hoặc bằng 20 phút"
                };

                var doctor = await _doctorRepository.GetDoctorById(scheduleCreateDto.DoctorId.Value);
                if(doctor== null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tìm thấy bác sĩ có Id này"
                    };
                }
                var Schedule = _mapper.Map<ScheduleCreateDto, Schedule>(scheduleCreateDto);
                var listSchedule = _ScheduleRepository.GetSchedules(0, int.MaxValue, scheduleCreateDto.DoctorId).Result.ListItem;
                var IsInvalid = IsScheduleConflicting(Schedule, listSchedule);
                if(IsInvalid)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Thời gian bị trùng với lịch khám khác"
                    };
                }
                // Tạo danh sách các ca làm việc
                List<Schedule> shifts = new List<Schedule>();

                DateTime shiftStartTime = scheduleCreateDto.StartTime;

                // Tạo các ca làm việc và thêm vào danh sách
                for (int i = 1; i <= scheduleCreateDto.Count; i++)
                {
                    DateTime shiftEndTime = shiftStartTime.Add(averageDuration);
                    await _ScheduleRepository.CreateSchedule(new Schedule 
                        { 
                            StartTime = shiftStartTime.AddMilliseconds(1), // Thêm 1 ms để tránh trùng lịch
                            EndTime = shiftEndTime, 
                            Status = "Available", 
                            Cost = scheduleCreateDto.Cost,
                            DoctorId = scheduleCreateDto.DoctorId.Value,
                        });

                    shiftStartTime = shiftEndTime;
                }
                await _ScheduleRepository.IsSaveChanges();
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

        public async Task<ApiResponse> UpdateSchedule(int id, ScheduleDto ScheduleDto)
        {
            try
            {
                var doctor = _doctorRepository.GetDoctorById(ScheduleDto.DoctorId);
                var ScheduleCurrent = _ScheduleRepository.GetScheduleById(id);
                await Task.WhenAll(doctor, ScheduleCurrent);
                if (doctor.Result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tìm thấy bác sĩ có Id này"
                    };
                }
                if (ScheduleCurrent.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy lịch khám có id này"
                };
                if (ScheduleCurrent.Result.Status != "Available") return new ApiResponse
                {
                    statusCode = 400,
                    message = "Không thể thay đổi thông tin lịch khám đã được đặt"
                };
                var Schedule = _mapper.Map<ScheduleDto, Schedule>(ScheduleDto);
                var listSchedule = _ScheduleRepository.GetSchedules(0, int.MaxValue, ScheduleDto.DoctorId).Result.ListItem;
                listSchedule = listSchedule.Where(s => s.Id != ScheduleDto.Id).ToList();
                var IsInvalid = IsScheduleConflicting(Schedule, listSchedule);
                if (IsInvalid)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Thời gian bị trùng với lịch khám khác"
                    };
                }

                ScheduleCurrent.Result.StartTime = ScheduleDto.StartTime;
                ScheduleCurrent.Result.EndTime = ScheduleDto.EndTime;
                ScheduleCurrent.Result.Cost= ScheduleDto.Cost;
                ScheduleCurrent.Result.DoctorId= ScheduleDto.DoctorId;
                await _ScheduleRepository.UpdateSchedule(ScheduleCurrent.Result);
                await _ScheduleRepository.IsSaveChanges();
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

        public async Task<ApiResponse> DeleteSchedule(int id)
        {
            try
            {
                var ScheduleCurrent = await _ScheduleRepository.GetScheduleById(id);
                if (ScheduleCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy Schedule có id này"
                };
                if (ScheduleCurrent.Status == "Booked" || ScheduleCurrent.Status == "Pending") return new ApiResponse
                {
                    statusCode = 400,
                    message = "Không thể xóa lịch khám đã được đặt"
                };

                await _ScheduleRepository.DeleteSchedule(ScheduleCurrent);
                await _ScheduleRepository.IsSaveChanges();
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
        public bool IsScheduleConflicting(Schedule newSchedule, List<ScheduleView> scheduleList)
        {
            if (scheduleList.Count == 0) return false;
            foreach (var schedule in scheduleList)
            {
                if (newSchedule.StartTime < schedule.EndTime && newSchedule.EndTime > schedule.StartTime)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<ApiResponse> ChangeStatusScheduleToTrue(int id)
        {
            var ScheduleCurrent = await _ScheduleRepository.GetScheduleById(id);
            if (ScheduleCurrent == null) return new ApiResponse
            {
                statusCode = 404,
                message = "Không tìm thấy lịch khám có id này"
            };
            ScheduleCurrent.Status = "Pending";
            await _ScheduleRepository.UpdateSchedule(ScheduleCurrent);
            await _ScheduleRepository.IsSaveChanges();
            return new ApiResponse
            {
                statusCode = 200,
                message = "Thành công"
            };
        }
    }
}
