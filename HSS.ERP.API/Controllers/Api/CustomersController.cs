using Microsoft.AspNetCore.Mvc;
using HSS.ERP.API.Services;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var customers = await _customerService.GetCustomersPagedAsync(page, pageSize);
                var totalCount = await _customerService.GetCustomerCountAsync();
                
                // Add pagination metadata to response headers
                Response.Headers["X-Total-Count"] = totalCount.ToString();
                Response.Headers["X-Page"] = page.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();
                Response.Headers["X-Total-Pages"] = ((int)Math.Ceiling((double)totalCount / pageSize)).ToString();

                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                return StatusCode(500, "Internal server error occurred while retrieving customers");
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            try
            {
                _logger.LogWarning("GetAllCustomers called - this will return only the first 50 customers for performance");
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                return StatusCode(500, "Internal server error occurred while retrieving customers");
            }
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<Customer>> GetCustomer(string code)
        {
            try
            {
                var customer = await _customerService.GetCustomerByCodeAsync(code);
                if (customer == null)
                {
                    return NotFound($"Customer with code {code} not found");
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerCode}", code);
                return StatusCode(500, "Internal server error occurred while retrieving customer");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            try
            {
                var createdCustomer = await _customerService.CreateCustomerAsync(customer);
                return CreatedAtAction(nameof(GetCustomer), new { code = createdCustomer.CustomerCode }, createdCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, "Internal server error occurred while creating customer");
            }
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateCustomer(string code, Customer customer)
        {
            if (code != customer.CustomerCode)
            {
                return BadRequest("Customer code mismatch");
            }

            try
            {
                await _customerService.UpdateCustomerAsync(customer);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerCode}", code);
                return StatusCode(500, "Internal server error occurred while updating customer");
            }
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteCustomer(string code)
        {
            try
            {
                await _customerService.DeleteCustomerAsync(code);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerCode}", code);
                return StatusCode(500, "Internal server error occurred while deleting customer");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> SearchCustomers([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Search query cannot be empty");
                }

                var customers = await _customerService.SearchCustomersAsync(query);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with query: {Query}", query);
                return StatusCode(500, "Internal server error occurred while searching customers");
            }
        }
    }
}