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
                NationNumber = 44,
                Discount = 0.0f,
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
                InvoiceId = 1,
                CustomerCode = "CUST001",
                InvoiceCreateDate = DateTime.UtcNow,
                InvoiceEndDate = DateTime.UtcNow.AddDays(30),
                InvoiceValue = 100.00m,
                InvoiceStatusCode = "D",
                InvoiceLines = new List<InvoiceLine>()
            };
        }

        public InvoiceBuilder WithId(int id)
        {
            _invoice.InvoiceId = id;
            return this;
        }

        public InvoiceBuilder WithCustomerCode(string customerCode)
        {
            _invoice.CustomerCode = customerCode;
            return this;
        }

        public InvoiceBuilder WithTotal(decimal total)
        {
            _invoice.InvoiceValue = total;
            return this;
        }

        public InvoiceBuilder WithStatus(string status)
        {
            _invoice.InvoiceStatusCode = status;
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
                InvoiceId = 1,
                InvoiceLineLineNo = 1,
                StockNo = 1,
                StockName = "Test Product",
                InvoiceLineQty = 1,
                InvoiceLinePrice = 100.00m,
                InvoiceLineCharge = 100.00m,
                VatPercent = 20.0f
            };
        }

        public InvoiceLineBuilder WithInvoiceId(int invoiceId)
        {
            _invoiceLine.InvoiceId = invoiceId;
            return this;
        }

        public InvoiceLineBuilder WithStockNo(int stockNo)
        {
            _invoiceLine.StockNo = stockNo;
            return this;
        }

        public InvoiceLineBuilder WithQuantity(int quantity)
        {
            _invoiceLine.InvoiceLineQty = quantity;
            _invoiceLine.InvoiceLineCharge = quantity * _invoiceLine.InvoiceLinePrice;
            return this;
        }

        public InvoiceLineBuilder WithUnitPrice(decimal unitPrice)
        {
            _invoiceLine.InvoiceLinePrice = unitPrice;
            _invoiceLine.InvoiceLineCharge = _invoiceLine.InvoiceLineQty * unitPrice;
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
                BookingNo = 1,
                CustomerCode = "CUST001",
                BookingCreateDate = DateTime.UtcNow,
                BookingExpiryDate = DateTime.UtcNow.AddDays(30),
                BookingTypeCode = "C",
                BookingLines = new List<BookingLine>()
            };
        }

        public BookingBuilder WithNo(int bookingNo)
        {
            _booking.BookingNo = bookingNo;
            return this;
        }

        public BookingBuilder WithCustomerCode(string customerCode)
        {
            _booking.CustomerCode = customerCode;
            return this;
        }

        public BookingBuilder WithOrder(string order)
        {
            _booking.BookingOrder = order;
            return this;
        }

        public BookingBuilder WithStatus(string status)
        {
            _booking.BookingTypeCode = status;
            return this;
        }

        public BookingBuilder WithContact(string contact)
        {
            _booking.BookingContact = contact;
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
                CourseNo = 1,
                CourseCode = "COURSE001",
                CourseName = "Test Course",
                CourseTypeCode = "C",
                CourseDuration = 2.0m,
                MaxDelegates = 10,
                CourseStatus = "A"
            };
        }

        public CourseBuilder WithNo(int courseNo)
        {
            _course.CourseNo = courseNo;
            return this;
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

        public CourseBuilder WithSupplierCost(decimal cost)
        {
            _course.SupplierCost = cost;
            return this;
        }

        public CourseBuilder WithDuration(decimal duration)
        {
            _course.CourseDuration = duration;
            return this;
        }

        public CourseBuilder WithMaxDelegates(short maxDelegates)
        {
            _course.MaxDelegates = maxDelegates;
            return this;
        }

        public CourseBuilder AsActive()
        {
            _course.CourseStatus = "A";
            return this;
        }

        public CourseBuilder AsInactive()
        {
            _course.CourseStatus = "I";
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
                StockNo = 1,
                StockCode = "STOCK001",
                StockName = "Test Stock Item",
                StockLongName = "A test stock item",
                StockStatus = "A"
            };
        }

        public StockBuilder WithNo(int stockNo)
        {
            _stock.StockNo = stockNo;
            return this;
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

        public StockBuilder AsActive()
        {
            _stock.StockStatus = "A";
            return this;
        }

        public StockBuilder AsInactive()
        {
            _stock.StockStatus = "I";
            return this;
        }

        public Stock Build() => _stock;
    }
}