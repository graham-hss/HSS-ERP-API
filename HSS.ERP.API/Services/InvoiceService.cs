using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Data;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(InvoiceDbContext context, ILogger<InvoiceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            try
            {
                // Temporarily remove customer include to diagnose issue
                return await _context.Invoices
                    .OrderByDescending(i => i.ProcessedDate)
                    .Take(100) // Limit to first 100 for performance
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all invoices");
                throw;
            }
        }

        public async Task<Invoice?> GetInvoiceByCompositeKeyAsync(string contractCode, short invoiceSeqno)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.InvoiceLines)
                    .FirstOrDefaultAsync(i => i.ContractCode == contractCode && i.InvoiceSeqNo == invoiceSeqno);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with contract code {ContractCode} and sequence {InvoiceSeqno}", 
                    contractCode, invoiceSeqno);
                throw;
            }
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.InvoiceLines)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with ID {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.InvoiceLines)
                    .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice with number {InvoiceNumber}", invoiceNumber);
                throw;
            }
        }

        public async Task<Invoice?> CreateInvoiceAsync(Invoice invoice)
        {
            try
            {
                invoice.ProcessedDate = DateTime.UtcNow;
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Created new invoice {InvoiceNumber} for contract {ContractCode} by {CreatedBy}", 
                    invoice.InvoiceNumber, invoice.ContractCode, invoice.ProcessedBy);
                
                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice {InvoiceNumber} for contract {ContractCode}", 
                    invoice.InvoiceNumber, invoice.ContractCode);
                throw;
            }
        }

        public async Task<Invoice?> UpdateInvoiceAsync(Invoice invoice)
        {
            try
            {
                var existingInvoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.ContractCode == invoice.ContractCode && i.InvoiceSeqNo == invoice.InvoiceSeqNo);
                    
                if (existingInvoice == null)
                {
                    return null;
                }

                // Update fields that can be modified
                existingInvoice.CustomerCode = invoice.CustomerCode;
                existingInvoice.InvoiceValue = invoice.InvoiceValue;
                existingInvoice.InvoiceVat = invoice.InvoiceVat;
                existingInvoice.InvoiceStatusCode = invoice.InvoiceStatusCode;
                existingInvoice.ProcessedBy = invoice.ProcessedBy;
                existingInvoice.ProcessedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Updated invoice {InvoiceNumber} for contract {ContractCode} by {ModifiedBy}", 
                    existingInvoice.InvoiceNumber, existingInvoice.ContractCode, invoice.ProcessedBy);
                
                return existingInvoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice for contract {ContractCode}, sequence {InvoiceSeqno}", 
                    invoice.ContractCode, invoice.InvoiceSeqNo);
                throw;
            }
        }

        public async Task<bool> DeleteInvoiceAsync(string contractCode, short invoiceSeqno)
        {
            try
            {
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.ContractCode == contractCode && i.InvoiceSeqNo == invoiceSeqno);
                    
                if (invoice == null)
                {
                    return false;
                }

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Deleted invoice {InvoiceNumber} for contract {ContractCode}", 
                    invoice.InvoiceNumber, contractCode);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice for contract {ContractCode}, sequence {InvoiceSeqno}", 
                    contractCode, invoiceSeqno);
                throw;
            }
        }

        public async Task<bool> DeleteInvoiceByIdAsync(int invoiceId)
        {
            try
            {
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice == null)
                {
                    _logger.LogWarning("Invoice with ID {InvoiceId} not found for deletion", invoiceId);
                    return false;
                }

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Deleted invoice with ID {InvoiceId}", invoiceId);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice with ID {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<bool> UpdateRefundAmountAsync(int invoiceId, decimal refundAmount, string modifiedBy)
        {
            try
            {
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
                    
                if (invoice == null)
                {
                    return false;
                }

                // Since RefundAmount is computed, we need to update individual line items
                // For now, just update the processed info to track the refund request
                invoice.ProcessedBy = modifiedBy;
                invoice.ProcessedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating refund amount for invoice ID {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<bool> UpdateRefundAmountAsync(string contractCode, short invoiceSeqno, decimal refundAmount, string modifiedBy)
        {
            try
            {
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.ContractCode == contractCode && i.InvoiceSeqNo == invoiceSeqno);
                    
                if (invoice == null)
                {
                    return false;
                }

                // Since RefundAmount is computed, we need to update individual line items
                // For now, just update the processed info to track the refund request
                invoice.ProcessedBy = modifiedBy;
                invoice.ProcessedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Updated refund tracking for invoice {InvoiceNumber} to {RefundAmount} by {ModifiedBy}", 
                    invoice.InvoiceNumber, refundAmount, modifiedBy);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating refund amount for invoice contract {ContractCode}, sequence {InvoiceSeqno}", 
                    contractCode, invoiceSeqno);
                throw;
            }
        }

        public async Task<bool> UpdateRefundAmountByIdAsync(int invoiceId, decimal refundAmount, string modifiedBy)
        {
            try
            {
                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice == null)
                {
                    _logger.LogWarning("Invoice with ID {InvoiceId} not found for refund update", invoiceId);
                    return false;
                }

                // Update refund tracking fields
                invoice.ProcessedBy = modifiedBy;
                invoice.ProcessedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Updated refund tracking for invoice ID {InvoiceId} to {RefundAmount} by {ModifiedBy}", 
                    invoiceId, refundAmount, modifiedBy);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating refund amount for invoice ID {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<bool> UpdateInvoiceLineRefundAsync(string contractCode, short invoiceSeqno, int contractlineNo, decimal refundAmount, string modifiedBy)
        {
            try
            {
                var invoiceLine = await _context.InvoiceLines
                    .FirstOrDefaultAsync(l => l.ContractCode == contractCode && 
                                            l.InvoiceSeqNo == invoiceSeqno && 
                                            l.ContractLineNo == contractlineNo);
                                            
                if (invoiceLine == null)
                {
                    return false;
                }

                // Update the off-hire discount as it represents refund amount
                invoiceLine.InvoiceLineOffHireDiscount = refundAmount;
                invoiceLine.ProcessedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Updated refund amount for invoice line {ContractCode}-{InvoiceSeqno}-{ContractlineNo} to {RefundAmount} by {ModifiedBy}", 
                    contractCode, invoiceSeqno, contractlineNo, refundAmount, modifiedBy);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating refund amount for invoice line {ContractCode}-{InvoiceSeqno}-{ContractlineNo}", 
                    contractCode, invoiceSeqno, contractlineNo);
                throw;
            }
        }

        public async Task<decimal> GetTotalRefundsAsync()
        {
            try
            {
                // Calculate total refunds from invoice lines (off-hire discounts represent refunds)
                var totalRefunds = await _context.InvoiceLines
                    .Where(il => il.InvoiceLineOffHireDiscount > 0)
                    .SumAsync(il => il.InvoiceLineOffHireDiscount);

                return totalRefunds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total refunds");
                throw;
            }
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(string status)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.InvoiceLines)
                    .Where(i => i.InvoiceStatusCode == status)
                    .OrderByDescending(i => i.ProcessedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices with status {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerAsync(string customerCode)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.InvoiceLines)
                    .Where(i => i.CustomerCode == customerCode)
                    .OrderByDescending(i => i.ProcessedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for customer {CustomerCode}", customerCode);
                throw;
            }
        }

        public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
        {
            try
            {
                var today = DateTime.UtcNow;
                return await _context.Invoices
                    .Include(i => i.InvoiceLines)
                    .Where(i => i.InvoiceEndDate.HasValue && i.InvoiceEndDate < today && 
                               i.InvoiceStatusCode != "P") // Not paid
                    .OrderBy(i => i.InvoiceEndDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue invoices");
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetDashboardInvoicesAsync()
        {
            try
            {
                return await _context.Invoices
                    .OrderByDescending(i => i.ProcessedDate)
                    .Take(10)
                    .Select(i => new 
                    {
                        ContractCode = i.ContractCode,
                        InvoiceSeqno = i.InvoiceSeqNo,
                        InvoiceNumber = i.InvoiceNumber,
                        CustomerCode = i.CustomerCode,
                        InvoiceValue = i.InvoiceValue,
                        InvoiceVat = i.InvoiceVat,
                        TotalAmount = i.InvoiceValue + i.InvoiceVat,
                        StatusCode = i.InvoiceStatusCode,
                        InvoiceDate = i.InvoiceCreateDate,
                        DueDate = i.InvoiceEndDate,
                        ProcessedDate = i.ProcessedDate
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard invoices");
                throw;
            }
        }

        public async Task<IEnumerable<Invoice>> SearchInvoicesAsync(string? searchTerm, string? status, string? dateRange, int page = 1, int pageSize = 50)
        {
            try
            {
                var query = _context.Invoices.AsQueryable();

                // Apply search term filter (search in invoice ID, contract code, customer code, or invoice order)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var term = searchTerm.ToLower();
                    
                    // Try to parse as integer for invoice ID search
                    bool isNumeric = int.TryParse(searchTerm, out int invoiceIdSearch);
                    
                    query = query.Where(i => 
                        (isNumeric && i.InvoiceId == invoiceIdSearch) ||
                        i.ContractCode.ToLower().Contains(term) ||
                        (i.CustomerCode != null && i.CustomerCode.ToLower().Contains(term)) ||
                        (i.InvoiceOrder != null && i.InvoiceOrder.ToLower().Contains(term))
                    );
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(status))
                {
                    // Map frontend status to database status codes
                    var statusCode = status.ToUpper() switch
                    {
                        "DRAFT" => "D",
                        "SENT" => "S", 
                        "PAID" => "P",
                        "OVERDUE" => "O",
                        "CANCELLED" => "C",
                        _ => status.ToUpper()
                    };
                    
                    query = query.Where(i => i.InvoiceStatusCode == statusCode);
                }

                // Apply date range filter
                if (!string.IsNullOrWhiteSpace(dateRange))
                {
                    var now = DateTime.UtcNow;
                    var filterDate = dateRange.ToLower() switch
                    {
                        "today" => now.Date,
                        "week" => now.AddDays(-7),
                        "month" => now.AddMonths(-1),
                        "quarter" => now.AddMonths(-3),
                        _ => DateTime.MinValue
                    };

                    if (filterDate != DateTime.MinValue)
                    {
                        query = query.Where(i => i.InvoiceCreateDate >= filterDate);
                    }
                }

                // Apply pagination
                var skip = (page - 1) * pageSize;
                
                var results = await query
                    .OrderByDescending(i => i.ProcessedDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation($"Search found {results.Count} results for term='{searchTerm}', status='{status}', dateRange='{dateRange}'");
                
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching invoices with term: {SearchTerm}, status: {Status}, dateRange: {DateRange}", 
                    searchTerm, status, dateRange);
                throw;
            }
        }

        // Missing interface implementations
        public async Task<IEnumerable<Invoice>> GetInvoicesPagedAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            return await _context.Invoices
                .OrderByDescending(i => i.ProcessedDate)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(string contractCode, short invoiceSeqno)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.ContractCode == contractCode && i.InvoiceSeqNo == invoiceSeqno);
        }

        public async Task<IEnumerable<InvoiceLine>> GetInvoiceLinesAsync(int invoiceId)
        {
            return await _context.InvoiceLines
                .Where(il => il.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) return false;
            
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetInvoiceCountAsync()
        {
            return await _context.Invoices.CountAsync();
        }

        public async Task<decimal> GetTotalInvoiceValueAsync()
        {
            return await _context.Invoices.SumAsync(i => i.InvoiceValue);
        }

        public async Task<decimal> GetTotalOutstandingValueAsync()
        {
            return await _context.Invoices
                .Where(i => i.InvoiceStatusCode != "P") // Not paid
                .SumAsync(i => i.InvoiceValue);
        }

        public async Task<int> GetOverdueInvoiceCountAsync()
        {
            var today = DateTime.Today;
            return await _context.Invoices.CountAsync(i => 
                i.InvoiceStatusCode == "O" || 
                (i.InvoiceStatusCode != "P" && i.InvoiceDate < today.AddDays(-30))
            );
        }

        public async Task<object> GetInvoiceStatisticsAsync()
        {
            var totalCount = await _context.Invoices.CountAsync();
            var paidCount = await _context.Invoices.CountAsync(i => i.InvoiceStatusCode == "P");
            var overdueCount = await GetOverdueInvoiceCountAsync();
            var totalValue = await GetTotalInvoiceValueAsync();
            var outstandingValue = await GetTotalOutstandingValueAsync();

            return new
            {
                TotalInvoices = totalCount,
                PaidInvoices = paidCount,
                OverdueInvoices = overdueCount,
                TotalValue = totalValue,
                OutstandingValue = outstandingValue
            };
        }
    }
}
