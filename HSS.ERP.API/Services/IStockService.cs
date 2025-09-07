using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public interface IStockService
    {
        Task<Dictionary<int, string>> GetStockNamesAsync(IEnumerable<int> stockNos);
        Task<Stock?> GetStockByNumberAsync(int stockNo, short divisionNo = 0);
        Task<IEnumerable<Stock>> SearchStocksAsync(string searchTerm, int limit = 50);
        Task<IEnumerable<Stock>> GetAllStocksAsync(int page = 1, int pageSize = 50, string? search = null, short? divisionNo = null, string? status = null);
        Task<int> GetStockCountAsync(string? search = null, short? divisionNo = null, string? status = null);
        Task<IEnumerable<short>> GetDivisionsAsync();
    }
}
