using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("hss_invoiceline", Schema = "tms")]
    public class InvoiceLine
    {
        [Key]
        [Column("invoice_line_id")]
        public int InvoiceLineId { get; set; }

        [Required]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Column("contract_code")]
        [StringLength(10)]
        public string ContractCode { get; set; } = string.Empty;

        [Column("invoice_seqno")]
        public short InvoiceSeqNo { get; set; }

        [Column("contractline_no")]
        public short ContractLineNo { get; set; }

        [Required]
        [Column("stock_no")]
        public int StockNo { get; set; } = 0;

        [Column("stock_name")]
        [StringLength(255)]
        public string StockName { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("stocktype_code")]
        public string StockTypeCode { get; set; } = string.Empty;

        [Required]
        [Column("invoiceline_qty")]
        public int InvoiceLineQty { get; set; } = 0;

        [Required]
        [Column("invoiceline_price", TypeName = "numeric(14,2)")]
        public decimal InvoiceLinePrice { get; set; } = 0;

        [Required]
        [Column("invoiceline_charge", TypeName = "numeric(14,2)")]
        public decimal InvoiceLineCharge { get; set; } = 0;

        [Required]
        [StringLength(1)]
        [Column("invoiceline_discountflag")]
        public string InvoiceLineDiscountFlag { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("invoiceline_periodflag")]
        public string InvoiceLinePeriodFlag { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("invoiceline_createflag")]
        public string InvoiceLineCreateFlag { get; set; } = string.Empty;

        [Required]
        [StringLength(3)]
        [Column("nomtype_code")]
        public string NomTypeCode { get; set; } = string.Empty;

        [Required]
        [Column("duration_no_full")]
        public int DurationNoFull { get; set; } = 0;

        [Required]
        [Column("duration_no_current")]
        public int DurationNoCurrent { get; set; } = 0;

        [Required]
        [StringLength(1)]
        [Column("invoice_finalflag")]
        public string InvoiceFinalFlag { get; set; } = string.Empty;

        [Required]
        [Column("vat_percent")]
        public float VatPercent { get; set; } = 0;

        [Required]
        [Column("invoiceline_lineno")]
        public short InvoiceLineLineNo { get; set; } = 0;

        [Required]
        [Column("invoiceline_discount")]
        public float InvoiceLineDiscount { get; set; } = 0;

        [Required]
        [Column("invoiceline_basiccharge", TypeName = "numeric(14,2)")]
        public decimal InvoiceLineBasicCharge { get; set; } = 0;

        [Required]
        [Column("invoiceline_autocharge", TypeName = "numeric(14,2)")]
        public decimal InvoiceLineAutoCharge { get; set; } = 0;

        [Required]
        [Column("invoiceline_preeditcharge", TypeName = "numeric(14,2)")]
        public decimal InvoiceLinePreEditCharge { get; set; } = 0;

        [Column("invoiceline_deliverydate")]
        public DateTime? InvoiceLineDeliveryDate { get; set; }

        [Required]
        [StringLength(15)]
        [Column("invoiceline_deliveryno")]
        public string InvoiceLineDeliveryNo { get; set; } = string.Empty;

        [Required]
        [Column("invoiceline_offhirediscount", TypeName = "numeric(14,2)")]
        public decimal InvoiceLineOffHireDiscount { get; set; } = 0;

        [Required]
        [StringLength(1)]
        [Column("invoiceline_offhireexclude")]
        public string InvoiceLineOffHireExclude { get; set; } = string.Empty;

        [Required]
        [Column("vatreverse_percent")]
        public float VatReversePercent { get; set; } = 0;

        [Column("tmsbookingline_no")]
        public int? TmsBookingLineNo { get; set; }

        [Column("processed_date")]
        public DateTime? ProcessedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; } = null!;

        // Computed properties for Teams App compatibility
        [NotMapped]
        public int Id => InvoiceLineId; // Use the new primary key

        [NotMapped]
        public string Description => !string.IsNullOrEmpty(StockName) ? StockName : "not specified";

        [NotMapped]
        public int Quantity => InvoiceLineQty;

        [NotMapped]
        public decimal UnitPrice => InvoiceLinePrice;

        [NotMapped]
        public decimal LineTotal => InvoiceLineCharge;

        [NotMapped]
        public decimal RefundAmount => InvoiceLineOffHireDiscount; // Using off-hire discount as refund amount

        [NotMapped]
        public string? ProductCode => StockNo.ToString();

        [NotMapped]
        public string? Unit => "ea"; // Default unit

        [NotMapped]
        public decimal TaxRate => (decimal)VatPercent;

        [NotMapped]
        public decimal TaxAmount => LineTotal * TaxRate / 100;

        [NotMapped]
        public DateTime CreatedAt => ProcessedDate ?? DateTime.UtcNow;

        [NotMapped]
        public string? LastModifiedBy => "System";

        [NotMapped]
        public DateTime? LastModifiedAt => ProcessedDate;
    }
}
