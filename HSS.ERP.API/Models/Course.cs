using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("tmscourse", Schema = "tms")]
    public class Course
    {
        [Key]
        [Column("tmscourse_no")]
        public int CourseNo { get; set; }

        [Required]
        [StringLength(1)]
        [Column("tmscoursetype_code")]
        public string CourseTypeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("tmscourse_code")]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Column("tmscourse_name")]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("tmscourse_webcode")]
        public string CourseWebCode { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("tmscourse_status")]
        public string CourseStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("tmscourse_publish")]
        public string CoursePublish { get; set; } = string.Empty;

        [Required]
        [Column("tmscourse_duration", TypeName = "numeric(5,2)")]
        public decimal CourseDuration { get; set; } = 0;

        [Required]
        [Column("tmscourse_mindelegate")]
        public short MinDelegates { get; set; } = 0;

        [Required]
        [Column("tmscourse_maxdelegate")]
        public short MaxDelegates { get; set; } = 0;

        [Required]
        [Column("tmscourse_suppliercost", TypeName = "numeric(14,2)")]
        public decimal SupplierCost { get; set; } = 0;

        [Required]
        [Column("tmscourse_sundrycost", TypeName = "numeric(14,2)")]
        public decimal SundryCost { get; set; } = 0;

        [Required]
        [Column("stock_no_delegate")]
        public int StockNoDelegate { get; set; } = 0;

        [Required]
        [Column("stock_no_fullwithitem")]
        public int StockNoFullWithItem { get; set; } = 0;

        [Required]
        [Column("stock_no_fullnoitem")]
        public int StockNoFullNoItem { get; set; } = 0;

        [Required]
        [StringLength(7)]
        [Column("smsupplier_code")]
        public string SupplierCode { get; set; } = string.Empty;

        [Required]
        [Column("smsupordadd_id")]
        public short SupplierOrderAddId { get; set; } = 0;

        [Required]
        [StringLength(1)]
        [Column("tmscourse_internallyhosted")]
        public string InternallyHosted { get; set; } = string.Empty;

        [Required]
        [Column("tmscourse_delegateceu")]
        public int DelegateCeu { get; set; } = 0;

        [Required]
        [Column("tmscourse_trainerceu")]
        public int TrainerCeu { get; set; } = 0;

        [Required]
        [Column("tmsoccupancy_no")]
        public int OccupancyNo { get; set; } = 0;

        [Required]
        [StringLength(250)]
        [Column("tmscourse_website")]
        public string CourseWebsite { get; set; } = string.Empty;

        [Required]
        [Column("tmscoursecategory_no")]
        public short CourseCategoryNo { get; set; } = 0;

        [Required]
        [StringLength(250)]
        [Column("tmscourse_joiningurl")]
        public string JoiningUrl { get; set; } = string.Empty;

        [Required]
        [Column("tmscourse_editcounter")]
        public int EditCounter { get; set; } = 0;

        [Required]
        [Column("tmsbody_no")]
        public int BodyNo { get; set; } = 0;

        [Required]
        [StringLength(50)]
        [Column("tmscourse_bodyref")]
        public string BodyRef { get; set; } = string.Empty;

        [Required]
        [Column("tmscitbtier_no")]
        public int CitbTierNo { get; set; } = 0;

        [Required]
        [StringLength(50)]
        [Column("tmscourse_citbref")]
        public string CitbRef { get; set; } = string.Empty;

        [Required]
        [Column("tmscerttype_no")]
        public int CertTypeNo { get; set; } = 0;

        [Required]
        [StringLength(100)]
        [Column("tmscourse_suppliercode")]
        public string CourseSupplierCode { get; set; } = string.Empty;

        // Navigation properties
        public virtual CourseCategory? CourseCategory { get; set; }
        public virtual CourseType? CourseType { get; set; }

        // Computed properties for Teams App compatibility
        [NotMapped]
        public int Id => CourseNo;

        [NotMapped]
        public string Name => CourseName;

        [NotMapped]
        public string Code => CourseCode;

        [NotMapped]
        public string WebCode => CourseWebCode;

        [NotMapped]
        public string Status => CourseStatus switch
        {
            "A" => "Active",
            "I" => "Inactive",
            "D" => "Deleted",
            "P" => "Pending",
            _ => "Unknown"
        };

        [NotMapped]
        public string Type => CourseType?.CourseTypeName ?? CourseTypeCode switch
        {
            "C" => "Classroom",
            "O" => "Online",
            "B" => "Blended",
            "W" => "Workshop",
            _ => "Unknown"
        };

        [NotMapped]
        public string CategoryName => CourseCategory?.CourseCategoryName ?? "No Category";

        [NotMapped]
        public bool IsPublished => CoursePublish == "Y";

        [NotMapped]
        public bool IsInternallyHosted => InternallyHosted == "Y";

        [NotMapped]
        public string FormattedDuration => CourseDuration > 0 
            ? $"{CourseDuration:0.##} {(CourseDuration == 1 ? "day" : "days")}"
            : "Not specified";

        [NotMapped]
        public string FormattedSupplierCost => SupplierCost > 0 
            ? $"£{SupplierCost:N2}" 
            : "No cost";

        [NotMapped]
        public string FormattedSundryCost => SundryCost > 0 
            ? $"£{SundryCost:N2}" 
            : "No cost";

        [NotMapped]
        public string DelegateRange => MinDelegates > 0 || MaxDelegates > 0 
            ? $"{MinDelegates}-{MaxDelegates} delegates"
            : "No limit specified";

        [NotMapped]
        public decimal TotalCost => SupplierCost + SundryCost;

        [NotMapped]
        public string FormattedTotalCost => TotalCost > 0 
            ? $"£{TotalCost:N2}" 
            : "No cost";
    }
}
