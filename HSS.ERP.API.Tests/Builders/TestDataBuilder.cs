using HSS.ERP.API.Models;

namespace HSS.ERP.API.Tests.Builders
{
    /// <summary>
    /// Builder class for creating test data objects with sensible defaults.
    /// </summary>
    public static class TestDataBuilder
    {
        public static CustomerBuilder Customer() => new CustomerBuilder();
        public static InvoiceBuilder Invoice() => new InvoiceBuilder();
        public static InvoiceLineBuilder InvoiceLine() => new InvoiceLineBuilder();
        public static BookingBuilder Booking() => new BookingBuilder();
        public static CourseBuilder Course() => new CourseBuilder();
        public static StockBuilder Stock() => new StockBuilder();
    }

    public class CustomerBuilder
    {
        private readonly Customer _customer;

        public CustomerBuilder()
        {
            _customer = new Customer
            {
                CustomerCode = "CUST001",
                CustomerName = "Test Customer Ltd",
                Address1 = "123 Test Street",
                Address2 = "",
                Address3 = "",
                Town = "Test Town",
                County = "Test County",
                Postcode = "TE1 2ST",
                Telephone = "01234567890",
                Fax = "",
                Email = "test@customer.com",
                VatNumber = "GB123456789",
                ContactFlagRaw = "Y",
                VatFlagRaw = "Y",
                NationNumber = "GB",
                Discount = 0.0m,
                CustomerTypeCode = "STANDARD",
                CustomerStatusCode = "ACTIVE"
            };
        }

        public CustomerBuilder WithCode(string code)
        {
            _customer.CustomerCode = code;
            return this;
        }

        public CustomerBuilder WithName(string name)
        {
            _customer.CustomerName = name;
            return this;
        }

        public CustomerBuilder WithEmail(string email)
        {
            _customer.Email = email;
            return this;
        }

        public CustomerBuilder WithStatus(string status)
        {
            _customer.CustomerStatusCode = status;
            return this;
        }

        public Customer Build() => _customer;
    }

    public class InvoiceBuilder
    {
        private readonly Invoice _invoice;

        public InvoiceBuilder()
        {
            _invoice = new Invoice
            {
                InvoiceNumber = "INV-001",
                CustomerCode = "CUST001",
                InvoiceCreateDate = DateTime.UtcNow,
                InvoiceDueDate = DateTime.UtcNow.AddDays(30),
                InvoiceTotal = 100.00m,
                InvoiceStatus = "PENDING",
                InvoiceLines = new List<InvoiceLine>()
            };
        }

        public InvoiceBuilder WithNumber(string number)
        {
            _invoice.InvoiceNumber = number;
            return this;
        }

        public InvoiceBuilder WithCustomerCode(string customerCode)
        {
            _invoice.CustomerCode = customerCode;
            return this;
        }

        public InvoiceBuilder WithTotal(decimal total)
        {
            _invoice.InvoiceTotal = total;
            return this;
        }

        public InvoiceBuilder WithStatus(string status)
        {
            _invoice.InvoiceStatus = status;
            return this;
        }

        public InvoiceBuilder WithCreateDate(DateTime createDate)
        {
            _invoice.InvoiceCreateDate = createDate;
            return this;
        }

        public InvoiceBuilder WithLines(params InvoiceLine[] lines)
        {
            _invoice.InvoiceLines = lines.ToList();
            return this;
        }

        public Invoice Build() => _invoice;
    }

    public class InvoiceLineBuilder
    {
        private readonly InvoiceLine _invoiceLine;

        public InvoiceLineBuilder()
        {
            _invoiceLine = new InvoiceLine
            {
                InvoiceNumber = "INV-001",
                LineNumber = 1,
                StockCode = "STOCK001",
                StockName = "Test Product",
                Quantity = 1,
                UnitPrice = 100.00m,
                LineTotal = 100.00m,
                VatRate = 20.0m,
                VatAmount = 20.0m
            };
        }

        public InvoiceLineBuilder WithInvoiceNumber(string invoiceNumber)
        {
            _invoiceLine.InvoiceNumber = invoiceNumber;
            return this;
        }

        public InvoiceLineBuilder WithStockCode(string stockCode)
        {
            _invoiceLine.StockCode = stockCode;
            return this;
        }

