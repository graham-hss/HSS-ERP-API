using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Data;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Services
{
    public class TmsBodyService : ITmsBodyService
    {
        private readonly InvoiceDbContext _context;

        public TmsBodyService(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TmsBody>> GetAllSuppliersAsync()
        {
            return await _context.TmsBodies
                .OrderBy(t => t.TmsBodyName)
                .ToListAsync();
        }

        public async Task<TmsBody?> GetSupplierByIdAsync(int tmsbodyNo)
        {
            return await _context.TmsBodies
                .FirstOrDefaultAsync(t => t.TmsBodyNo == tmsbodyNo);
        }

        public async Task<IEnumerable<TmsBody>> SearchSuppliersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllSuppliersAsync();

            var lowerSearchTerm = searchTerm.ToLower();
            
            return await _context.TmsBodies
                .Where(t => t.TmsBodyName.ToLower().Contains(lowerSearchTerm) ||
                           (t.TmsBodyWebsite != null && t.TmsBodyWebsite.ToLower().Contains(lowerSearchTerm)))
                .OrderBy(t => t.TmsBodyName)
                .ToListAsync();
        }

        public async Task<IEnumerable<TmsBody>> GetActiveSuppliersAsync()
        {
            return await _context.TmsBodies
                .Where(t => t.IsActive)
                .OrderBy(t => t.TmsBodyName)
                .ToListAsync();
        }
    }
}
