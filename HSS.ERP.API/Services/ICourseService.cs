using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetCoursesAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, string? type = null, bool? isPublished = null);
        Task<Course?> GetCourseByNumberAsync(int courseNo);
        Task<Course?> GetCourseByCodeAsync(string courseCode);
        Task<int> GetCourseCountAsync();
        Task<int> GetCourseCountAsync(string? search = null, string? status = null, string? type = null, bool? isPublished = null);
        Task<IEnumerable<Course>> GetRecentCoursesAsync(int limit = 10);
        Task<Dictionary<string, int>> GetCourseStatusStatisticsAsync();
        Task<Dictionary<string, int>> GetCourseTypeStatisticsAsync();
        Task<IEnumerable<Course>> SearchCoursesAsync(string query, int limit = 10);
    }
}
