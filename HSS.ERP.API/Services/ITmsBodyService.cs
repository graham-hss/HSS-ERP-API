using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public interface ITmsBodyService
    {
        Task<IEnumerable<TmsBody>> GetAllSuppliersAsync();
        Task<TmsBody?> GetSupplierByIdAsync(int tmsbodyNo);
        Task<IEnumerable<TmsBody>> SearchSuppliersAsync(string searchTerm);
        Task<IEnumerable<TmsBody>> GetActiveSuppliersAsync();
    }
}
