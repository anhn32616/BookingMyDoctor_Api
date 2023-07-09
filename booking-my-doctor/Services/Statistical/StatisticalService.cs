using booking_my_doctor.DTOs;
using booking_my_doctor.Repositories;

namespace booking_my_doctor.Services.Statistical
{
    public class StatisticalService : IStatisticalService
    {
        public readonly IStatisticalRepository _statisticalRepository;
        public StatisticalService(IStatisticalRepository statisticalRepository)
        {
            _statisticalRepository = statisticalRepository;
        }

        public async Task<ApiResponse> GetQuantityStatistics()
        {
            try
            {
                var res = await _statisticalRepository.GetQuantityStatistics();
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = res
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ApiResponse> GetStatistical(DateTime startTime, DateTime endTime, int? page = null, int? pageSize = null, int? doctorId = null)
        {
            try
            {
                var res = await _statisticalRepository.GetStatistical(startTime, endTime, page, pageSize, doctorId);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = res
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }

        }

        public async Task<ApiResponse> GetStatisticsOfDoctor(int id, DateTime startTime, DateTime endTime)
        {
            try
            {
                var res = await _statisticalRepository.GetStatisticsOfDoctor(id, startTime, endTime);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công",
                    data = res
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }
        }
    }
}
