using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Hospital;
using booking_my_doctor.Repositories;

using System.Linq;

namespace booking_my_doctor.Services
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IMapper _mapper;
        public HospitalService(IHospitalRepository hospitalRepository,
            IMapper mapper)
        {
            _hospitalRepository = hospitalRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetHospitals(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id")
        {
            try
            {
                var result = await _hospitalRepository.GetHospitals(page, pageSize, keyword, sortColumn);
                var resultDto = _mapper.Map<PaginationDTO<Hospital>, PaginationDTO<HospitalDto>> (result);
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
        public async Task<ApiResponse> GetHospitalById(int id)
        {
            try
            {
                var result = await _hospitalRepository.GetHospitalById(id);
                if (result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại hospital có id này"
                    };
                }
                var resultDto = new HospitalDetail();
                resultDto.Id = result.Id;
                resultDto.address = result.address;
                resultDto.name = result.name;
                resultDto.ward= result.ward;
                resultDto.district = result.district;
                resultDto.city = result.city;
                resultDto.imageUrl = result.imageUrl;
                resultDto.doctors = result.doctors.Select(_mapper.Map<Doctor, DoctorDto>).ToList();
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

        public async Task<ApiResponse> CreateHospital(HospitalDto hospitalDto)
        {
            try
            {
                var hospital = _mapper.Map<HospitalDto, Hospital>(hospitalDto);
                var result = await _hospitalRepository.CreateHospital(hospital);
                await _hospitalRepository.IsSaveChanges();
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

        public async Task<ApiResponse> UpdateHospital(int id, HospitalDto hospitalDto)
        {
            try
            {
                var hospitalCurrent = await _hospitalRepository.GetHospitalById(id);
                if (hospitalCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy hospital có id này"
                };
                hospitalCurrent.address = hospitalDto.address;
                hospitalCurrent.name = hospitalDto.name;
                hospitalCurrent.imageUrl = hospitalDto.imageUrl;
                hospitalCurrent.city= hospitalDto.city;
                hospitalCurrent.district= hospitalDto.district;
                hospitalCurrent.ward = hospitalDto.ward;
                await _hospitalRepository.UpdateHospital(hospitalCurrent);
                await _hospitalRepository.IsSaveChanges();
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

        public async Task<ApiResponse> DeleteHospital(int id)
        {
            try
            {
                var hospitalCurrent = await _hospitalRepository.GetHospitalById(id);
                if (hospitalCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy hospital có id này"
                };

                await _hospitalRepository.DeleteHospital(hospitalCurrent);
                await _hospitalRepository.IsSaveChanges();
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
