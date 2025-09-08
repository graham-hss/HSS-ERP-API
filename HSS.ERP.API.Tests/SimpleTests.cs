using HSS.ERP.API.Models;

namespace HSS.ERP.API.Tests
{
    /// <summary>
    /// Simple tests to verify basic functionality without complex dependencies.
    /// </summary>
    public class SimpleTests
    {
        [Fact]
        public void Customer_ShouldCreateValidInstance_WhenPropertiesSet()
        {
            // Arrange & Act
            var customer = new Customer
            {
                CustomerCode = "TEST001",
                CustomerName = "Test Customer",
                Email = "test@example.com",
                Town = "Test Town",
                CustomerStatusCode = "ACTIVE"
            };

            // Assert
            customer.CustomerCode.Should().Be("TEST001");
            customer.CustomerName.Should().Be("Test Customer");
            customer.Email.Should().Be("test@example.com");
            customer.Town.Should().Be("Test Town");
            customer.CustomerStatusCode.Should().Be("ACTIVE");
        }

        [Fact]
        public void Invoice_ShouldCreateValidInstance_WhenPropertiesSet()
        {
            // Arrange & Act
            var invoice = new Invoice
            {
                InvoiceId = 1,
                CustomerCode = "CUST001",
                InvoiceCreateDate = DateTime.UtcNow,
                InvoiceValue = 100.50m,
                InvoiceStatusCode = "P"
            };

            // Assert
            invoice.InvoiceNumber.Should().Be("1"); // Computed property from InvoiceId
            invoice.CustomerCode.Should().Be("CUST001");
            invoice.InvoiceValue.Should().Be(100.50m);
            invoice.InvoiceStatusCode.Should().Be("P");
            invoice.InvoiceCreateDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Theory]
        [InlineData("CUST001", "Customer One")]
        [InlineData("CUST002", "Customer Two")]
        [InlineData("ACME123", "ACME Corporation")]
        public void Customer_ShouldAcceptDifferentCodes_WhenValidFormat(string code, string name)
        {
            // Arrange & Act
            var customer = new Customer
            {
                CustomerCode = code,
                CustomerName = name
            };

            // Assert
            customer.CustomerCode.Should().Be(code);
            customer.CustomerName.Should().Be(name);
        }

        [Fact]
        public void InvoiceLine_ShouldCalculateLineTotal_WhenQuantityAndPriceSet()
        {
            // Arrange
            var invoiceLine = new InvoiceLine
            {
                InvoiceId = 1,
                InvoiceLineLineNo = 1,
                StockNo = 1,
                InvoiceLineQty = 5,
                InvoiceLinePrice = 10.00m
            };

            // Act - Set the computed charge value
            invoiceLine.InvoiceLineCharge = invoiceLine.InvoiceLineQty * invoiceLine.InvoiceLinePrice;

            // Assert - Use computed property
            invoiceLine.LineTotal.Should().Be(50.00m);
        }

        [Theory]
        [InlineData(1, 10.00, 10.00)]
        [InlineData(3, 15.50, 46.50)]
        [InlineData(10, 2.99, 29.90)]
        public void InvoiceLine_ShouldCalculateCorrectTotal_ForDifferentQuantitiesAndPrices(int quantity, decimal unitPrice, decimal expectedTotal)
        {
            // Arrange
            var invoiceLine = new InvoiceLine
            {
                InvoiceLineQty = quantity,
                InvoiceLinePrice = unitPrice
            };

            // Act - Set the computed charge value
            invoiceLine.InvoiceLineCharge = invoiceLine.InvoiceLineQty * invoiceLine.InvoiceLinePrice;

            // Assert - Use computed property
            invoiceLine.LineTotal.Should().Be(expectedTotal);
        }

        [Fact]
        public void Stock_ShouldHaveDisplayName_WhenCreated()
        {
            // Arrange & Act
            var stock = new Stock
            {
                StockNo = 123,
                StockName = "Test Product",
                StockShortName = "TestProd"
            };

            // Assert
            stock.DisplayName.Should().Be("Test Product");
            stock.StockNo.Should().Be(123);
        }

        [Fact]
        public void Stock_DisplayName_ShouldFallbackToShortName_WhenNameIsEmpty()
        {
            // Arrange & Act
            var stock = new Stock
            {
                StockNo = 123,
                StockName = "",
                StockShortName = "TestProd"
            };

            // Assert
            stock.DisplayName.Should().Be("TestProd");
        }

        [Fact]
        public void Stock_DisplayName_ShouldFallbackToStockNumber_WhenNamesAreEmpty()
        {
            // Arrange & Act
            var stock = new Stock
            {
                StockNo = 123,
                StockName = "",
                StockShortName = ""
            };

            // Assert
            stock.DisplayName.Should().Be("Stock #123");
        }

        [Fact]
        public void Booking_Status_ShouldMapCorrectly_FromBookingTypeCode()
        {
            // Arrange & Act
            var draftBooking = new Booking { BookingTypeCode = "D" };
            var confirmedBooking = new Booking { BookingTypeCode = "C" };
            var paidBooking = new Booking { BookingTypeCode = "P" };
            var cancelledBooking = new Booking { BookingTypeCode = "X" };
            var unknownBooking = new Booking { BookingTypeCode = "?" };

            // Assert
            draftBooking.Status.Should().Be("Draft");
            confirmedBooking.Status.Should().Be("Confirmed");
            paidBooking.Status.Should().Be("Paid");
            cancelledBooking.Status.Should().Be("Cancelled");
            unknownBooking.Status.Should().Be("Unknown");
        }

        [Fact]
        public void Course_Status_ShouldMapCorrectly_FromCourseStatus()
        {
            // Arrange & Act
            var activeCourse = new Course { CourseStatus = "A" };
            var inactiveCourse = new Course { CourseStatus = "I" };
            var deletedCourse = new Course { CourseStatus = "D" };
            var pendingCourse = new Course { CourseStatus = "P" };
            var unknownCourse = new Course { CourseStatus = "?" };

            // Assert
            activeCourse.Status.Should().Be("Active");
            inactiveCourse.Status.Should().Be("Inactive");
            deletedCourse.Status.Should().Be("Deleted");
            pendingCourse.Status.Should().Be("Pending");
            unknownCourse.Status.Should().Be("Unknown");
        }

        [Theory]
        [InlineData(1.0, "1 day")]
        [InlineData(2.5, "2.5 days")]
        [InlineData(0.5, "0.5 days")]
        [InlineData(0, "Not specified")]
        public void Course_FormattedDuration_ShouldFormatCorrectly(decimal duration, string expected)
        {
            // Arrange & Act
            var course = new Course { CourseDuration = duration };

            // Assert
            course.FormattedDuration.Should().Be(expected);
        }
    }
}