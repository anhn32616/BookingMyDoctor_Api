using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace booking_my_doctor.Services
{
    public interface ISpeciatlyService
    {
        Task<ApiResponse> GetSpeciatlys(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<ApiResponse> GetSpeciatlyById(int id);
        Task<ApiResponse> UpdateSpeciatly(int id, SpeciatlyDto speciatlyDto);
        Task<ApiResponse> CreateSpeciatly(SpeciatlyDto speciatlyDto);
        Task<ApiResponse> DeleteSpeciatly(int id);
    }
}
