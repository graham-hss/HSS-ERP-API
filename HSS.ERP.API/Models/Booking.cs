using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("tmsbooking", Schema = "tms")]
    public class Booking
    {
        [Key]
        [Column("tmsbooking_no")]
        public int BookingNo { get; set; }

        [Required]
        [StringLength(1)]
        [Column("tmsbookingtype_code")]
        public string BookingTypeCode { get; set; } = string.Empty;

        [Required]
        [Column("tmswebsite_no")]
        public int WebsiteNo { get; set; } = 0;

        [Column("tmsbooking_createdate")]
        public DateTime? BookingCreateDate { get; set; }

        [Column("tmsbooking_expirydate")]
        public DateTime? BookingExpiryDate { get; set; }

        [Required]
        [Column("staff_no_create")]
        public int StaffNoCreate { get; set; } = 0;

        [Required]
        [StringLength(8)]
        [Column("cust_code")]
        public string CustomerCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("tmsbooking_order")]
        public string BookingOrder { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("tmsbooking_contact")]
        public string BookingContact { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("tmsbooking_tel")]
        public string BookingTel { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("tmsbooking_email")]
        public string BookingEmail { get; set; } = string.Empty;

        [Required]
        [Column("tmsbooking_charge", TypeName = "numeric(14,2)")]
        public decimal BookingCharge { get; set; } = 0;

        [Required]
        [Column("tmsbooking_vat", TypeName = "numeric(14,2)")]
        public decimal BookingVat { get; set; } = 0;

        [Required]
        [StringLength(50)]
        [Column("tmsbooking_token")]
        public string BookingToken { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Column("tmsbooking_bacsref")]
        public string BookingBacsRef { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("tmsbookingsource_code")]
        public string BookingSourceCode { get; set; } = string.Empty;

        [Required]
        [Column("unit_no_source")]
        public short UnitNoSource { get; set; } = 0;

        [Required]
        [Column("staff_no_source")]
        public int StaffNoSource { get; set; } = 0;

        [Required]
        [Column("tmsbooking_editcounter")]
        public int BookingEditCounter { get; set; } = 0;

        [Required]
        [StringLength(1000)]
        [Column("tmsbooking_notes")]
        public string BookingNotes { get; set; } = string.Empty;

        [Required]
        [Column("tmscontact_no")]
        public int ContactNo { get; set; } = 0;

        [Required]
        [Column("unit_no_income")]
        public short UnitNoIncome { get; set; } = 0;

        [Required]
        [StringLength(20)]
        [Column("tmsbooking_merchant")]
        public string BookingMerchant { get; set; } = string.Empty;

        [Required]
        [Column("tmsbooking_captured", TypeName = "numeric(14,2)")]
        public decimal BookingCaptured { get; set; } = 0;

        [Required]
        [Column("tmsbooking_refunded", TypeName = "numeric(14,2)")]
        public decimal BookingRefunded { get; set; } = 0;

        [Required]
        [Column("tmsbooking_danteid")]
        public int BookingDanteId { get; set; } = 0;

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<BookingLine> BookingLines { get; set; } = new List<BookingLine>();

        // Computed properties for Teams App compatibility
        [NotMapped]
        public int Id => BookingNo;

        [NotMapped]
        public string BookingNumber => BookingNo.ToString();

        [NotMapped]
        public DateTime BookingDate => BookingCreateDate ?? DateTime.UtcNow;

        [NotMapped]
        public DateTime? ExpiryDate => BookingExpiryDate;

        [NotMapped]
        public string CustomerName => Customer?.CustomerName ?? CustomerCode;

        [NotMapped]
        public string ContactName => BookingContact;

        [NotMapped]
        public string Email => BookingEmail;

        [NotMapped]
        public string Telephone => BookingTel;

        [NotMapped]
        public decimal SubTotal => BookingCharge;

        [NotMapped]
        public decimal VatAmount => BookingVat;

        [NotMapped]
        public decimal TotalAmount => BookingCharge + BookingVat;

        [NotMapped]
        public decimal CapturedAmount => BookingCaptured;

        [NotMapped]
        public decimal RefundedAmount => BookingRefunded;

        [NotMapped]
        public string Status => BookingTypeCode switch
        {
            "D" => "Draft",
            "C" => "Confirmed",
            "P" => "Paid",
            "X" => "Cancelled",
            _ => "Unknown"
        };

        [NotMapped]
        public string Notes => BookingNotes;

        [NotMapped]
        public string OrderReference => BookingOrder;

        [NotMapped]
        public string FormattedDate => BookingDate.ToString("dd/MM/yyyy");

        [NotMapped]
        public string FormattedExpiryDate => ExpiryDate?.ToString("dd/MM/yyyy") ?? "N/A";

        [NotMapped]
        public int LineCount => BookingLines?.Count ?? 0;
    }
}
