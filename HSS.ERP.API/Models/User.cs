using System.ComponentModel.DataAnnotations;

namespace HSS.ERP.API.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string DisplayName { get; set; } = string.Empty;
        
        public string AadObjectId { get; set; } = string.Empty;
        
        public string TenantId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Permission flags
        public bool CanViewInvoices { get; set; } = true;
        public bool CanEditInvoices { get; set; } = false;
        public bool CanDeleteInvoices { get; set; } = false;
        
        public bool CanViewCustomers { get; set; } = true;
        public bool CanEditCustomers { get; set; } = false;
        
        public bool CanViewCourses { get; set; } = true;
        public bool CanEditCourses { get; set; } = false;
        
        public bool CanViewReports { get; set; } = false;
        
        public bool IsAdmin { get; set; } = false;
        
        // Navigation properties
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
