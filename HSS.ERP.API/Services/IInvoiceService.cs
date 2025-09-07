using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public interface IInvoiceService
    {
        // Invoice retrieval methods
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesPagedAsync(int page = 1, int pageSize = 50);
        Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
        Task<IEnumerable<Invoice>> SearchInvoicesAsync(string? searchTerm, string? status, string? dateRange, int page = 1, int pageSize = 50);

        // Single invoice methods
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<Invoice?> GetInvoiceByCompositeKeyAsync(string contractCode, short invoiceSeqno);
        Task<Invoice?> GetInvoiceByIdAsync(string contractCode, short invoiceSeqno);
        Task<Invoice?> GetInvoiceByNumberAsync(string invoiceNumber);
        Task<IEnumerable<InvoiceLine>> GetInvoiceLinesAsync(int invoiceId);

        // Invoice creation and updates
        Task<Invoice?> CreateInvoiceAsync(Invoice invoice);
        Task<Invoice?> UpdateInvoiceAsync(Invoice invoice);
        Task<bool> DeleteInvoiceAsync(int invoiceId);

        // Statistics and aggregations
        Task<int> GetInvoiceCountAsync();
        Task<decimal> GetTotalInvoiceValueAsync();
        Task<decimal> GetTotalOutstandingValueAsync();
        Task<int> GetOverdueInvoiceCountAsync();
        Task<object> GetInvoiceStatisticsAsync();

        // Refund operations
        Task<decimal> GetTotalRefundsAsync();
        Task<bool> UpdateRefundAmountAsync(int invoiceId, decimal refundAmount, string modifiedBy);
        Task<bool> UpdateRefundAmountAsync(string contractCode, short invoiceSeqno, decimal refundAmount, string modifiedBy);
        Task<bool> UpdateInvoiceLineRefundAsync(string contractCode, short invoiceSeqno, int contractlineNo, decimal refundAmount, string modifiedBy);
    }
}
