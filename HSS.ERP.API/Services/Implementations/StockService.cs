using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Data;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services.Implementations
{
    public class StockService : IStockService
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<StockService> _logger;

        public StockService(InvoiceDbContext context, ILogger<StockService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Dictionary<int, string>> GetStockNamesAsync(IEnumerable<int> stockNos)
        {
            try
            {
                var stockNosList = stockNos.ToList();
                if (!stockNosList.Any())
                {
                    return new Dictionary<int, string>();
                }

                // Query the tms.stock table first for stock names
                var stockNames = await _context.Stocks
                    .Where(s => stockNosList.Contains(s.StockNo) && !string.IsNullOrEmpty(s.StockName))
                    .GroupBy(s => s.StockNo)
                    .ToDictionaryAsync(g => g.Key, g => g.First().StockName);

                // If we didn't find all stock names in the Stock table, 
                // fall back to InvoiceLines for any missing ones
                var missingStockNos = stockNosList.Where(sn => !stockNames.ContainsKey(sn)).ToList();
                if (missingStockNos.Count > 0)
                {
                    var invoiceLineStockNames = await _context.InvoiceLines
                        .Where(il => missingStockNos.Contains(il.StockNo) && !string.IsNullOrEmpty(il.StockName))
                        .GroupBy(il => il.StockNo)
                        .ToDictionaryAsync(g => g.Key, g => g.First().StockName);

                    // Merge the results
                    foreach (var kvp in invoiceLineStockNames)
                    {
                        if (!stockNames.ContainsKey(kvp.Key))
                        {
                            stockNames[kvp.Key] = kvp.Value;
                        }
                    }
                }

                return stockNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock names for stock numbers: {StockNos}", string.Join(", ", stockNos));
                return new Dictionary<int, string>();
            }
        }

        public async Task<Stock?> GetStockByNumberAsync(int stockNo, short divisionNo = 0)
        {
            try
            {
                return await _context.Stocks
                    .FirstOrDefaultAsync(s => s.StockNo == stockNo && s.DivisionNo == divisionNo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock with number {StockNo} and division {DivisionNo}", stockNo, divisionNo);
                return null;
            }
        }

        public async Task<IEnumerable<Stock>> SearchStocksAsync(string searchTerm, int limit = 50)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return new List<Stock>();
                }

                var term = searchTerm.ToLower();

                return await _context.Stocks
                    .Where(s => s.StockName.ToLower().Contains(term) || 
                               s.StockShortName.ToLower().Contains(term) ||
                               s.StockCode.ToLower().Contains(term) ||
                               s.StockNo.ToString().Contains(term))
                    .OrderBy(s => s.StockName)
                    .Take(limit)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stocks with term: {SearchTerm}", searchTerm);
                return new List<Stock>();
            }
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync(int page = 1, int pageSize = 50, string? search = null, short? divisionNo = null, string? status = null)
        {
            try
            {
                _logger.LogInformation("GetAllStocksAsync called with page: {Page}, pageSize: {PageSize}, search: {Search}, division: {Division}, status: {Status}", 
                    page, pageSize, search, divisionNo, status);

                var query = _context.Stocks.AsQueryable();

                // Get total count before filtering for debugging
                var totalStocks = await _context.Stocks.CountAsync();
                _logger.LogInformation("Total stocks in database: {TotalStocks}", totalStocks);

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(search))
                {
                    _logger.LogInformation("Applying search filter: {Search}", search);
                    query = query.Where(s => 
                        (s.StockName != null && s.StockName.Contains(search)) ||
                        (s.StockShortName != null && s.StockShortName.Contains(search)) ||
                        (s.StockCode != null && s.StockCode.Contains(search)) ||
                        (s.StockLongName != null && s.StockLongName.Contains(search)));
                }

                // Apply division filter if provided
                if (divisionNo.HasValue)
                {
                    _logger.LogInformation("Applying division filter: {Division}", divisionNo.Value);
                    query = query.Where(s => s.DivisionNo == divisionNo.Value);
                }

                // Apply status filter if provided
                if (!string.IsNullOrWhiteSpace(status))
                {
                    _logger.LogInformation("Applying status filter: {Status}", status);
                    query = query.Where(s => s.StockStatus == status);
                }

                // Get count after filtering
                var filteredCount = await query.CountAsync();
                _logger.LogInformation("Filtered stock count: {FilteredCount}", filteredCount);

                // Apply pagination and ordering
                var result = await query
                    .OrderBy(s => s.StockNo)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Returning {ResultCount} stocks", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stocks for page {Page}, pageSize {PageSize}, search: {Search}, division: {Division}, status: {Status}", 
                    page, pageSize, search, divisionNo, status);
                return new List<Stock>();
            }
        }

        public async Task<int> GetStockCountAsync(string? search = null, short? divisionNo = null, string? status = null)
        {
            try
            {
                var query = _context.Stocks.AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(s => 
                        (s.StockName != null && s.StockName.Contains(search)) ||
                        (s.StockShortName != null && s.StockShortName.Contains(search)) ||
                        (s.StockCode != null && s.StockCode.Contains(search)) ||
                        (s.StockLongName != null && s.StockLongName.Contains(search)));
                }

                // Apply division filter if provided
                if (divisionNo.HasValue)
                {
                    query = query.Where(s => s.DivisionNo == divisionNo.Value);
                }

                // Apply status filter if provided
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(s => s.StockStatus == status);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting stocks with search: {Search}, division: {Division}, status: {Status}", 
                    search, divisionNo, status);
                return 0;
            }
        }

        public async Task<IEnumerable<short>> GetDivisionsAsync()
        {
            try
            {
                return await _context.Stocks
                    .Select(s => s.DivisionNo)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving divisions");
                return new List<short>();
            }
        }
    }
}
