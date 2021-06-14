using System.Threading.Tasks;
using EvenTransit.Service.Dto.Log;

namespace EvenTransit.Service.Abstractions
{
    public interface ILogService
    {
        Task<LogSearchResultDto> SearchAsync(LogSearchRequestDto request);
        Task<LogItemDto> GetByIdAsync(string id);
        Task<LogStatisticsDto> GetDashboardStatistics();
    }
}