using booking_my_doctor.Data.Entities;

namespace booking_my_doctor.Repositories
{
    public interface IRateRepository
    {
        Task<List<Rate>> GetRates(int? doctorId = null); 
        Task<bool> CreateRate(Rate rate);
        Task<bool> UpdateRate(Rate rate);
        Task<bool> IsSaveChange();
        Task<Rate> GetRateByAppointmentId (int appointmentId);
    }
}
