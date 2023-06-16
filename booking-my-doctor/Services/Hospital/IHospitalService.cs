using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace booking_my_doctor.Services
{
    public interface IHospitalService
    {
        Task<ApiResponse> GetHospitals(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<ApiResponse> GetHospitalById(int id);
        Task<ApiResponse> UpdateHospital(int id, HospitalDto hospitalDto);
        Task<ApiResponse> CreateHospital(HospitalDto hospitalDto);
        Task<ApiResponse> DeleteHospital(int id);
    }
}
