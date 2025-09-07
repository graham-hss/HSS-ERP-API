using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Models;
using HSS.ERP.API.Services;
using HSS.ERP.API.Data;

namespace HSS.ERP.API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IStockService _stockService;
        private readonly ILogger<BookingsController> _logger;
        private readonly InvoiceDbContext _context;

        public BookingsController(IBookingService bookingService, IStockService stockService, ILogger<BookingsController> logger, InvoiceDbContext context)
        {
            _bookingService = bookingService;
            _stockService = stockService;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetBookings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? customerCode = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsAsync(page, pageSize, search, customerCode, status, fromDate, toDate);
                var totalCount = await _bookingService.GetBookingCountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Set pagination headers
                Response.Headers.Append("X-Total-Count", totalCount.ToString());
                Response.Headers.Append("X-Total-Pages", totalPages.ToString());
                Response.Headers.Append("X-Current-Page", page.ToString());
                Response.Headers.Append("X-Page-Size", pageSize.ToString());

                return Ok(new
                {
                    bookings = bookings.Select(b => new
                    {
                        id = b.Id,
                        bookingNumber = b.BookingNumber,
                        bookingDate = b.BookingDate,
                        expiryDate = b.ExpiryDate,
                        customerCode = b.CustomerCode,
                        customerName = b.Customer?.CustomerName ?? b.CustomerName,
                        customer = b.Customer != null ? new 
                        {
                            code = b.Customer.CustomerCode,
                            name = b.Customer.CustomerName,
                            address1 = b.Customer.Address1,
                            address2 = b.Customer.Address2,
                            city = b.Customer.Town,
                            postcode = b.Customer.Postcode,
                            telephone = b.Customer.Telephone,
                            email = b.Customer.Email
                        } : null,
                        contactName = b.ContactName,
                        email = b.Email,
                        telephone = b.Telephone,
                        subTotal = b.SubTotal,
                        vatAmount = b.VatAmount,
                        totalAmount = b.TotalAmount,
                        capturedAmount = b.CapturedAmount,
                        refundedAmount = b.RefundedAmount,
                        status = b.Status,
                        notes = b.Notes,
                        orderReference = b.OrderReference,
                        formattedDate = b.FormattedDate,
                        formattedExpiryDate = b.FormattedExpiryDate,
                        lineCount = b.LineCount
                    }).ToList(),
                    pagination = new
                    {
                        currentPage = page,
                        totalPages = totalPages,
                        totalCount = totalCount,
                        pageSize = pageSize,
                        hasNextPage = page < totalPages,
                        hasPreviousPage = page > 1
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings");
                return StatusCode(500, new { message = "Error retrieving bookings", error = ex.Message });
            }
        }

        [HttpGet("{bookingNo}")]
        public async Task<ActionResult<object>> GetBooking(int bookingNo)
        {
            try
            {
                var booking = await _bookingService.GetBookingByNumberAsync(bookingNo);

                if (booking == null)
                {
                    return NotFound(new { message = $"Booking {bookingNo} not found" });
                }

                // Debug logging
                _logger.LogInformation("Booking Customer: {Customer}", booking.Customer?.CustomerName ?? "NULL");
                _logger.LogInformation("Booking CustomerName property: {CustomerName}", booking.CustomerName);
                _logger.LogInformation("Booking CustomerCode: {CustomerCode}", booking.CustomerCode);

                return Ok(new
                {
                    id = booking.Id,
                    bookingNumber = booking.BookingNumber,
                    bookingDate = booking.BookingDate,
                    expiryDate = booking.ExpiryDate,
                    customerCode = booking.CustomerCode,
                    customerName = booking.Customer?.CustomerName ?? "Customer data not loaded",
                    customer = booking.Customer != null ? new 
                    {
                        code = booking.Customer.CustomerCode,
                        name = booking.Customer.CustomerName,
                        address1 = booking.Customer.Address1,
                        address2 = booking.Customer.Address2,
                        city = booking.Customer.Town,
                        postcode = booking.Customer.Postcode,
                        telephone = booking.Customer.Telephone,
                        email = booking.Customer.Email
                    } : null,
                    contactName = booking.ContactName,
                    email = booking.Email,
                    telephone = booking.Telephone,
                    subTotal = booking.SubTotal,
                    vatAmount = booking.VatAmount,
                    totalAmount = booking.TotalAmount,
                    capturedAmount = booking.CapturedAmount,
                    refundedAmount = booking.RefundedAmount,
                    status = booking.Status,
                    notes = booking.Notes,
                    orderReference = booking.OrderReference,
                    formattedDate = booking.FormattedDate,
                    formattedExpiryDate = booking.FormattedExpiryDate,
                    lineCount = booking.LineCount,
                    bookingLines = booking.BookingLines?.Select(bl => new
                    {
                        lineNumber = bl.LineNumber,
                        description = bl.Description,
                        quantity = bl.Quantity,
                        unitPrice = bl.UnitPrice,
                        basicPrice = bl.BasicPrice,
                        autoPrice = bl.AutoPrice,
                        lineTotal = bl.LineTotal,
                        vatRate = bl.VatRate,
                        vatAmount = bl.VatAmount,
                        discountPercent = bl.DiscountPercent,
                        requestedQty = bl.RequestedQty,
                        cancelledQty = bl.CancelledQty,
                        availableQty = bl.AvailableQty,
                        lineType = bl.LineType,
                        deliveryType = bl.DeliveryType,
                        isPriceEdited = bl.IsPriceEdited,
                        productCode = bl.ProductCode,
                        formattedLineTotal = bl.FormattedLineTotal,
                        formattedUnitPrice = bl.FormattedUnitPrice,
                        contractNo = bl.ContractNo,
                        stockNo = bl.StockNo
                    }).OrderBy(bl => bl.lineNumber).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking {BookingNo}", bookingNo);
                return StatusCode(500, new { message = $"Error retrieving booking {bookingNo}", error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetBookingStatistics()
        {
            try
            {
                var totalBookings = await _bookingService.GetBookingCountAsync();
                var totalValue = await _bookingService.GetTotalBookingAmountAsync();
                var totalCaptured = await _bookingService.GetTotalCapturedAmountAsync();
                var totalRefunded = await _bookingService.GetTotalRefundedAmountAsync();
                var statusStats = await _bookingService.GetBookingStatusStatisticsAsync();
                var recentBookings = await _bookingService.GetRecentBookingsAsync(5);

                return Ok(new
                {
                    totalBookings = totalBookings,
                    totalValue = totalValue,
                    totalCaptured = totalCaptured,
                    totalRefunded = totalRefunded,
                    statusBreakdown = statusStats.Select(sc => new
                    {
                        status = sc.Key switch
                        {
                            "D" => "Draft",
                            "C" => "Confirmed",
                            "P" => "Paid",
                            "X" => "Cancelled",
                            _ => "Unknown"
                        },
                        count = sc.Value
                    }).ToList(),
                    recentBookings = recentBookings
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking statistics");
                return StatusCode(500, new { message = "Error retrieving booking statistics", error = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchBookings([FromQuery] string query = "", [FromQuery] int limit = 10)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsAsync(1, limit, search: query);

                return Ok(bookings.Select(b => new
                {
                    id = b.Id,
                    bookingNumber = b.BookingNumber,
                    customerCode = b.CustomerCode,
                    contactName = b.ContactName,
                    totalAmount = b.TotalAmount,
                    status = b.Status,
                    createDate = b.BookingDate
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching bookings with query: {Query}", query);
                return StatusCode(500, new { message = "Error searching bookings", error = ex.Message });
            }
        }

        [HttpGet("stock-names")]
        public async Task<ActionResult<object>> GetStockNames([FromQuery] string[] stockNos)
        {
            try
            {
                if (stockNos == null || stockNos.Length == 0)
                {
                    return Ok(new Dictionary<string, string>());
                }

                // Convert string stock numbers to integers
                var stockNoInts = stockNos
                    .Where(s => int.TryParse(s, out _))
                    .Select(s => int.Parse(s))
                    .ToList();

                if (stockNoInts.Count == 0)
                {
                    return Ok(new Dictionary<string, string>());
                }

                // Use the StockService to get stock names
                var stockNames = await _stockService.GetStockNamesAsync(stockNoInts);

                // Convert to string keys for API response
                var result = stockNames.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock names for stock numbers: {StockNos}", string.Join(", ", stockNos ?? new string[0]));
                return StatusCode(500, new { message = "Error retrieving stock names", error = ex.Message });
            }
        }
    }
}