using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Customer>> GetCustomersPagedAsync(int page = 1, int pageSize = 50);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm, int page = 1, int pageSize = 50);
        Task<IEnumerable<Customer>> GetCustomersByLetterAsync(string letter, int pageSize = 200);
        Task<int> GetCustomerCountAsync();
        Task<Customer?> GetCustomerByCodeAsync(string customerCode);
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerCodeAsync(string customerCode);
        Task<Customer?> CreateCustomerAsync(Customer customer);
        Task<Customer?> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(string customerCode);
    }
}
