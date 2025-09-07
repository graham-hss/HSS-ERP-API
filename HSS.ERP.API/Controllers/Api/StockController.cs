using Microsoft.AspNetCore.Mvc;
using HSS.ERP.API.Services;

namespace HSS.ERP.API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockService stockService, ILogger<StockController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchStock([FromQuery] string term, [FromQuery] int limit = 50)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return Ok(new { stocks = new object[0] });
                }

                var stocks = await _stockService.SearchStocksAsync(term, limit);

                return Ok(new
                {
                    stocks = stocks.Select(s => new
                    {
                        stockNo = s.StockNo,
                        divisionNo = s.DivisionNo,
                        stockName = s.StockName,
                        stockShortName = s.StockShortName,
                        stockCode = s.StockCode,
                        displayName = s.DisplayName,
                        stockStatus = s.StockStatus
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stock with term: {Term}", term);
                return StatusCode(500, new { message = "Error searching stock", error = ex.Message });
            }
        }

        [HttpGet("{stockNo}")]
        public async Task<ActionResult<object>> GetStock(int stockNo, [FromQuery] short divisionNo = 0)
        {
            try
            {
                var stock = await _stockService.GetStockByNumberAsync(stockNo, divisionNo);

                if (stock == null)
                {
                    return NotFound(new { message = $"Stock #{stockNo} not found" });
                }

                return Ok(new
                {
                    stockNo = stock.StockNo,
                    divisionNo = stock.DivisionNo,
                    stockName = stock.StockName,
                    stockShortName = stock.StockShortName,
                    stockCode = stock.StockCode,
                    displayName = stock.DisplayName,
                    stockStatus = stock.StockStatus,
                    stockLongName = stock.StockLongName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock {StockNo}", stockNo);
                return StatusCode(500, new { message = "Error retrieving stock", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetStocks(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 50, 
            [FromQuery] string? search = null,
            [FromQuery] short? divisionNo = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var stocks = await _stockService.GetAllStocksAsync(page, pageSize, search, divisionNo, status);
                var totalCount = await _stockService.GetStockCountAsync(search, divisionNo, status);

                return Ok(new
                {
                    stocks = stocks.Select(s => new
                    {
                        stockNo = s.StockNo,
                        divisionNo = s.DivisionNo,
                        stockName = s.StockName,
                        stockShortName = s.StockShortName,
                        stockCode = s.StockCode,
                        displayName = s.DisplayName,
                        stockStatus = s.StockStatus,
                        stockLongName = s.StockLongName
                    }),
                    totalCount = totalCount,
                    page = page,
                    pageSize = pageSize,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stocks for page {Page}", page);
                return StatusCode(500, new { message = "Error retrieving stocks", error = ex.Message });
            }
        }

        [HttpGet("divisions")]
        public async Task<ActionResult<IEnumerable<short>>> GetDivisions()
        {
            try
            {
                var divisions = await _stockService.GetDivisionsAsync();
                return Ok(divisions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving divisions");
                return StatusCode(500, new { message = "Error retrieving divisions", error = ex.Message });
            }
        }
    }
}