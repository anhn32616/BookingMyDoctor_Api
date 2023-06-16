using booking_my_doctor.Repositories;

namespace schedule_my_doctor.Services.Schedule
{
    public class ScheduleBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ScheduleBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateStatusSchedules();
                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }
        private async Task UpdateStatusSchedules()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scheduleRepository = scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
                await scheduleRepository.UpdateScheduleExpired();
                await scheduleRepository.IsSaveChanges();
            }
        }
    }
}
