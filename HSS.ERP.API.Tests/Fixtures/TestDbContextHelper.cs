using HSS.ERP.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HSS.ERP.API.Tests.Fixtures
{
    /// <summary>
    /// Helper class for creating test database contexts with in-memory database.
    /// </summary>
    public static class TestDbContextHelper
    {
        /// <summary>
        /// Creates a new in-memory database context for testing.
        /// </summary>
        /// <param name="databaseName">Optional unique name for the database. If not provided, a GUID will be used.</param>
        /// <returns>A configured InvoiceDbContext using in-memory database.</returns>
        public static InvoiceDbContext CreateInMemoryContext(string? databaseName = null)
        {
            databaseName ??= Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<InvoiceDbContext>()
                .UseInMemoryDatabase(databaseName)
                .EnableSensitiveDataLogging()
                .Options;

            var context = new InvoiceDbContext(options);
            
            // Ensure the database is created
            context.Database.EnsureCreated();
            
            return context;
        }

        /// <summary>
        /// Creates a test database context seeded with common test data.
        /// </summary>
        /// <param name="databaseName">Optional unique name for the database.</param>
        /// <returns>A configured InvoiceDbContext with seeded test data.</returns>
        public static InvoiceDbContext CreateSeededContext(string? databaseName = null)
        {
            var context = CreateInMemoryContext(databaseName);
            SeedTestData(context);
            return context;
        }

        /// <summary>
        /// Seeds the provided context with common test data.
        /// </summary>
        /// <param name="context">The database context to seed.</param>
        public static void SeedTestData(InvoiceDbContext context)
        {
            // Add test customers
            var customers = new[]
            {
                Builders.TestDataBuilder.Customer()
                    .WithCode("CUST001")
                    .WithName("Test Customer 1")
                    .WithEmail("test1@customer.com")
                    .Build(),
                Builders.TestDataBuilder.Customer()
                    .WithCode("CUST002")
                    .WithName("Test Customer 2")
                    .WithEmail("test2@customer.com")
                    .Build(),
                Builders.TestDataBuilder.Customer()
                    .WithCode("CUST003")
                    .WithName("Inactive Customer")
                    .WithStatus("INACTIVE")
                    .Build()
            };

            context.Customers.AddRange(customers);

            // Add test stock items
            var stockItems = new[]
            {
                Builders.TestDataBuilder.Stock()
                    .WithNo(1)
                    .WithCode("STOCK001")
                    .WithName("Test Product 1")
                    .Build(),
                Builders.TestDataBuilder.Stock()
                    .WithNo(2)
                    .WithCode("STOCK002")
                    .WithName("Test Product 2")
                    .Build(),
                Builders.TestDataBuilder.Stock()
                    .WithNo(3)
                    .WithCode("STOCK003")
                    .WithName("Out of Stock Product")
                    .Build()
            };

            context.Stocks.AddRange(stockItems);

            // Add test courses
            var courses = new[]
            {
                Builders.TestDataBuilder.Course()
                    .WithNo(1)
                    .WithCode("COURSE001")
                    .WithName("Introduction to Safety")
                    .WithSupplierCost(200.00m)
                    .WithDuration(1)
                    .Build(),
                Builders.TestDataBuilder.Course()
                    .WithNo(2)
                    .WithCode("COURSE002")
                    .WithName("Advanced Safety Training")
                    .WithSupplierCost(500.00m)
                    .WithDuration(3)
                    .WithMaxDelegates(8)
                    .Build(),
                Builders.TestDataBuilder.Course()
                    .WithNo(3)
                    .WithCode("COURSE003")
                    .WithName("Inactive Course")
                    .AsInactive()
                    .Build()
            };

            context.Courses.AddRange(courses);

            // Add test invoices
            var invoices = new[]
            {
                Builders.TestDataBuilder.Invoice()
                    .WithId(1)
                    .WithCustomerCode("CUST001")
                    .WithTotal(100.00m)
                    .WithStatus("PAID")
                    .WithCreateDate(DateTime.UtcNow.AddDays(-30))
                    .Build(),
                Builders.TestDataBuilder.Invoice()
                    .WithId(2)
                    .WithCustomerCode("CUST002")
                    .WithTotal(250.00m)
                    .WithStatus("PENDING")
                    .WithCreateDate(DateTime.UtcNow.AddDays(-15))
                    .Build()
            };

            context.Invoices.AddRange(invoices);

            // Add test invoice lines
            var invoiceLines = new[]
            {
                Builders.TestDataBuilder.InvoiceLine()
                    .WithInvoiceId(1)
                    .WithStockNo(1)
                    .WithQuantity(2)
                    .WithUnitPrice(50.00m)
                    .Build(),
                Builders.TestDataBuilder.InvoiceLine()
                    .WithInvoiceId(2)
                    .WithStockNo(2)
                    .WithQuantity(3)
                    .WithUnitPrice(75.00m)
                    .Build()
            };

            context.InvoiceLines.AddRange(invoiceLines);

            // Add test bookings
            var bookings = new[]
            {
                Builders.TestDataBuilder.Booking()
                    .WithNo(1)
                    .WithCustomerCode("CUST001")
                    .WithOrder("COURSE001")
                    .WithStatus("C")
                    .WithContact("John Doe")
                    .Build(),
                Builders.TestDataBuilder.Booking()
                    .WithNo(2)
                    .WithCustomerCode("CUST002")
                    .WithOrder("COURSE002")
                    .WithStatus("D")
                    .WithContact("Jane Smith")
                    .Build()
            };

            context.Bookings.AddRange(bookings);

            context.SaveChanges();
        }

        /// <summary>
        /// Clears all data from the provided context.
        /// </summary>
        /// <param name="context">The database context to clear.</param>
        public static void ClearTestData(InvoiceDbContext context)
        {
            context.InvoiceLines.RemoveRange(context.InvoiceLines);
            context.Invoices.RemoveRange(context.Invoices);
            context.Bookings.RemoveRange(context.Bookings);
            context.Courses.RemoveRange(context.Courses);
            context.Stocks.RemoveRange(context.Stocks);
            context.Customers.RemoveRange(context.Customers);
            context.SaveChanges();
        }
    }
}