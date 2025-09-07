using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("tmsbookingline", Schema = "tms")]
    public class BookingLine
    {
        [Key, Column("tmsbooking_no", Order = 0)]
        public int BookingNo { get; set; }

        [Key, Column("tmsbookingline_no", Order = 1)]
        public int BookingLineNo { get; set; }

        [Required]
        [Column("tmsbookingline_lineref")]
        public int BookingLineRef { get; set; } = 0;

        [Required]
        [Column("contract_no")]
        public int ContractNo { get; set; } = 0;

        [Required]
        [Column("stock_no")]
        public int StockNo { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_qty")]
        public int BookingLineQty { get; set; } = 0;

        [Required]
        [StringLength(1)]
        [Column("tmsbookingline_type")]
        public string BookingLineType { get; set; } = string.Empty;

        [Required]
        [Column("tmsbookingline_ref")]
        public int BookingLineRefNo { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_basicprice", TypeName = "numeric(14,2)")]
        public decimal BookingLineBasicPrice { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_autoprice", TypeName = "numeric(14,2)")]
        public decimal BookingLineAutoPrice { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_price", TypeName = "numeric(14,2)")]
        public decimal BookingLinePrice { get; set; } = 0;

        [Required]
        [StringLength(1)]
        [Column("tmsbookingline_priceeditflag")]
        public string BookingLinePriceEditFlag { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("tmsbookingline_autodisctype")]
        public string BookingLineAutoDiscType { get; set; } = string.Empty;

        [Required]
        [Column("tmsbookingline_autodiscref")]
        public int BookingLineAutoDiscRef { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_autodiscpercent", TypeName = "numeric(5,2)")]
        public decimal BookingLineAutoDiscPercent { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_vatpercent", TypeName = "numeric(4,2)")]
        public decimal BookingLineVatPercent { get; set; } = 0;

        [Required]
        [StringLength(1)]
        [Column("tmsbookingline_deliverytype")]
        public string BookingLineDeliveryType { get; set; } = string.Empty;

        [Required]
        [Column("tmsbookingdelivery_no")]
        public int BookingDeliveryNo { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_reqqty")]
        public int BookingLineReqQty { get; set; } = 0;

        [Required]
        [Column("tmsbookingline_cancqty")]
        public int BookingLineCancQty { get; set; } = 0;

        [Required]
        [Column("contractline_no")]
        public short ContractLineNo { get; set; } = 0;

        [Required]
        [Column("tmsevent_no_delivery")]
        public int EventNoDelivery { get; set; } = 0;

        // Navigation properties
        [ForeignKey("BookingNo")]
        public virtual Booking Booking { get; set; } = null!;

        [ForeignKey("StockNo")]
        public virtual Stock? Stock { get; set; }

        // Computed properties for Teams App compatibility
        [NotMapped]
        public string Description => Stock?.DisplayName ?? $"Stock #{StockNo}"; // Now uses actual stock name from Stock navigation property

        [NotMapped]
        public int Quantity => BookingLineQty;

        [NotMapped]
        public decimal UnitPrice => BookingLinePrice;

        [NotMapped]
        public decimal BasicPrice => BookingLineBasicPrice;

        [NotMapped]
        public decimal AutoPrice => BookingLineAutoPrice;

        [NotMapped]
        public decimal LineTotal => BookingLinePrice * BookingLineQty;

        [NotMapped]
        public decimal VatRate => BookingLineVatPercent;

        [NotMapped]
        public decimal VatAmount => LineTotal * VatRate / 100;

        [NotMapped]
        public decimal DiscountPercent => BookingLineAutoDiscPercent;

        [NotMapped]
        public int RequestedQty => BookingLineReqQty;

        [NotMapped]
        public int CancelledQty => BookingLineCancQty;

        [NotMapped]
        public int AvailableQty => RequestedQty - CancelledQty;

        [NotMapped]
        public string LineType => BookingLineType switch
        {
            "H" => "Hire",
            "S" => "Sale",
            "D" => "Damage",
            "L" => "Labour",
            _ => "Other"
        };

        [NotMapped]
        public string DeliveryType => BookingLineDeliveryType switch
        {
            "D" => "Delivery",
            "C" => "Collection",
            "S" => "Self Service",
            _ => "TBD"
        };

        [NotMapped]
        public bool IsPriceEdited => BookingLinePriceEditFlag?.ToUpper() == "Y" || BookingLinePriceEditFlag?.ToUpper() == "T";

        [NotMapped]
        public string ProductCode => StockNo.ToString();

        [NotMapped]
        public int LineNumber => BookingLineNo;

        [NotMapped]
        public string FormattedLineTotal => $"£{LineTotal:N2}";

        [NotMapped]
        public string FormattedUnitPrice => $"£{UnitPrice:N2}";
    }
}
