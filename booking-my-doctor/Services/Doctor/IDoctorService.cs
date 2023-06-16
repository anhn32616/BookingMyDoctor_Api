using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace booking_my_doctor.Services
{
    public interface IDoctorService
    {
        Task<ApiResponse> CreateDoctor(DoctorCreateDto doctorCreateDto);
        Task<ApiResponse> GetDoctors(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<ApiResponse> GetDoctorById(int id);
        Task<ApiResponse> UpdateDoctor(int id, DoctorUpdateDto doctorUpdateDto);
        Task<ApiResponse> DeleteDoctor(int id);
    }
}