        public InvoiceLineBuilder WithQuantity(int quantity)
        {
            _invoiceLine.Quantity = quantity;
            _invoiceLine.LineTotal = quantity * _invoiceLine.UnitPrice;
            _invoiceLine.VatAmount = _invoiceLine.LineTotal * (_invoiceLine.VatRate / 100);
            return this;
        }

        public InvoiceLineBuilder WithUnitPrice(decimal unitPrice)
        {
            _invoiceLine.UnitPrice = unitPrice;
            _invoiceLine.LineTotal = _invoiceLine.Quantity * unitPrice;
            _invoiceLine.VatAmount = _invoiceLine.LineTotal * (_invoiceLine.VatRate / 100);
            return this;
        }

        public InvoiceLine Build() => _invoiceLine;
    }

    public class BookingBuilder
    {
        private readonly Booking _booking;

        public BookingBuilder()
        {
            _booking = new Booking
            {
                BookingId = 1,
                CustomerCode = "CUST001",
                CourseCode = "COURSE001",
                BookingDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(32),
                NumberOfDelegates = 1,
                BookingStatus = "CONFIRMED",
                BookingLines = new List<BookingLine>()
            };
        }

        public BookingBuilder WithId(int id)
        {
            _booking.BookingId = id;
            return this;
        }

        public BookingBuilder WithCustomerCode(string customerCode)
        {
            _booking.CustomerCode = customerCode;
            return this;
        }

        public BookingBuilder WithCourseCode(string courseCode)
        {
            _booking.CourseCode = courseCode;
            return this;
        }

        public BookingBuilder WithStatus(string status)
        {
            _booking.BookingStatus = status;
            return this;
        }

        public BookingBuilder WithDelegates(int numberOfDelegates)
        {
            _booking.NumberOfDelegates = numberOfDelegates;
            return this;
        }

        public Booking Build() => _booking;
    }

    public class CourseBuilder
    {
        private readonly Course _course;

        public CourseBuilder()
        {
            _course = new Course
            {
                CourseCode = "COURSE001",
                CourseName = "Test Course",
                CourseDescription = "A test course for unit testing",
                Duration = 2,
                Price = 500.00m,
                MaxDelegates = 10,
                CourseCategory = "TRAINING",
                CourseType = "CLASSROOM",
                IsActive = true
            };
        }

        public CourseBuilder WithCode(string code)
        {
            _course.CourseCode = code;
            return this;
        }

        public CourseBuilder WithName(string name)
        {
            _course.CourseName = name;
            return this;
        }

        public CourseBuilder WithPrice(decimal price)
        {
            _course.Price = price;
            return this;
        }

        public CourseBuilder WithDuration(int duration)
        {
            _course.Duration = duration;
            return this;
        }

        public CourseBuilder WithMaxDelegates(int maxDelegates)
        {
            _course.MaxDelegates = maxDelegates;
            return this;
        }

        public CourseBuilder AsInactive()
        {
            _course.IsActive = false;
            return this;
        }

        public Course Build() => _course;
    }

    public class StockBuilder
    {
        private readonly Stock _stock;

        public StockBuilder()
        {
            _stock = new Stock
            {
                StockCode = "STOCK001",
                StockName = "Test Stock Item",
                StockDescription = "A test stock item",
                UnitPrice = 10.00m,
                StockLevel = 100,
                ReorderLevel = 10,
                IsActive = true,
                Category = "GENERAL"
            };
        }

        public StockBuilder WithCode(string code)
        {
            _stock.StockCode = code;
            return this;
        }

        public StockBuilder WithName(string name)
        {
            _stock.StockName = name;
            return this;
        }

        public StockBuilder WithPrice(decimal price)
        {
            _stock.UnitPrice = price;
            return this;
        }

        public StockBuilder WithStockLevel(int level)
        {
            _stock.StockLevel = level;
            return this;
        }

        public StockBuilder WithReorderLevel(int level)
        {
            _stock.ReorderLevel = level;
            return this;
        }

        public StockBuilder AsInactive()
        {
            _stock.IsActive = false;
            return this;
        }

        public Stock Build() => _stock;
    }
}