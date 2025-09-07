using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("tmscoursecategory", Schema = "tms")]
    public class CourseCategory
    {
        [Key]
        [Column("tmscoursecategory_no")]
        public short CourseCategoryNo { get; set; }

        [Required]
        [StringLength(50)]
        [Column("tmscoursecategory_name")]
        public string CourseCategoryName { get; set; } = string.Empty;

        [Required]
        [StringLength(1)]
        [Column("tmscoursecategory_status")]
        public string CourseCategoryStatus { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
