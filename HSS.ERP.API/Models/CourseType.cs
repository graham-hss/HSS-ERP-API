using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("tmscoursetype", Schema = "tms")]
    public class CourseType
    {
        [Key]
        [Column("tmscoursetype_code")]
        [StringLength(1)]
        public string CourseTypeCode { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("tmscoursetype_name")]
        public string CourseTypeName { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("tmscoursetype_status")]
        public string CourseTypeStatus { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("tmscoursetype_isevent")]
        public string CourseTypeIsEvent { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
