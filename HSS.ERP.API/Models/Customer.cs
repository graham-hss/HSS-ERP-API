using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HSS.ERP.API.Models
{
    [Table("cust", Schema = "tms")]
    public class Customer
    {
        [Key]
        [Column("cust_code")]
        public string CustomerCode { get; set; } = string.Empty;

        [Column("cust_name")]
        public string CustomerName { get; set; } = string.Empty;

        [Column("cust_addr1")]
        public string? Address1 { get; set; }

        [Column("cust_addr2")]
        public string? Address2 { get; set; }

        [Column("cust_addr3")]
        public string? Address3 { get; set; }

        [Column("cust_town")]
        public string? Town { get; set; }

        [Column("cust_county")]
        public string? County { get; set; }

        [Column("cust_postcode")]
        public string? Postcode { get; set; }

        [Column("cust_tel")]
        public string? Telephone { get; set; }

        [Column("cust_fax")]
        public string? Fax { get; set; }

        [Column("cust_email")]
        public string? Email { get; set; }

        [Column("cust_vatno")]
        public string? VatNumber { get; set; }

        [Column("cust_contactflag")]
        public string? ContactFlagRaw { get; set; }

        [Column("cust_vatflag")]
        public string? VatFlagRaw { get; set; }

        [Column("nation_no")]
        public int? NationNumber { get; set; }

        [Column("cust_discount")]
        public float? Discount { get; set; }

        [Column("custtype_code")]
        public string? CustomerTypeCode { get; set; }

        [Column("custstatus_code")]
        public string? CustomerStatusCode { get; set; }

        // Navigation properties
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // Computed properties for Teams app compatibility
        [NotMapped]
        public string FullAddress => string.Join(", ", new[] { Address1, Address2, Address3, Town, County, Postcode }.Where(x => !string.IsNullOrWhiteSpace(x)));

        [NotMapped]
        public string DisplayName => $"{CustomerCode} - {CustomerName}";

        // Boolean properties converted from database character flags
        [NotMapped]
        public bool ContactFlag => ContactFlagRaw?.ToUpper() == "Y" || ContactFlagRaw?.ToUpper() == "T" || ContactFlagRaw == "1";

        [NotMapped]
        public bool VatFlag => VatFlagRaw?.ToUpper() == "Y" || VatFlagRaw?.ToUpper() == "T" || VatFlagRaw == "1";
    }
}
