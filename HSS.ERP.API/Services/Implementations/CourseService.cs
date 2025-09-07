using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Data;
using HSS.ERP.API.Models;
using HSS.ERP.API.Services;
using System.Linq;

namespace HSS.ERP.API.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<CourseService> _logger;

        public CourseService(InvoiceDbContext context, ILogger<CourseService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null, string? type = null, bool? isPublished = null)
        {
            try
            {
                // First get the courses normally
                var query = _context.Courses.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => 
                        EF.Functions.ILike(c.CourseName, $"%{search}%") ||
                        EF.Functions.ILike(c.CourseCode, $"%{search}%") ||
                        EF.Functions.ILike(c.CourseWebCode, $"%{search}%"));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(c => c.CourseStatus == status);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(c => c.CourseTypeCode == type);
                }

                if (isPublished.HasValue)
                {
                    var publishedValue = isPublished.Value ? "Y" : "N";
                    query = query.Where(c => c.CoursePublish == publishedValue);
                }

                var courses = await query
                    .OrderByDescending(c => c.CourseNo)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Load ALL CourseTypes and CourseCategories at once (pre-load the lookup tables)
                var allCourseTypes = await _context.CourseTypes.ToListAsync();
                var allCourseCategories = await _context.CourseCategories.ToListAsync();

                // Create dictionaries for fast lookup
                var courseTypesDict = allCourseTypes.ToDictionary(ct => ct.CourseTypeCode, ct => ct);
                var courseCategoriesDict = allCourseCategories.ToDictionary(cc => cc.CourseCategoryNo, cc => cc);

                // Assign the lookup data to each course
                foreach (var course in courses)
                {
                    if (!string.IsNullOrEmpty(course.CourseTypeCode) && courseTypesDict.ContainsKey(course.CourseTypeCode))
                    {
                        course.CourseType = courseTypesDict[course.CourseTypeCode];
                    }

                    if (course.CourseCategoryNo > 0 && courseCategoriesDict.ContainsKey(course.CourseCategoryNo))
                    {
                        course.CourseCategory = courseCategoriesDict[course.CourseCategoryNo];
                    }
                }

                _logger.LogInformation("Loaded {CourseCount} courses with lookup data. CourseTypes loaded: {TypeCount}, CourseCategories loaded: {CategoryCount}", 
                    courses.Count(), allCourseTypes.Count, allCourseCategories.Count);

                return courses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses for page {Page} with pageSize {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<Course?> GetCourseByNumberAsync(int courseNo)
        {
            try
            {
                var course = await _context.Courses
                    .FirstOrDefaultAsync(c => c.CourseNo == courseNo);

                if (course != null)
                {
                    // Load CourseType if needed
                    if (!string.IsNullOrEmpty(course.CourseTypeCode))
                    {
                        course.CourseType = await _context.CourseTypes
                            .FirstOrDefaultAsync(ct => ct.CourseTypeCode == course.CourseTypeCode);
                    }

                    // Load CourseCategory if needed
                    if (course.CourseCategoryNo > 0)
                    {
                        course.CourseCategory = await _context.CourseCategories
                            .FirstOrDefaultAsync(cc => cc.CourseCategoryNo == course.CourseCategoryNo);
                    }
                }

                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course {CourseNo}", courseNo);
                throw;
            }
        }

        public async Task<Course?> GetCourseByCodeAsync(string courseCode)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.CourseCategory)
                    .Include(c => c.CourseType)
                    .FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with code {CourseCode}", courseCode);
                throw;
            }
        }

        public async Task<int> GetCourseCountAsync()
        {
            try
            {
                return await _context.Courses.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course count");
                throw;
            }
        }

        public async Task<int> GetCourseCountAsync(string? search = null, string? status = null, string? type = null, bool? isPublished = null)
        {
            try
            {
                var query = _context.Courses.AsQueryable();

                // Apply same filters as GetCoursesAsync
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c => 
                        EF.Functions.ILike(c.CourseName, $"%{search}%") ||
                        EF.Functions.ILike(c.CourseCode, $"%{search}%") ||
                        EF.Functions.ILike(c.CourseWebCode, $"%{search}%"));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(c => c.CourseStatus == status);
                }

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(c => c.CourseTypeCode == type);
                }

                if (isPublished.HasValue)
                {
                    var publishedValue = isPublished.Value ? "Y" : "N";
                    query = query.Where(c => c.CoursePublish == publishedValue);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered course count");
                throw;
            }
        }

        public async Task<IEnumerable<Course>> GetRecentCoursesAsync(int limit = 10)
        {
            try
            {
                return await _context.Courses
                    .OrderByDescending(c => c.CourseNo)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent courses");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetCourseStatusStatisticsAsync()
        {
            try
            {
                return await _context.Courses
                    .GroupBy(c => c.CourseStatus)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course status statistics");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetCourseTypeStatisticsAsync()
        {
            try
            {
                return await _context.Courses
                    .GroupBy(c => c.CourseTypeCode)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course type statistics");
                throw;
            }
        }

        public async Task<IEnumerable<Course>> SearchCoursesAsync(string query, int limit = 10)
        {
            try
            {
                return await _context.Courses
                    .Where(c => 
                        EF.Functions.ILike(c.CourseName, $"%{query}%") ||
                        EF.Functions.ILike(c.CourseCode, $"%{query}%") ||
                        EF.Functions.ILike(c.CourseWebCode, $"%{query}%"))
                    .OrderByDescending(c => c.CourseNo)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses with query: {Query}", query);
                throw;
            }
        }
    }
}
