using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace booking_my_doctor.Services
{
    public interface IClinicService
    {
        Task<ApiResponse> GetClinics(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<ApiResponse> GetClinicById(int id);
        Task<ApiResponse> UpdateClinic(int id, ClinicDto clinicDto);
        Task<ApiResponse> CreateClinic(ClinicDto clinicDto);
        Task<ApiResponse> DeleteClinic(int id);
    }
}
