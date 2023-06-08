using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Repositories;
using booking_my_doctor.Services;
using Microsoft.AspNetCore.Mvc;
using MyWebApiApp.Models;
using System.Linq;

namespace booking_my_doctor.Services
{
    public class SpeciatlyService : ISpeciatlyService
    {
        private readonly ISpeciatlyRepository _speciatlyRepository;
        private readonly IMapper _mapper;
        public SpeciatlyService(ISpeciatlyRepository speciatlyRepository,
            IMapper mapper)
        {
            _speciatlyRepository = speciatlyRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetSpeciatlys(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id")
        {
            try
            {
                var result = await _speciatlyRepository.GetSpeciatlies(page, pageSize, keyword, sortColumn);
                var resultDto = _mapper.Map<PaginationDTO<Speciatly>, PaginationDTO<SpeciatlyDto>> (result);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success",
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
        public async Task<ApiResponse> GetSpeciatlyById(int id)
        {
            try
            {
                var result = await _speciatlyRepository.GetSpeciatlyById(id);
                if (result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại speciatly có id này"
                    };
                }
                var resultDto = _mapper.Map<Speciatly, SpeciatlyDto>(result);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success",
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

        public async Task<ApiResponse> CreateSpeciatly(SpeciatlyDto speciatlyDto)
        {
            try
            {
                var speciatly = _mapper.Map<SpeciatlyDto, Speciatly>(speciatlyDto);
                var result = await _speciatlyRepository.CreateSpeciatly(speciatly);
                await _speciatlyRepository.IsSaveChanges();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success"
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

        public async Task<ApiResponse> UpdateSpeciatly(int id, SpeciatlyDto speciatlyDto)
        {
            try
            {
                var speciatlyCurrent = await _speciatlyRepository.GetSpeciatlyById(id);
                if (speciatlyCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy speciatly có id này"
                };
                speciatlyCurrent.name = speciatlyDto.name;
                speciatlyCurrent.imageUrl = speciatlyDto.imageUrl;
                await _speciatlyRepository.UpdateSpeciatly(speciatlyCurrent);
                await _speciatlyRepository.IsSaveChanges();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success"
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

        public async Task<ApiResponse> DeleteSpeciatly(int id)
        {
            try
            {
                var speciatlyCurrent = await _speciatlyRepository.GetSpeciatlyById(id);
                if (speciatlyCurrent == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tìm thấy speciatly có id này"
                };

                await _speciatlyRepository.DeleteSpeciatly(speciatlyCurrent);
                await _speciatlyRepository.IsSaveChanges();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success"
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
