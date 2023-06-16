using booking_my_doctor.Repositories.Appoiment;

namespace appointment_my_doctor.Services.Appointment
{
    public class AppointmentBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AppointmentBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateStatusAppointments();
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }
        private async Task UpdateStatusAppointments()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var appointmentRepository = scope.ServiceProvider.GetRequiredService<IAppointmentRepository>();
                await appointmentRepository.UpdateAppointmentCancel();
                await appointmentRepository.UpdateAppointmentDone();
                await appointmentRepository.IsSaveChange();
            }
        }
    }
}
