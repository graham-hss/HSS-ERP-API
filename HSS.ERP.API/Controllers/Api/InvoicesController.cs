using Microsoft.AspNetCore.Mvc;
using HSS.ERP.API.Services;
using HSS.ERP.API.Models;
using HSS.ERP.API.Data;

namespace HSS.ERP.API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;
        private readonly InvoiceDbContext _context;
        private readonly IAppInsightsService? _appInsights;

        public InvoicesController(
            IInvoiceService invoiceService, 
            ILogger<InvoicesController> logger, 
            InvoiceDbContext context,
            IAppInsightsService? appInsights = null)
        {
            _invoiceService = invoiceService;
            _logger = logger;
            _context = context;
            _appInsights = appInsights;
        }

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetInvoiceCount()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var count = await _invoiceService.GetInvoiceCountAsync();
                stopwatch.Stop();
                
                _appInsights?.TrackDatabaseQuery("GetInvoiceCount", stopwatch.Elapsed, true);
                _appInsights?.TrackUserAction("ViewInvoiceCount", GetUserId());
                
                return Ok(count);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error getting invoice count");
                _appInsights?.TrackException(ex, new Dictionary<string, string>
                {
                    ["operation"] = "GetInvoiceCount",
                    ["userId"] = GetUserId() ?? "anonymous"
                });
                return StatusCode(500, "Internal server error occurred while getting invoice count");
            }
        }

        [HttpGet("overdue/count")]
        public async Task<ActionResult<int>> GetOverdueInvoiceCount()
        {
            try
            {
                var count = await _invoiceService.GetOverdueInvoiceCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue invoice count");
                return StatusCode(500, "Internal server error occurred while getting overdue invoice count");
            }
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetInvoiceStatistics()
        {
            try
            {
                var stats = await _invoiceService.GetInvoiceStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting invoice statistics");
                return StatusCode(500, "Internal server error occurred while getting invoice statistics");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetAllInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all invoices");
                return StatusCode(500, "Internal server error occurred while getting invoices");
            }
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetOverdueInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetOverdueInvoicesAsync();
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue invoices");
                return StatusCode(500, "Internal server error occurred while getting overdue invoices");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchInvoices(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? dateRange = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var searchResults = await _invoiceService.SearchInvoicesAsync(search, status, dateRange, page, pageSize);
                
                // Enrich results with customer names
                var enrichedResults = new List<object>();
                foreach (var invoice in searchResults)
                {
                    // Get customer name
                    var customer = await _context.Customers.FindAsync(invoice.CustomerCode);
                    var customerName = customer?.CustomerName ?? invoice.CustomerCode;
                    
                    enrichedResults.Add(new
                    {
                        invoice.InvoiceId,
                        invoice.ContractCode,
                        invoice.InvoiceSeqNo,
                        invoice.CustomerCode,
                        CustomerName = customerName,
                        invoice.InvoiceCreateDate,
                        invoice.InvoiceEndDate,
                        invoice.InvoiceValue,
                        invoice.InvoiceVat,
                        invoice.InvoiceStatusCode,
                        invoice.ProcessedDate,
                        invoice.InvoiceOrder
                    });
                }
                
                _logger.LogInformation($"Search completed. Found {enrichedResults.Count} results for search='{search}', status='{status}', dateRange='{dateRange}'");
                
                return Ok(enrichedResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching invoices");
                return StatusCode(500, new { message = "Error searching invoices", error = ex.Message });
            }
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<ActionResult<Invoice>> GetInvoiceById(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                {
                    return NotFound($"Invoice with ID {invoiceId} not found");
                }
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with ID {InvoiceId}", invoiceId);
                return StatusCode(500, "Internal server error occurred while retrieving the invoice");
            }
        }

        [HttpGet("{contractCode}/{invoiceSeqNo}")]
        public async Task<ActionResult<Invoice>> GetInvoiceByCompositeKey(string contractCode, short invoiceSeqNo)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByCompositeKeyAsync(contractCode, invoiceSeqNo);
                if (invoice == null)
                {
                    return NotFound($"Invoice with contract code {contractCode} and sequence {invoiceSeqNo} not found");
                }
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with contract code {ContractCode} and sequence {InvoiceSeqNo}", 
                    contractCode, invoiceSeqNo);
                return StatusCode(500, "Internal server error occurred while retrieving the invoice");
            }
        }

        [HttpGet("{invoiceId:int}/lines")]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLinesByInvoiceId(int invoiceId)
        {
            try
            {
                var lines = await _invoiceService.GetInvoiceLinesAsync(invoiceId);
                return Ok(lines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice lines for invoice ID {InvoiceId}", invoiceId);
                return StatusCode(500, "Internal server error occurred while retrieving the invoice lines");
            }
        }

        [HttpGet("{contractCode}/{invoiceSeqNo}/lines")]
        public async Task<ActionResult<IEnumerable<InvoiceLine>>> GetInvoiceLines(string contractCode, short invoiceSeqNo)
        {
            try
            {
                // First get the invoice to find its InvoiceId
                var invoice = await _invoiceService.GetInvoiceByCompositeKeyAsync(contractCode, invoiceSeqNo);
                if (invoice == null)
                {
                    return NotFound($"Invoice with contract code {contractCode} and sequence {invoiceSeqNo} not found");
                }
                
                var lines = await _invoiceService.GetInvoiceLinesAsync(invoice.InvoiceId);
                return Ok(lines);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice lines for contract code {ContractCode} and sequence {InvoiceSeqNo}", 
                    contractCode, invoiceSeqNo);
                return StatusCode(500, "Internal server error occurred while retrieving the invoice lines");
            }
        }

        [HttpPatch("{invoiceId:int}/refund")]
        public async Task<IActionResult> UpdateRefundAmountById(int invoiceId, [FromBody] RefundUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get user info from request headers (in a real app, from auth token)
                var userId = Request.Headers["X-User-Id"].FirstOrDefault() ?? "system@company.com";

                var updated = await _invoiceService.UpdateRefundAmountAsync(invoiceId, request.RefundAmount, userId);
                if (!updated)
                {
                    return NotFound($"Invoice with ID {invoiceId} not found");
                }

                return Ok(new { message = "Refund amount updated successfully", refundAmount = request.RefundAmount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating refund amount for invoice ID {InvoiceId}", invoiceId);
                return StatusCode(500, "Internal server error occurred while updating the refund amount");
            }
        }

        [HttpPatch("{contractCode}/{invoiceSeqNo}/refund")]
        public async Task<IActionResult> UpdateRefundAmount(string contractCode, short invoiceSeqNo, [FromBody] RefundUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get user info from request headers (in a real app, from auth token)
                var userId = Request.Headers["X-User-Id"].FirstOrDefault() ?? "system@company.com";

                var updated = await _invoiceService.UpdateRefundAmountAsync(contractCode, invoiceSeqNo, request.RefundAmount, userId);
                if (!updated)
                {
                    return NotFound($"Invoice with contract code {contractCode} and sequence {invoiceSeqNo} not found");
                }

                return Ok(new { message = "Refund amount updated successfully", refundAmount = request.RefundAmount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating refund amount for invoice with contract code {ContractCode} and sequence {InvoiceSeqNo}", 
                    contractCode, invoiceSeqNo);
                return StatusCode(500, "Internal server error occurred while updating the refund amount");
            }
        }

        private string? GetUserId()
        {
            return Request.Headers["X-User-Id"].FirstOrDefault() 
                ?? Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].FirstOrDefault()
                ?? User?.Identity?.Name;
        }
    }

    public class RefundUpdateRequest
    {
        public decimal RefundAmount { get; set; }
        public string? Reason { get; set; }
    }
}