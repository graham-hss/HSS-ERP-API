using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("tmsbody", Schema = "tms")]
    public class TmsBody
    {
        [Key]
        [Column("tmsbody_no")]
        public int TmsBodyNo { get; set; }

        [Column("tmsbody_name")]
        [StringLength(50)]
        public string TmsBodyName { get; set; } = string.Empty;

        [Column("tmsbody_website")]
        [StringLength(250)]
        public string TmsBodyWebsite { get; set; } = string.Empty;

        [Column("tmsbody_status")]
        [StringLength(1)]
        public string TmsBodyStatus { get; set; } = string.Empty;

        [Column("tmsbody_editcounter")]
        public int TmsBodyEditCounter { get; set; }

        [Column("tmsbody_delegatedisplay")]
        [StringLength(1)]
        public string TmsBodyDelegateDisplay { get; set; } = string.Empty;

        [Column("tmsintegration_no")]
        public int TmsIntegrationNo { get; set; }

        // Computed properties for display
        [NotMapped]
        public string SupplierName => TmsBodyName;

        [NotMapped]
        public string Website => TmsBodyWebsite;

        [NotMapped]
        public string Status => TmsBodyStatus;

        [NotMapped]
        public bool IsActive => TmsBodyStatus == "A" || TmsBodyStatus == "1" || string.IsNullOrEmpty(TmsBodyStatus);

        [NotMapped]
        public string DisplayStatus => IsActive ? "Active" : "Inactive";

        [NotMapped]
        public string StatusClass => IsActive ? "active" : "inactive";
    }
}
