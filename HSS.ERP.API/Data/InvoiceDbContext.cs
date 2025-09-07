using Microsoft.EntityFrameworkCore;
using HSS.ERP.API.Models;

namespace HSS.ERP.API.Data
{
    public class InvoiceDbContext : DbContext
    {
        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }
        public DbSet<InvoiceHistory> InvoiceHistories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingLine> BookingLines { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseCategory> CourseCategories { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<TmsBody> TmsBodies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary keys
            // Invoice now uses invoice_id as primary key (configured via [Key] attribute)
            // InvoiceLine now uses invoice_line_id as primary key (configured via [Key] attribute)
            // No need to explicitly configure primary keys as they're already defined in the models

            // Configure the relationship using the invoice_id foreign key
            // InvoiceLines are linked to Invoice via the invoice_id field
            modelBuilder.Entity<InvoiceLine>()
                .HasOne(e => e.Invoice)
                .WithMany(i => i.InvoiceLines)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add index on the new invoice_id field for performance
            modelBuilder.Entity<InvoiceLine>()
                .HasIndex(e => e.InvoiceId)
                .HasDatabaseName("IX_InvoiceLine_InvoiceId");

            // Configure InvoiceHistory relationship
            modelBuilder.Entity<InvoiceHistory>()
                .HasOne(h => h.Invoice)
                .WithMany(i => i.InvoiceHistories)
                .HasForeignKey(h => h.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add index on invoice_id for InvoiceHistory for performance
            modelBuilder.Entity<InvoiceHistory>()
                .HasIndex(h => h.InvoiceId)
                .HasDatabaseName("IX_InvoiceHistory_InvoiceId");

            // Ensure CustomerCode is treated as a simple property, not a foreign key
            // This prevents EF from creating shadow properties for relationships
            modelBuilder.Entity<Invoice>()
                .Property(i => i.CustomerCode)
                .IsRequired()
                .HasMaxLength(8);

            // Configure Customer entity explicitly
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerCode);

            // Configure Customer-Invoice relationship
            modelBuilder.Entity<Invoice>()
                .HasOne<Customer>()
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerCode)
                .HasPrincipalKey(c => c.CustomerCode);

            // Configure Booking relationships
            modelBuilder.Entity<BookingLine>()
                .HasKey(bl => new { bl.BookingNo, bl.BookingLineNo });

            modelBuilder.Entity<BookingLine>()
                .HasOne(bl => bl.Booking)
                .WithMany(b => b.BookingLines)
                .HasForeignKey(bl => bl.BookingNo)
                .OnDelete(DeleteBehavior.Restrict);

            // Add indexes for performance
            modelBuilder.Entity<BookingLine>()
                .HasIndex(bl => bl.BookingNo)
                .HasDatabaseName("IX_BookingLine_BookingNo");

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.CustomerCode)
                .HasDatabaseName("IX_Booking_CustomerCode");

            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingCreateDate)
                .HasDatabaseName("IX_Booking_CreateDate");

            // Configure Booking-Customer relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerCode)
                .HasPrincipalKey(c => c.CustomerCode);

            // Configure Course-CourseCategory relationship
            modelBuilder.Entity<Course>()
                .HasOne(c => c.CourseCategory)
                .WithMany(cc => cc.Courses)
                .HasForeignKey(c => c.CourseCategoryNo)
                .HasPrincipalKey(cc => cc.CourseCategoryNo)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Course-CourseType relationship  
            modelBuilder.Entity<Course>()
                .HasOne(c => c.CourseType)
                .WithMany(ct => ct.Courses)
                .HasForeignKey(c => c.CourseTypeCode)
                .HasPrincipalKey(ct => ct.CourseTypeCode)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure CourseCategory primary key is configured
            modelBuilder.Entity<CourseCategory>()
                .HasKey(cc => cc.CourseCategoryNo);

            // Ensure CourseType primary key is configured
            modelBuilder.Entity<CourseType>()
                .HasKey(ct => ct.CourseTypeCode);

            // Configure Stock entity primary key
            modelBuilder.Entity<Stock>()
                .HasKey(s => s.StockNo);

            // Configure BookingLine-Stock relationship
            modelBuilder.Entity<BookingLine>()
                .HasOne(bl => bl.Stock)
                .WithMany()
                .HasForeignKey(bl => bl.StockNo)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
