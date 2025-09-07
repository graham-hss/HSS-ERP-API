using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("hss_invoice", Schema = "tms")]
    public class Invoice
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("contract_code")]
        [StringLength(10)]
        public string ContractCode { get; set; } = string.Empty;

        [Column("invoice_seqno")]
        public short InvoiceSeqNo { get; set; }

        [Required]
        [Column("unit_no_income")]
        public short UnitNoIncome { get; set; } = 0;

        [Required]
        [Column("period_no")]
        public short PeriodNo { get; set; } = 0;

        [Required]
        [StringLength(8)]
        [Column("cust_code")]
        public string CustomerCode { get; set; } = string.Empty;

        [Required]
        [Column("unit_no_control")]
        public short UnitNoControl { get; set; } = 0;

        [Required]
        [Column("contract_no")]
        public int ContractNo { get; set; } = 0;

        [StringLength(8)]
        [Column("invoice_slcode")]
        public string? InvoiceSlCode { get; set; }

        [Required]
        [StringLength(1)]
        [Column("invoicetype_code")]
        public string InvoiceTypeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("invoicestatus_code")]
        public string InvoiceStatusCode { get; set; } = string.Empty;

        [Column("invoice_sldate")]
        public DateTime? InvoiceSlDate { get; set; }

        [Column("invoice_createdate")]
        public DateTime? InvoiceCreateDate { get; set; }

        [Column("invoice_startdate")]
        public DateTime? InvoiceStartDate { get; set; }

        [Column("invoice_enddate")]
        public DateTime? InvoiceEndDate { get; set; }

        [Required]
        [Column("invoice_offhire")]
        public short InvoiceOffHire { get; set; } = 0;

        [Required]
        [Column("pr_no")]
        public int PrNo { get; set; } = 0;

        [Required]
        [Column("invoice_value", TypeName = "numeric(14,2)")]
        public decimal InvoiceValue { get; set; } = 0;

        [Required]
        [Column("invoice_vat", TypeName = "numeric(14,2)")]
        public decimal InvoiceVat { get; set; } = 0;

        [Required]
        [Column("cust_discount")]
        public float CustomerDiscount { get; set; } = 0;

        [Required]
        [Column("dw_percent")]
        public float DwPercent { get; set; } = 0;

        [Required]
        [StringLength(20)]
        [Column("invoice_order")]
        public string InvoiceOrder { get; set; } = string.Empty;

        [Required]
        [StringLength(8)]
        [Column("cpcomplaint_code")]
        public string CpComplaintCode { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("cnrteam_flag")]
        public string CnrTeamFlag { get; set; } = string.Empty;

        [Column("tmsbooking_no")]
        public int? TmsBookingNo { get; set; }

        [Column("processed_date")]
        public DateTime? ProcessedDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        [Column("processed_by")]
        public string? ProcessedBy { get; set; } = "Teams App";

        [StringLength(100)]
        [Column("azure_function_run_id")]
        public string? AzureFunctionRunId { get; set; }

        // Navigation properties
        public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
        public virtual ICollection<InvoiceHistory> InvoiceHistories { get; set; } = new List<InvoiceHistory>();
        
        // Temporarily disable customer navigation property to diagnose issue
        // [ForeignKey("CustomerCode")]
        // public virtual Customer? Customer { get; set; }

        // Computed properties for Teams App compatibility
        [NotMapped]
        public string InvoiceNumber => InvoiceId.ToString();

        [NotMapped]
        public DateTime InvoiceDate => InvoiceCreateDate ?? DateTime.UtcNow;

        [NotMapped]
        public DateTime DueDate => InvoiceEndDate ?? DateTime.UtcNow.AddDays(30);

        [NotMapped]
        public string CustomerName => CustomerCode; // Will need to join with customer table for actual name

        [NotMapped]
        public string CustomerEmail => string.Empty; // Will need to join with customer table

        [NotMapped]
        public string BillingAddress => string.Empty; // Will need to join with customer table

        [NotMapped]
        public decimal SubTotal => InvoiceValue;

        [NotMapped]
        public decimal TaxAmount => InvoiceVat;

        [NotMapped]
        public decimal TotalAmount => InvoiceValue + InvoiceVat;

        [NotMapped]
        public decimal RefundAmount => InvoiceLines?.Sum(l => l.RefundAmount) ?? 0;

        [NotMapped]
        public string Status => InvoiceStatusCode switch
        {
            "D" => "Draft",
            "S" => "Sent",
            "P" => "Paid",
            "O" => "Overdue",
            "C" => "Cancelled",
            _ => "Unknown"
        };

        [NotMapped]
        public string Notes => string.Empty;

        [NotMapped]
        public string CreatedBy => ProcessedBy ?? "System";

        [NotMapped]
        public DateTime CreatedAt => ProcessedDate ?? DateTime.UtcNow;

        [NotMapped]
        public string? LastModifiedBy => ProcessedBy;

        [NotMapped]
        public DateTime? LastModifiedAt => ProcessedDate;
    }
}
