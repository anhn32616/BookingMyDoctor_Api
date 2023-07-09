using booking_my_doctor.Repositories;
using Quartz;
using Quartz.Impl;

namespace booking_my_doctor.Services
{
    public class ScheduleJob : IJob
    {

        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleJob(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Thực hiện công việc lập lịch của bạn ở đây
                // Ví dụ:
                await _scheduleRepository.AutoAddSchedule();

                Console.WriteLine("Schedule job executed successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error executing schedule job: " + e.Message);
            }
        }
    }
}
