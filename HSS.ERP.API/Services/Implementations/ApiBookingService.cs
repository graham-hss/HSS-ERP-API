using HSS.ERP.API.Models;
using System.Text.Json;

namespace HSS.ERP.API.Services.Implementations
{
    public class ApiBookingService : IBookingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiBookingService> _logger;
        private readonly string _baseApiUrl;

        public ApiBookingService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiBookingService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _baseApiUrl = _configuration.GetValue<string>("ExternalApi:BookingsBaseUrl") ?? throw new ArgumentNullException("ExternalApi:BookingsBaseUrl configuration is required");

            // Configure HttpClient
            _httpClient.BaseAddress = new Uri(_baseApiUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Add any required authentication headers
            var apiKey = _configuration.GetValue<string>("ExternalApi:ApiKey");
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(
            int page = 1,
            int pageSize = 20,
            string? search = null,
            string? customerCode = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(search))
                    queryParams.Add($"search={Uri.EscapeDataString(search)}");

                if (!string.IsNullOrEmpty(customerCode))
                    queryParams.Add($"customerCode={Uri.EscapeDataString(customerCode)}");

                if (!string.IsNullOrEmpty(status))
                    queryParams.Add($"status={Uri.EscapeDataString(status)}");

                if (fromDate.HasValue)
                    queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");

                if (toDate.HasValue)
                    queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");

                var queryString = string.Join("&", queryParams);
                var response = await _httpClient.GetAsync($"/api/bookings?{queryString}");
                
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiBookingsResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return apiResponse?.Bookings ?? new List<Booking>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error retrieving bookings from external API");
                throw new ServiceException("Failed to retrieve bookings from external API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error when retrieving bookings");
                throw new ServiceException("Failed to parse bookings response from external API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving bookings from external API");
                throw;
            }
        }

        public async Task<Booking?> GetBookingByNumberAsync(int bookingNo)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/bookings/{bookingNo}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var booking = JsonSerializer.Deserialize<Booking>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return booking;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error retrieving booking {BookingNo} from external API", bookingNo);
                throw new ServiceException($"Failed to retrieve booking {bookingNo} from external API", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error when retrieving booking {BookingNo}", bookingNo);
                throw new ServiceException($"Failed to parse booking {bookingNo} response from external API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error retrieving booking {BookingNo} from external API", bookingNo);
                throw;
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingsByCustomerCodeAsync(string customerCode, int page = 1, int pageSize = 20)
        {
            return await GetBookingsAsync(page, pageSize, customerCode: customerCode);
        }

        public async Task<int> GetBookingCountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/bookings/count");
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var countResponse = JsonSerializer.Deserialize<CountResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return countResponse?.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking count from external API");
                throw new ServiceException("Failed to get booking count from external API", ex);
            }
        }

        public async Task<int> GetBookingCountByCustomerCodeAsync(string customerCode)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/bookings/count?customerCode={Uri.EscapeDataString(customerCode)}");
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var countResponse = JsonSerializer.Deserialize<CountResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return countResponse?.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking count for customer {CustomerCode} from external API", customerCode);
                throw new ServiceException($"Failed to get booking count for customer {customerCode} from external API", ex);
            }
        }

        public async Task<decimal> GetTotalBookingAmountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/bookings/statistics/total-amount");
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var amountResponse = JsonSerializer.Deserialize<AmountResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return amountResponse?.Amount ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total booking amount from external API");
                throw new ServiceException("Failed to get total booking amount from external API", ex);
            }
        }

        public async Task<decimal> GetTotalCapturedAmountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/bookings/statistics/captured-amount");
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var amountResponse = JsonSerializer.Deserialize<AmountResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return amountResponse?.Amount ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total captured amount from external API");
                throw new ServiceException("Failed to get total captured amount from external API", ex);
            }
        }

        public async Task<decimal> GetTotalRefundedAmountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/bookings/statistics/refunded-amount");
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var amountResponse = JsonSerializer.Deserialize<AmountResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return amountResponse?.Amount ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total refunded amount from external API");
                throw new ServiceException("Failed to get total refunded amount from external API", ex);
            }
        }

        public async Task<Dictionary<string, int>> GetBookingStatusStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/bookings/statistics/status");
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var statisticsResponse = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return statisticsResponse ?? new Dictionary<string, int>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking status statistics from external API");
                throw new ServiceException("Failed to get booking status statistics from external API", ex);
            }
        }

        public async Task<IEnumerable<object>> GetRecentBookingsAsync(int count = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/bookings/recent?count={count}");
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var recentBookings = JsonSerializer.Deserialize<IEnumerable<object>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return recentBookings ?? new List<object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent bookings from external API");
                throw new ServiceException("Failed to get recent bookings from external API", ex);
            }
        }

        public async Task<Booking?> CreateBookingAsync(Booking booking)
        {
            try
            {
                var json = JsonSerializer.Serialize(booking);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/bookings", content);
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var createdBooking = JsonSerializer.Deserialize<Booking>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return createdBooking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking via external API");
                throw new ServiceException("Failed to create booking via external API", ex);
            }
        }

        public async Task<Booking?> UpdateBookingAsync(Booking booking)
        {
            try
            {
                var json = JsonSerializer.Serialize(booking);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"/api/bookings/{booking.BookingNo}", content);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                
                response.EnsureSuccessStatusCode();
                
                var jsonContent = await response.Content.ReadAsStringAsync();
                var updatedBooking = JsonSerializer.Deserialize<Booking>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return updatedBooking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingNo} via external API", booking.BookingNo);
                throw new ServiceException($"Failed to update booking {booking.BookingNo} via external API", ex);
            }
        }

        public async Task<bool> DeleteBookingAsync(int bookingNo)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/bookings/{bookingNo}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;
                
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {BookingNo} via external API", bookingNo);
                throw new ServiceException($"Failed to delete booking {bookingNo} via external API", ex);
            }
        }
    }

    // Helper classes for API responses
    public class ApiBookingsResponse
    {
        public IEnumerable<Booking> Bookings { get; set; } = new List<Booking>();
        public PaginationInfo? Pagination { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class CountResponse
    {
        public int Count { get; set; }
    }

    public class AmountResponse
    {
        public decimal Amount { get; set; }
    }

    public class ServiceException : Exception
    {
        public ServiceException(string message) : base(message) { }
        public ServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
