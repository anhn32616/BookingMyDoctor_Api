using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Clinic;
using booking_my_doctor.DTOs.Hospital;
using booking_my_doctor.Repositories;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Mvc;

using System.Linq;

namespace booking_my_clinic.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IMapper _mapper;
        public ClinicService(IClinicRepository clinicRepository,
            IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetClinics(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id")
        {
            try
            {
                var result = await _clinicRepository.GetClinics(page, pageSize, keyword, sortColumn);
                var resultDto = _mapper.Map<PaginationDTO<Clinic>, PaginationDTO<ClinicDto>> (result);
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
        public async Task<ApiResponse> GetClinicById(int id)
        {
            try
            {
                var result = await _clinicRepository.GetClinicById(id);
                if (result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại clinic có id này"
                    };
                }
                var resultDto = new ClinicDetail();
                resultDto.Id = result.Id;
                resultDto.address = result.address;
                resultDto.name = result.name;
                resultDto.ward = result.ward;
                resultDto.district = result.district;
                resultDto.imageUrl = result.imageUrl;
                resultDto.doctor = _mapper.Map<Doctor, DoctorDto>(result.doctor);
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

        public async Task<ApiResponse> CreateClinic(ClinicDto clinicDto)
        {
            try
            {
                var clinic = _mapper.Map<ClinicDto, Clinic>(clinicDto);
                var result = await _clinicRepository.CreateClinic(clinic);
                await _clinicRepository.IsSaveChanges();
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

        public async Task<ApiResponse> UpdateClinic(int id, ClinicDto clinicDto)
        {
            try
            {
                var clinicCurrent = await _clinicRepository.GetClinicById(id);
                if (clinicCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy clinic có id này"
                };
                clinicCurrent.address = clinicDto.address;
                clinicCurrent.name = clinicDto.name;
                clinicCurrent.imageUrl = clinicDto.imageUrl;
                clinicCurrent.city= clinicDto.city;
                await _clinicRepository.UpdateClinic(clinicCurrent);
                await _clinicRepository.IsSaveChanges();
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

        public async Task<ApiResponse> DeleteClinic(int id)
        {
            try
            {
                var clinicCurrent = await _clinicRepository.GetClinicById(id);
                if (clinicCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy clinic có id này"
                };

                await _clinicRepository.DeleteClinic(clinicCurrent);
                await _clinicRepository.IsSaveChanges();
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
