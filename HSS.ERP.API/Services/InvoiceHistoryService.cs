using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Data;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public class InvoiceHistoryService : IInvoiceHistoryService
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<InvoiceHistoryService> _logger;

        public InvoiceHistoryService(InvoiceDbContext context, ILogger<InvoiceHistoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<InvoiceHistory>> GetInvoiceHistoryAsync(int invoiceId)
        {
            try
            {
                return await _context.InvoiceHistories
                    .Where(h => h.InvoiceId == invoiceId)
                    .OrderByDescending(h => h.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history for invoice {InvoiceId}", invoiceId);
                return new List<InvoiceHistory>();
            }
        }

        public async Task<InvoiceHistory?> GetHistoryByIdAsync(int historyId)
        {
            try
            {
                return await _context.InvoiceHistories
                    .Include(h => h.Invoice)
                    .FirstOrDefaultAsync(h => h.HistoryId == historyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving history {HistoryId}", historyId);
                return null;
            }
        }

        public async Task<InvoiceHistory> AddHistoryAsync(int invoiceId, string title, string content, string createdBy, string historyType = "note")
        {
            try
            {
                var history = new InvoiceHistory
                {
                    InvoiceId = invoiceId,
                    HistoryTitle = title,
                    HistoryContent = content,
                    CreatedBy = createdBy,
                    HistoryType = historyType,
                    CreatedAt = DateTime.UtcNow
                };

                _context.InvoiceHistories.Add(history);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added history entry {HistoryId} for invoice {InvoiceId} by {CreatedBy}", 
                    history.HistoryId, invoiceId, createdBy);

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding history for invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<bool> DeleteHistoryAsync(int historyId)
        {
            try
            {
                var history = await _context.InvoiceHistories.FindAsync(historyId);
                if (history == null)
                {
                    _logger.LogWarning("History entry {HistoryId} not found for deletion", historyId);
                    return false;
                }

                _context.InvoiceHistories.Remove(history);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted history entry {HistoryId}", historyId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting history {HistoryId}", historyId);
                return false;
            }
        }

        public async Task<bool> UpdateHistoryAsync(int historyId, string title, string content, string modifiedBy)
        {
            try
            {
                var history = await _context.InvoiceHistories.FindAsync(historyId);
                if (history == null)
                {
                    _logger.LogWarning("History entry {HistoryId} not found for update", historyId);
                    return false;
                }

                history.HistoryTitle = title;
                history.HistoryContent = content;
                // Note: We're not tracking who modified it in this simple implementation
                // You could add a ModifiedBy and ModifiedAt field if needed

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated history entry {HistoryId} by {ModifiedBy}", historyId, modifiedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating history {HistoryId}", historyId);
                return false;
            }
        }
    }
}
