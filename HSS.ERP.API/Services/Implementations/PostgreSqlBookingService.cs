using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Data;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services.Implementations
{
    public class PostgreSqlBookingService : IBookingService
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<PostgreSqlBookingService> _logger;

        public PostgreSqlBookingService(InvoiceDbContext context, ILogger<PostgreSqlBookingService> logger)
        {
            _context = context;
            _logger = logger;
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
                var query = _context.Bookings
                    .Include(b => b.Customer)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(b => 
                        b.BookingNo.ToString().Contains(search) ||
                        b.CustomerCode.Contains(search) ||
                        b.BookingContact.Contains(search) ||
                        b.BookingEmail.Contains(search) ||
                        b.BookingOrder.Contains(search));
                }

                // Apply customer filter
                if (!string.IsNullOrEmpty(customerCode))
                {
                    query = query.Where(b => b.CustomerCode == customerCode);
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(b => b.BookingTypeCode == status);
                }

                // Apply date range filter
                if (fromDate.HasValue)
                {
                    query = query.Where(b => b.BookingCreateDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(b => b.BookingCreateDate <= toDate.Value);
                }

                // Apply pagination
                var bookings = await query
                    .OrderByDescending(b => b.BookingCreateDate ?? DateTime.MinValue)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings with filters: search={Search}, customerCode={CustomerCode}, status={Status}, fromDate={FromDate}, toDate={ToDate}", 
                    search, customerCode, status, fromDate, toDate);
                throw;
            }
        }

        public async Task<Booking?> GetBookingByNumberAsync(int bookingNo)
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.BookingLines)
                        .ThenInclude(bl => bl.Stock)
                    .FirstOrDefaultAsync(b => b.BookingNo == bookingNo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking {BookingNo}", bookingNo);
                throw;
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingsByCustomerCodeAsync(string customerCode, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.Customer)
                    .Where(b => b.CustomerCode == customerCode)
                    .OrderByDescending(b => b.BookingCreateDate ?? DateTime.MinValue)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings for customer {CustomerCode}", customerCode);
                throw;
            }
        }

        public async Task<int> GetBookingCountAsync()
        {
            try
            {
                return await _context.Bookings.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking count");
                throw;
            }
        }

        public async Task<int> GetBookingCountByCustomerCodeAsync(string customerCode)
        {
            try
            {
                return await _context.Bookings
                    .Where(b => b.CustomerCode == customerCode)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking count for customer {CustomerCode}", customerCode);
                throw;
            }
        }

        public async Task<decimal> GetTotalBookingAmountAsync()
        {
            try
            {
                return await _context.Bookings
                    .SumAsync(b => b.BookingCharge + b.BookingVat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total booking amount");
                throw;
            }
        }

        public async Task<decimal> GetTotalCapturedAmountAsync()
        {
            try
            {
                return await _context.Bookings
                    .SumAsync(b => b.BookingCaptured);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total captured amount");
                throw;
            }
        }

        public async Task<decimal> GetTotalRefundedAmountAsync()
        {
            try
            {
                return await _context.Bookings
                    .SumAsync(b => b.BookingRefunded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total refunded amount");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetBookingStatusStatisticsAsync()
        {
            try
            {
                return await _context.Bookings
                    .GroupBy(b => b.BookingTypeCode)
                    .ToDictionaryAsync(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking status statistics");
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetRecentBookingsAsync(int count = 10)
        {
            try
            {
                return await _context.Bookings
                    .OrderByDescending(b => b.BookingCreateDate)
                    .Take(count)
                    .Select(b => new
                    {
                        id = b.BookingNo,
                        bookingNumber = b.BookingNo.ToString(),
                        customerCode = b.CustomerCode,
                        totalAmount = b.BookingCharge + b.BookingVat,
                        status = b.BookingTypeCode,
                        createDate = b.BookingCreateDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent bookings");
                throw;
            }
        }

        public async Task<Booking?> CreateBookingAsync(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                throw;
            }
        }

        public async Task<Booking?> UpdateBookingAsync(Booking booking)
        {
            try
            {
                var existingBooking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingNo == booking.BookingNo);

                if (existingBooking == null)
                    return null;

                _context.Entry(existingBooking).CurrentValues.SetValues(booking);
                await _context.SaveChangesAsync();
                return existingBooking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingNo}", booking.BookingNo);
                throw;
            }
        }

        public async Task<bool> DeleteBookingAsync(int bookingNo)
        {
            try
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingNo == bookingNo);

                if (booking == null)
                    return false;

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking {BookingNo}", bookingNo);
                throw;
            }
        }
    }
}
