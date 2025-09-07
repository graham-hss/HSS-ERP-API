using Microsoft.AspNetCore.Mvc;
using HSS.ERP.API.Models;
using HSS.ERP.API.Services;

namespace HSS.ERP.API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetCourses(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isPublished = null)
        {
            try
            {
                var courses = await _courseService.GetCoursesAsync(page, pageSize, search, status, type, isPublished);
                var totalCount = await _courseService.GetCourseCountAsync(search, status, type, isPublished);
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Set pagination headers
                Response.Headers.Append("X-Total-Count", totalCount.ToString());
                Response.Headers.Append("X-Total-Pages", totalPages.ToString());
                Response.Headers.Append("X-Current-Page", page.ToString());
                Response.Headers.Append("X-Page-Size", pageSize.ToString());

                return Ok(new
                {
                    courses = courses.Select(c => new
                    {
                        id = c.Id,
                        courseNo = c.CourseNo,
                        courseCode = c.CourseCode,
                        courseName = c.CourseName,
                        courseWebCode = c.CourseWebCode,
                        status = c.Status,
                        statusCode = c.CourseStatus,
                        type = c.Type,
                        typeCode = c.CourseTypeCode,
                        categoryName = c.CategoryName,
                        categoryNo = c.CourseCategoryNo,
                        isPublished = c.IsPublished,
                        isInternallyHosted = c.IsInternallyHosted,
                        duration = c.CourseDuration,
                        formattedDuration = c.FormattedDuration,
                        minDelegates = c.MinDelegates,
                        maxDelegates = c.MaxDelegates,
                        delegateRange = c.DelegateRange,
                        supplierCost = c.SupplierCost,
                        formattedSupplierCost = c.FormattedSupplierCost,
                        sundryCost = c.SundryCost,
                        formattedSundryCost = c.FormattedSundryCost,
                        totalCost = c.TotalCost,
                        formattedTotalCost = c.FormattedTotalCost,
                        supplierCode = c.SupplierCode,
                        courseSupplierCode = c.CourseSupplierCode,
                        delegateCeu = c.DelegateCeu,
                        trainerCeu = c.TrainerCeu,
                        courseWebsite = c.CourseWebsite,
                        joiningUrl = c.JoiningUrl,
                        bodyRef = c.BodyRef,
                        citbRef = c.CitbRef
                    }).ToList(),
                    pagination = new
                    {
                        currentPage = page,
                        totalPages = totalPages,
                        totalCount = totalCount,
                        pageSize = pageSize,
                        hasNextPage = page < totalPages,
                        hasPreviousPage = page > 1
                    },
                    filters = new
                    {
                        search = search,
                        status = status,
                        type = type,
                        isPublished = isPublished
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving courses");
                return StatusCode(500, new { message = "Error retrieving courses", error = ex.Message });
            }
        }

        [HttpGet("{courseNo}")]
        public async Task<ActionResult<object>> GetCourse(int courseNo)
        {
            try
            {
                var course = await _courseService.GetCourseByNumberAsync(courseNo);

                if (course == null)
                {
                    return NotFound(new { message = $"Course {courseNo} not found" });
                }

                return Ok(new
                {
                    id = course.Id,
                    courseNo = course.CourseNo,
                    courseCode = course.CourseCode,
                    courseName = course.CourseName,
                    courseWebCode = course.CourseWebCode,
                    status = course.Status,
                    statusCode = course.CourseStatus,
                    type = course.Type,
                    typeCode = course.CourseTypeCode,
                    categoryName = course.CategoryName,
                    courseCategoryNo = course.CourseCategoryNo,
                    isPublished = course.IsPublished,
                    publishCode = course.CoursePublish,
                    isInternallyHosted = course.IsInternallyHosted,
                    internallyHostedCode = course.InternallyHosted,
                    duration = course.CourseDuration,
                    formattedDuration = course.FormattedDuration,
                    minDelegates = course.MinDelegates,
                    maxDelegates = course.MaxDelegates,
                    delegateRange = course.DelegateRange,
                    supplierCost = course.SupplierCost,
                    formattedSupplierCost = course.FormattedSupplierCost,
                    sundryCost = course.SundryCost,
                    formattedSundryCost = course.FormattedSundryCost,
                    totalCost = course.TotalCost,
                    formattedTotalCost = course.FormattedTotalCost,
                    supplierCode = course.SupplierCode,
                    courseSupplierCode = course.CourseSupplierCode,
                    stockNoDelegate = course.StockNoDelegate,
                    stockNoFullWithItem = course.StockNoFullWithItem,
                    stockNoFullNoItem = course.StockNoFullNoItem,
                    supplierOrderAddId = course.SupplierOrderAddId,
                    delegateCeu = course.DelegateCeu,
                    trainerCeu = course.TrainerCeu,
                    occupancyNo = course.OccupancyNo,
                    courseWebsite = course.CourseWebsite,
                    joiningUrl = course.JoiningUrl,
                    editCounter = course.EditCounter,
                    bodyNo = course.BodyNo,
                    bodyRef = course.BodyRef,
                    citbTierNo = course.CitbTierNo,
                    citbRef = course.CitbRef,
                    certTypeNo = course.CertTypeNo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course {CourseNo}", courseNo);
                return StatusCode(500, new { message = $"Error retrieving course {courseNo}", error = ex.Message });
            }
        }

        [HttpGet("by-code/{courseCode}")]
        public async Task<ActionResult<object>> GetCourseByCode(string courseCode)
        {
            try
            {
                var course = await _courseService.GetCourseByCodeAsync(courseCode);

                if (course == null)
                {
                    return NotFound(new { message = $"Course with code '{courseCode}' not found" });
                }

                // Return the same detailed format as GetCourse
                return Ok(new
                {
                    id = course.Id,
                    courseNo = course.CourseNo,
                    courseCode = course.CourseCode,
                    courseName = course.CourseName,
                    courseWebCode = course.CourseWebCode,
                    status = course.Status,
                    statusCode = course.CourseStatus,
                    type = course.Type,
                    typeCode = course.CourseTypeCode,
                    isPublished = course.IsPublished,
                    isInternallyHosted = course.IsInternallyHosted,
                    duration = course.CourseDuration,
                    formattedDuration = course.FormattedDuration,
                    delegateRange = course.DelegateRange,
                    formattedTotalCost = course.FormattedTotalCost
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course with code {CourseCode}", courseCode);
                return StatusCode(500, new { message = $"Error retrieving course with code '{courseCode}'", error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetCourseStatistics()
        {
            try
            {
                var totalCourses = await _courseService.GetCourseCountAsync();
                var statusStats = await _courseService.GetCourseStatusStatisticsAsync();
                var typeStats = await _courseService.GetCourseTypeStatisticsAsync();
                var recentCourses = await _courseService.GetRecentCoursesAsync(5);

                return Ok(new
                {
                    totalCourses = totalCourses,
                    statusBreakdown = statusStats.Select(sc => new
                    {
                        status = sc.Key switch
                        {
                            "A" => "Active",
                            "I" => "Inactive",
                            "D" => "Deleted",
                            "P" => "Pending",
                            _ => "Unknown"
                        },
                        statusCode = sc.Key,
                        count = sc.Value
                    }).ToList(),
                    typeBreakdown = typeStats.Select(tc => new
                    {
                        type = tc.Key switch
                        {
                            "C" => "Classroom",
                            "O" => "Online",
                            "B" => "Blended",
                            "W" => "Workshop",
                            _ => "Unknown"
                        },
                        typeCode = tc.Key,
                        count = tc.Value
                    }).ToList(),
                    recentCourses = recentCourses.Select(c => new
                    {
                        id = c.Id,
                        courseCode = c.CourseCode,
                        courseName = c.CourseName,
                        status = c.Status,
                        type = c.Type,
                        isPublished = c.IsPublished
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving course statistics");
                return StatusCode(500, new { message = "Error retrieving course statistics", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchCourses([FromQuery] string query = "", [FromQuery] int limit = 10)
        {
            try
            {
                var courses = await _courseService.SearchCoursesAsync(query, limit);

                return Ok(courses.Select(c => new
                {
                    id = c.Id,
                    courseNo = c.CourseNo,
                    courseCode = c.CourseCode,
                    courseName = c.CourseName,
                    status = c.Status,
                    type = c.Type,
                    isPublished = c.IsPublished,
                    formattedDuration = c.FormattedDuration,
                    formattedTotalCost = c.FormattedTotalCost
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses with query: {Query}", query);
                return StatusCode(500, new { message = "Error searching courses", error = ex.Message });
            }
        }
    }
}