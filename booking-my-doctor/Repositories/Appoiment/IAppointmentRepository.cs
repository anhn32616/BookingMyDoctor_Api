using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace booking_my_doctor.Repositories.Appoiment
{
    public interface IAppointmentRepository
    {
        Task<PaginationDTO<Appointment>> GetAppointments(int? page = null, int? pageSize = null, int? scheduleId = null, DateTime? date = null, string? status = null, int? patientId = null, int? doctorId = null, string? sortBy = "Date", bool? hiddenCancel = false);
        Task<bool> CreateAppointmentAsync(Appointment appointment);
        Task<bool> IsSaveChange();
        Task<Appointment> GetAppointmentById(int id);
        Task<bool> UpdateAppointment(Appointment appointment);
        Task<bool> DeleteAppointment(Appointment appointment);
        Task<bool> UpdateAppointmentCancel();
        Task<bool> UpdateAppointmentDone();


    }
}
