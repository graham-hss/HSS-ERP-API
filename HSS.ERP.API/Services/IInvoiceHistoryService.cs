using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public interface IInvoiceHistoryService
    {
        Task<IEnumerable<InvoiceHistory>> GetInvoiceHistoryAsync(int invoiceId);
        Task<InvoiceHistory?> GetHistoryByIdAsync(int historyId);
        Task<InvoiceHistory> AddHistoryAsync(int invoiceId, string title, string content, string createdBy, string historyType = "note");
        Task<bool> DeleteHistoryAsync(int historyId);
        Task<bool> UpdateHistoryAsync(int historyId, string title, string content, string modifiedBy);
    }
}
