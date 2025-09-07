using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public interface IBookingService
    {
        // Booking retrieval methods
        Task<IEnumerable<Booking>> GetBookingsAsync(
            int page = 1,
            int pageSize = 20,
            string? search = null,
            string? customerCode = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<Booking?> GetBookingByNumberAsync(int bookingNo);
        Task<IEnumerable<Booking>> GetBookingsByCustomerCodeAsync(string customerCode, int page = 1, int pageSize = 20);
        Task<int> GetBookingCountAsync();
        Task<int> GetBookingCountByCustomerCodeAsync(string customerCode);

        // Booking statistics
        Task<decimal> GetTotalBookingAmountAsync();
        Task<decimal> GetTotalCapturedAmountAsync();
        Task<decimal> GetTotalRefundedAmountAsync();
        Task<Dictionary<string, int>> GetBookingStatusStatisticsAsync();
        Task<IEnumerable<object>> GetRecentBookingsAsync(int count = 10);

        // CRUD operations (if needed)
        Task<Booking?> CreateBookingAsync(Booking booking);
        Task<Booking?> UpdateBookingAsync(Booking booking);
        Task<bool> DeleteBookingAsync(int bookingNo);
    }
}
