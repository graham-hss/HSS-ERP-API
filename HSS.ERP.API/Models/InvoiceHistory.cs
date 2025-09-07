using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("invoice_history", Schema = "tms")]
    public class InvoiceHistory
    {
        [Key]
        [Column("history_id")]
        public int HistoryId { get; set; }

        [Required]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("history_type")]
        public string HistoryType { get; set; } = "note";

        [Required]
        [StringLength(255)]
        [Column("history_title")]
        public string HistoryTitle { get; set; } = string.Empty;

        [Required]
        [Column("history_content")]
        public string HistoryContent { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("created_by")]
        public string CreatedBy { get; set; } = string.Empty;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; } = null!;

        // Computed properties for Teams App compatibility
        [NotMapped]
        public int Id => HistoryId;

        [NotMapped]
        public string Title => HistoryTitle;

        [NotMapped]
        public string Content => HistoryContent;

        [NotMapped]
        public string Type => HistoryType;

        [NotMapped]
        public string Author => CreatedBy;

        [NotMapped]
        public DateTime Date => CreatedAt;

        [NotMapped]
        public string FormattedDate => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    }
}
