using System.ComponentModel.DataAnnotations;

namespace HSS.ERP.API.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        
        public string UserDisplayName { get; set; } = string.Empty;
        
        [Required]
        public string Action { get; set; } = string.Empty;
        
        public string Details { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string? TeamId { get; set; }
        
        public string? ChannelId { get; set; }
        
        public string IpAddress { get; set; } = string.Empty;
        
        public string UserAgent { get; set; } = string.Empty;
        
        // Navigation property
        public virtual User? User { get; set; }
    }
}
