using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Repositories;


namespace booking_my_doctor.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ISpeciatlyRepository _speciatlyRepository;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public DoctorService(IDoctorRepository doctorRepository,
            IHospitalRepository hospitalRepository,
            IClinicRepository clinicRepository,
            ISpeciatlyRepository speciatlyRepository,
            IUserService userService,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _clinicRepository = clinicRepository;
            _hospitalRepository = hospitalRepository;
            _speciatlyRepository = speciatlyRepository;
            _userService = userService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateDoctor(DoctorCreateDto doctorCreateDto)
        {
            try
            {
                var IsHospitalIdValid = _hospitalRepository.GetHospitalById(doctorCreateDto.hospitalId);
                var IsClinicIdValid = _clinicRepository.GetClinicById(doctorCreateDto.clinicId);
                var IsSpeciatlyIdValid = _speciatlyRepository.GetSpeciatlyById(doctorCreateDto.specialtyId);
                await Task.WhenAll(IsHospitalIdValid, IsClinicIdValid, IsSpeciatlyIdValid);
                if (IsHospitalIdValid.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy bệnh viện có id này"
                };
                if (IsClinicIdValid.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy phòng khám có id này"
                };
                if (IsClinicIdValid.Result.doctor != null) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Phòng khám này của bác sĩ khác"
                };
                if (IsSpeciatlyIdValid.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy chuyên khoa có id này"
                };
                doctorCreateDto.user.roleName = "ROLE_DOCTOR";
                if (doctorCreateDto.user.image == null) doctorCreateDto.user.image = "https://e7.pngegg.com/pngimages/14/65/png-clipart-ico-avatar-scalable-graphics-icon-doctor-with-stethoscope-people-cartoon-thumbnail.png";
                var resultCreateUser = await _userService.CreateUser(doctorCreateDto.user);
                if (resultCreateUser.statusCode != 200) return resultCreateUser;
                var userId = await _userRepository.GetLastUserID();
                var doctor = new Doctor();
                doctor.rate = null;
                doctor.numberOfReviews = null;
                doctor.description = doctorCreateDto.description;
                doctor.monthPaid = null;
                doctor.hospitalId = doctorCreateDto.hospitalId;
                doctor.clinicId= doctorCreateDto.clinicId;
                doctor.specialtyId= doctorCreateDto.specialtyId;
                doctor.userId = userId;
                var resultData = await _doctorRepository.CreateDoctor(doctor);
                await _doctorRepository.IsSaveChanges();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
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

        public async Task<ApiResponse> DeleteDoctor(int id)
        {
            try
            {
                var doctor = await _doctorRepository.GetDoctorById(id);
                if (doctor == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy bác sĩ có id này"
                };
                await _userRepository.DeleteUser(doctor.user);
                await _userRepository.IsSaveChanges();
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

        public async Task<ApiResponse> GetDoctorById(int id)
        {
            try
            {
                var result = await _doctorRepository.GetDoctorById(id);
                if (result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy bác sĩ có id này"
                };
                var resultDto = _mapper.Map<Doctor, DoctorDto>(result);
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

        public async Task<ApiResponse> GetDoctors(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id")
        {
            try
            {
                var result = await _doctorRepository.GetDoctors(page, pageSize, keyword, sortColumn);
                var resultDto = _mapper.Map<PaginationDTO<Doctor>, PaginationDTO<DoctorDto>>(result);
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

        public async Task<ApiResponse> UpdateDoctor(int id, DoctorUpdateDto doctorUpdateDto)
        {
            try
            {
                var doctor = await _doctorRepository.GetDoctorById(id);
                if (doctor == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy bác sĩ có id này"
                };
                var IsHospitalIdValid = _hospitalRepository.GetHospitalById(doctorUpdateDto.hospitalId);
                var IsClinicIdValid = _clinicRepository.GetClinicById(doctorUpdateDto.clinicId);
                var IsSpeciatlyIdValid = _speciatlyRepository.GetSpeciatlyById(doctorUpdateDto.specialtyId);
                await Task.WhenAll(IsHospitalIdValid, IsClinicIdValid, IsSpeciatlyIdValid);
                if (IsHospitalIdValid.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy bệnh viện có id này"
                };
                if (IsClinicIdValid.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy phòng khám có id này"
                };
                if (IsClinicIdValid.Result.doctor != null && IsClinicIdValid.Result.doctor.Id != doctor.Id) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Phòng khám này của bác sĩ khác"
                };
                if (IsSpeciatlyIdValid.Result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy chuyên khoa có id này"
                };
                var resultUpdateUser = await _userService.UpdateUser(doctor.user.Id, doctorUpdateDto.user);
                if (resultUpdateUser.statusCode != 200) return resultUpdateUser;

                doctor.description = doctorUpdateDto.description;
                doctor.hospitalId = doctorUpdateDto.hospitalId;
                doctor.clinicId = doctorUpdateDto.clinicId;
                doctor.specialtyId = doctorUpdateDto.specialtyId;
                var resultData = await _doctorRepository.UpdateDoctor(doctor);
                await _doctorRepository.IsSaveChanges();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
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
