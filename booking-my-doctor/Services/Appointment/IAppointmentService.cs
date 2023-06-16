using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Appointment;


namespace booking_my_doctor.Services
{
    public interface IAppointmentService
    {
        Task<ApiResponse> GetAppointments(int? page = null, int? pageSize = null, int? scheduleId = null, DateTime? date = null, string? status = null, int? patientId = null, int? doctorId = null, string? sortBy = "Date", bool? hiddenCancel = false);
        Task<ApiResponse> CreateAppointment(AppointmentCreate appointmentCreate);
        Task<ApiResponse> GetAppointmentById(int id);
        Task<ApiResponse> DoctorAcceptAppointment(int doctorId, int appointmentId);
        Task<ApiResponse> DeleteAppointment(int id, string role, int userId);
        Task<ApiResponse> DoctorReportAppointment(int doctorId, int appointmentId);
        Task<ApiResponse> AdminHandleReport(int id, string violator);
        Task<ApiResponse> CancelAppointment(int appointmentId, int userId, string role);
        Task<ApiResponse> PatientRateAppointment(int id, int patientId, int rate);


    }
}
