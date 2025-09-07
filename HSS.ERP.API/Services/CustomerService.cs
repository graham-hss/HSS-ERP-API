using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Data;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly InvoiceDbContext _context;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(InvoiceDbContext context, ILogger<CustomerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            // Return first 50 customers by default to avoid performance issues
            return await GetCustomersPagedAsync(1, 50);
        }

        public async Task<IEnumerable<Customer>> GetCustomersPagedAsync(int page = 1, int pageSize = 50)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 1000) pageSize = 50; // Max 1000 records per page

                return await _context.Customers
                    .OrderBy(c => c.CustomerCode) // Use CustomerCode for better performance (likely indexed)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers page {Page} with size {PageSize}", page, pageSize);
                throw;
            }
        }

        public async Task<int> GetCustomerCountAsync()
        {
            try
            {
                return await _context.Customers.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer count");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm, int page = 1, int pageSize = 50)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return await GetCustomersPagedAsync(page, pageSize);

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 1000) pageSize = 50;

                searchTerm = searchTerm.Trim().ToUpper();

                return await _context.Customers
                    .Where(c => c.CustomerCode.ToUpper().Contains(searchTerm) || 
                               c.CustomerName.ToUpper().Contains(searchTerm))
                    .OrderBy(c => c.CustomerCode)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with term '{SearchTerm}'", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByLetterAsync(string letter, int pageSize = 200)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(letter))
                    return new List<Customer>();

                if (pageSize < 1 || pageSize > 1000) pageSize = 200;

                letter = letter.Trim().ToUpper();

                if (letter == "0-9")
                {
                    // Handle numeric filter - use simple StartsWith for each digit
                    return await _context.Customers
                        .Where(c => c.CustomerCode.StartsWith("0") || c.CustomerCode.StartsWith("1") || 
                                   c.CustomerCode.StartsWith("2") || c.CustomerCode.StartsWith("3") || 
                                   c.CustomerCode.StartsWith("4") || c.CustomerCode.StartsWith("5") || 
                                   c.CustomerCode.StartsWith("6") || c.CustomerCode.StartsWith("7") || 
                                   c.CustomerCode.StartsWith("8") || c.CustomerCode.StartsWith("9") ||
                                   c.CustomerName.StartsWith("0") || c.CustomerName.StartsWith("1") || 
                                   c.CustomerName.StartsWith("2") || c.CustomerName.StartsWith("3") || 
                                   c.CustomerName.StartsWith("4") || c.CustomerName.StartsWith("5") || 
                                   c.CustomerName.StartsWith("6") || c.CustomerName.StartsWith("7") || 
                                   c.CustomerName.StartsWith("8") || c.CustomerName.StartsWith("9"))
                        .OrderBy(c => c.CustomerCode)
                        .Take(pageSize)
                        .ToListAsync();
                }
                else
                {
                    // Handle alphabetic filter
                    return await _context.Customers
                        .Where(c => c.CustomerCode.ToUpper().StartsWith(letter) || 
                                   c.CustomerName.ToUpper().StartsWith(letter))
                        .OrderBy(c => c.CustomerCode)
                        .Take(pageSize)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering customers by letter '{Letter}'", letter);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByCodeAsync(string customerCode)
        {
            try
            {
                return await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerCode == customerCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with code {CustomerCode}", customerCode);
                throw;
            }
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerCodeAsync(string customerCode)
        {
            try
            {
                return await _context.Invoices
                    .Include(i => i.InvoiceLines)
                    .Where(i => i.CustomerCode == customerCode)
                    .OrderByDescending(i => i.InvoiceCreateDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for customer {CustomerCode}", customerCode);
                throw;
            }
        }

        public async Task<Customer?> CreateCustomerAsync(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer with code {CustomerCode}", customer.CustomerCode);
                throw;
            }
        }

        public async Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerCode == customer.CustomerCode);

                if (existingCustomer == null)
                    return null;

                // Update properties
                existingCustomer.CustomerName = customer.CustomerName;
                existingCustomer.Address1 = customer.Address1;
                existingCustomer.Address2 = customer.Address2;
                existingCustomer.Address3 = customer.Address3;
                existingCustomer.Town = customer.Town;
                existingCustomer.County = customer.County;
                existingCustomer.Postcode = customer.Postcode;
                existingCustomer.Telephone = customer.Telephone;
                existingCustomer.Fax = customer.Fax;
                existingCustomer.Email = customer.Email;
                existingCustomer.VatNumber = customer.VatNumber;
                existingCustomer.ContactFlagRaw = customer.ContactFlagRaw;
                existingCustomer.VatFlagRaw = customer.VatFlagRaw;
                existingCustomer.NationNumber = customer.NationNumber;
                existingCustomer.Discount = customer.Discount;
                existingCustomer.CustomerTypeCode = customer.CustomerTypeCode;
                existingCustomer.CustomerStatusCode = customer.CustomerStatusCode;

                await _context.SaveChangesAsync();
                return existingCustomer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with code {CustomerCode}", customer.CustomerCode);
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(string customerCode)
        {
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerCode == customerCode);

                if (customer == null)
                    return false;

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with code {CustomerCode}", customerCode);
                throw;
            }
        }
    }
}
