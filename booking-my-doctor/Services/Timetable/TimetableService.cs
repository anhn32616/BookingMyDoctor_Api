using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Timetable;
using booking_my_doctor.Repositories;


namespace booking_my_doctor.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _TimetableRepository;
        private readonly IMapper _mapper;
        private readonly IDoctorRepository _doctorRepository;
        public TimetableService(ITimetableRepository TimetableRepository,
            IMapper mapper,
            IDoctorRepository doctorRepository)
        {
            _TimetableRepository = TimetableRepository;
            _mapper = mapper;
            _doctorRepository = doctorRepository;
        }

        public async Task<ApiResponse> GetTimetables(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null)
        {
            try
            {
                var result = await _TimetableRepository.GetTimetables(page, pageSize, doctorId);
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
        public async Task<ApiResponse> GetTimetableById(int id)
        {
            try
            {
                var result = await _TimetableRepository.GetTimetableById(id);
                if (result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại lịch khám có id này"
                    };
                }
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

        public async Task<ApiResponse> CreateTimetable(TimetableCreateDto timetableCreateDto, int doctorId)
        {
            try
            {
                var starTime = timetableCreateDto.StartTime;
                timetableCreateDto.StartTime = new DateTime(2001, 4, 20, starTime.Hour, starTime.Minute, 0);
                var endTime = timetableCreateDto.EndTime;
                timetableCreateDto.EndTime = new DateTime(2001, 4, 20, endTime.Hour, starTime.Minute, 0);
                if (timetableCreateDto.StartTime > timetableCreateDto.EndTime) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Thời gian bắt đầu phải bé hơn thời gian kết thúc"
                };
                TimeSpan averageDuration = (timetableCreateDto.EndTime - timetableCreateDto.StartTime) / timetableCreateDto.Count;
                if (averageDuration.TotalMinutes < 20) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Thời gian mỗi ca khám phải lớn hơn hoặc bằng 20 phút"
                };

                var doctor = await _doctorRepository.GetDoctorById(doctorId);
                if(doctor== null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tìm thấy bác sĩ có Id này"
                    };
                }
                var Timetable = _mapper.Map<TimetableCreateDto, Timetable>(timetableCreateDto);
                var listTimetable = _TimetableRepository.GetTimetables(0, int.MaxValue, doctorId).Result.ListItem;
                var IsInvalid = IsTimetableConflicting(Timetable, listTimetable);
                if(IsInvalid)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Thời gian bị trùng với lịch khám khác"
                    };
                }
                // Tạo danh sách các ca làm việc
                List<Timetable> shifts = new List<Timetable>();

                DateTime shiftStartTime = timetableCreateDto.StartTime;

                // Tạo các ca làm việc và thêm vào danh sách
                for (int i = 1; i <= timetableCreateDto.Count; i++)
                {
                    DateTime shiftEndTime = shiftStartTime.Add(averageDuration);
                    await _TimetableRepository.CreateTimetable(new Timetable 
                        { 
                            StartTime = shiftStartTime.AddMilliseconds(1), // Thêm 1 ms để tránh trùng lịch
                            EndTime = shiftEndTime, 
                            Cost = timetableCreateDto.Cost,
                            DoctorId = doctorId,
                        });

                    shiftStartTime = shiftEndTime;
                }
                await _TimetableRepository.IsSaveChanges();
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

        public async Task<ApiResponse> UpdateTimetable(int id, Timetable timetable)
        {
            try
            {
                var doctor = _doctorRepository.GetDoctorById(timetable.DoctorId);
                var TimetableCurrent = _TimetableRepository.GetTimetableById(id);
                await Task.WhenAll(doctor, TimetableCurrent);
                if (doctor.Result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tìm thấy bác sĩ có Id này"
                    };
                }
                if (TimetableCurrent.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy lịch khám có id này"
                };

                var listTimetable = _TimetableRepository.GetTimetables(0, int.MaxValue, timetable.DoctorId).Result.ListItem;
                listTimetable = listTimetable.Where(s => s.Id != timetable.Id).ToList();
                var IsInvalid = IsTimetableConflicting(timetable, listTimetable);
                if (IsInvalid)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Thời gian bị trùng với lịch khám khác"
                    };
                }

                TimetableCurrent.Result.StartTime = timetable.StartTime;
                TimetableCurrent.Result.EndTime = timetable.EndTime;
                TimetableCurrent.Result.Cost= timetable.Cost;
                TimetableCurrent.Result.DoctorId= timetable.DoctorId;
                await _TimetableRepository.UpdateTimetable(TimetableCurrent.Result);
                await _TimetableRepository.IsSaveChanges();
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

        public async Task<ApiResponse> DeleteTimetable(int id)
        {
            try
            {
                var TimetableCurrent = await _TimetableRepository.GetTimetableById(id);
                if (TimetableCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy Timetable có id này"
                };

                await _TimetableRepository.DeleteTimetable(TimetableCurrent);
                await _TimetableRepository.IsSaveChanges();
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
        public bool IsTimetableConflicting(Timetable newTimetable, List<Timetable> timetableList)
        {
            if (timetableList.Count == 0) return false;
            foreach (var timetable in timetableList)
            {
                if (newTimetable.StartTime < timetable.EndTime && newTimetable.EndTime > timetable.StartTime)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
