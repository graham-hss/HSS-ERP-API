using HSS.ERP.API.Services;
using HSS.ERP.API.Tests.Fixtures;
using HSS.ERP.API.Tests.Builders;
using Microsoft.Extensions.Logging;

namespace HSS.ERP.API.Tests.Services
{
    /// <summary>
    /// Unit tests for CustomerService functionality.
    /// Tests cover all CRUD operations, search functionality, and error handling scenarios.
    /// </summary>
    public class CustomerServiceTests : IDisposable
    {
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly CustomerService _customerService;
        private readonly HSS.ERP.API.Data.InvoiceDbContext _context;

        public CustomerServiceTests()
        {
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _context = TestDbContextHelper.CreateInMemoryContext();
            _customerService = new CustomerService(_context, _loggerMock.Object);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Theory, AutoData]
        public async Task GetAllCustomersAsync_ShouldReturnFirst50Customers_WhenCalled(int numberOfCustomers)
        {
            // Arrange
            numberOfCustomers = Math.Max(60, numberOfCustomers % 100); // Ensure we have more than 50
            var customers = Enumerable.Range(1, numberOfCustomers)
                .Select(i => TestDataBuilder.Customer()
                    .WithCode($"CUST{i:D3}")
                    .WithName($"Customer {i}")
                    .Build())
                .ToList();

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(50, "GetAllCustomersAsync should return max 50 customers for performance");
            result.Should().BeInAscendingOrder(c => c.CustomerCode, "Results should be ordered by CustomerCode");
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(2, 15)]
        [InlineData(3, 20)]
        public async Task GetCustomersPagedAsync_ShouldReturnCorrectPage_WhenValidParametersProvided(int page, int pageSize)
        {
            // Arrange
            var totalCustomers = 50;
            var customers = Enumerable.Range(1, totalCustomers)
                .Select(i => TestDataBuilder.Customer()
                    .WithCode($"CUST{i:D3}")
                    .WithName($"Customer {i}")
                    .Build())
                .ToList();

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetCustomersPagedAsync(page, pageSize);

            // Assert
            result.Should().NotBeNull();
            
            var expectedCount = Math.Min(pageSize, Math.Max(0, totalCustomers - (page - 1) * pageSize));
            result.Count().Should().Be(expectedCount);
            
            if (result.Any())
            {
                result.Should().BeInAscendingOrder(c => c.CustomerCode);
                
                var firstExpectedCode = $"CUST{((page - 1) * pageSize) + 1:D3}";
                result.First().CustomerCode.Should().Be(firstExpectedCode);
            }
        }

        [Theory]
        [InlineData(0, 10, 1, 10)] // Invalid page should default to 1
        [InlineData(-1, 10, 1, 10)]
        [InlineData(1, 0, 1, 50)] // Invalid pageSize should default to 50
        [InlineData(1, -5, 1, 50)]
        [InlineData(1, 1500, 1, 50)] // PageSize over 1000 should default to 50
        public async Task GetCustomersPagedAsync_ShouldUseDefaults_WhenInvalidParametersProvided(int inputPage, int inputPageSize, int expectedPage, int expectedPageSize)
        {
            // Arrange
            var customers = Enumerable.Range(1, 100)
                .Select(i => TestDataBuilder.Customer()
                    .WithCode($"CUST{i:D3}")
                    .Build())
                .ToList();

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetCustomersPagedAsync(inputPage, inputPageSize);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().BeLessOrEqualTo(expectedPageSize);
            
            if (expectedPage == 1)
            {
                result.First().CustomerCode.Should().Be("CUST001");
            }
        }

        [Fact]
        public async Task GetCustomerCountAsync_ShouldReturnCorrectCount_WhenCustomersExist()
        {
            // Arrange
            var expectedCount = 25;
            var customers = Enumerable.Range(1, expectedCount)
                .Select(i => TestDataBuilder.Customer()
                    .WithCode($"CUST{i:D3}")
                    .Build())
                .ToList();

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetCustomerCountAsync();

            // Assert
            result.Should().Be(expectedCount);
        }

        [Fact]
        public async Task GetCustomerCountAsync_ShouldReturnZero_WhenNoCustomersExist()
        {
            // Act
            var result = await _customerService.GetCustomerCountAsync();

            // Assert
            result.Should().Be(0);
        }

        [Theory]
        [InlineData("test", 2)] // Should match "Test Customer 1" and "Test Customer 2"
        [InlineData("CUST001", 1)] // Should match exact customer code
        [InlineData("nonexistent", 0)]
        [InlineData("customer 1", 1)] // Case insensitive
        public async Task SearchCustomersAsync_ShouldReturnMatchingCustomers_WhenSearchTermProvided(string searchTerm, int expectedCount)
        {
            // Arrange
            var customers = new[]
            {
                TestDataBuilder.Customer().WithCode("CUST001").WithName("Test Customer 1").Build(),
                TestDataBuilder.Customer().WithCode("CUST002").WithName("Test Customer 2").Build(),
                TestDataBuilder.Customer().WithCode("CUST003").WithName("Different Customer").Build(),
                TestDataBuilder.Customer().WithCode("ACME001").WithName("ACME Corporation").Build()
            };

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.SearchCustomersAsync(searchTerm);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(expectedCount);
            
            if (result.Any())
            {
                result.Should().BeInAscendingOrder(c => c.CustomerCode);
                result.Should().OnlyContain(c => 
                    c.CustomerCode.ToUpper().Contains(searchTerm.ToUpper()) ||
                    c.CustomerName.ToUpper().Contains(searchTerm.ToUpper()));
            }
        }

        [Theory]
        [InlineData("", 1, 50)] // Empty search term should return paged results
        [InlineData(null, 1, 50)]
        [InlineData("   ", 1, 50)] // Whitespace should return paged results
        public async Task SearchCustomersAsync_ShouldReturnPagedResults_WhenSearchTermIsEmpty(string searchTerm, int expectedPage, int expectedPageSize)
        {
            // Arrange
            var customers = Enumerable.Range(1, 10)
                .Select(i => TestDataBuilder.Customer()
                    .WithCode($"CUST{i:D3}")
                    .Build())
                .ToList();

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.SearchCustomersAsync(searchTerm, expectedPage, expectedPageSize);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(10); // All customers should be returned
            result.Should().BeInAscendingOrder(c => c.CustomerCode);
        }

        [Theory]
        [InlineData("A", 2)] // Should match customers starting with 'A'
        [InlineData("B", 1)]
        [InlineData("0-9", 1)] // Should match customers starting with digits
        [InlineData("Z", 0)] // No customers start with 'Z'
        public async Task GetCustomersByLetterAsync_ShouldReturnMatchingCustomers_WhenLetterProvided(string letter, int expectedCount)
        {
            // Arrange
            var customers = new[]
            {
                TestDataBuilder.Customer().WithCode("ACME001").WithName("ACME Corporation").Build(),
                TestDataBuilder.Customer().WithCode("ALPHA002").WithName("Alpha Industries").Build(),
                TestDataBuilder.Customer().WithCode("BETA003").WithName("Beta Limited").Build(),
                TestDataBuilder.Customer().WithCode("123CORP").WithName("123 Corporation").Build()
            };

            _context.Customers.AddRange(customers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetCustomersByLetterAsync(letter);

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(expectedCount);
            
            if (result.Any())
            {
                result.Should().BeInAscendingOrder(c => c.CustomerCode);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task GetCustomersByLetterAsync_ShouldReturnEmpty_WhenLetterIsNullOrEmpty(string letter)
        {
            // Act
            var result = await _customerService.GetCustomersByLetterAsync(letter);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCustomerByCodeAsync_ShouldReturnCustomer_WhenValidCodeProvided()
        {
            // Arrange
            var customer = TestDataBuilder.Customer()
                .WithCode("TESTCUST")
                .WithName("Test Customer")
                .WithEmail("test@example.com")
                .Build();

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetCustomerByCodeAsync("TESTCUST");

            // Assert
            result.Should().NotBeNull();
            result!.CustomerCode.Should().Be("TESTCUST");
            result.CustomerName.Should().Be("Test Customer");
            result.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task GetCustomerByCodeAsync_ShouldReturnNull_WhenCustomerNotFound()
        {
            // Act
            var result = await _customerService.GetCustomerByCodeAsync("NONEXISTENT");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetInvoicesByCustomerCodeAsync_ShouldReturnCustomerInvoices_WhenInvoicesExist()
        {
            // Arrange
            var customer = TestDataBuilder.Customer().WithCode("CUST001").Build();
            var invoice1 = TestDataBuilder.Invoice()
                .WithNumber("INV001")
                .WithCustomerCode("CUST001")
                .WithCreateDate(DateTime.UtcNow.AddDays(-1))
                .Build();
            var invoice2 = TestDataBuilder.Invoice()
                .WithNumber("INV002")
                .WithCustomerCode("CUST001")
                .WithCreateDate(DateTime.UtcNow.AddDays(-2))
                .Build();
            var otherCustomerInvoice = TestDataBuilder.Invoice()
                .WithNumber("INV003")
                .WithCustomerCode("CUST002")
                .Build();

            _context.Customers.Add(customer);
            _context.Invoices.AddRange(invoice1, invoice2, otherCustomerInvoice);
            await _context.SaveChangesAsync();

            // Act
            var result = await _customerService.GetInvoicesByCustomerCodeAsync("CUST001");

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().OnlyContain(i => i.CustomerCode == "CUST001");
            result.Should().BeInDescendingOrder(i => i.InvoiceCreateDate, "Invoices should be ordered by creation date descending");
            result.First().InvoiceNumber.Should().Be("INV001"); // Most recent first
        }

        [Fact]
        public async Task GetInvoicesByCustomerCodeAsync_ShouldReturnEmpty_WhenNoInvoicesExist()
        {
            // Act
            var result = await _customerService.GetInvoicesByCustomerCodeAsync("NONEXISTENT");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldCreateAndReturnCustomer_WhenValidCustomerProvided()
        {
            // Arrange
            var newCustomer = TestDataBuilder.Customer()
                .WithCode("NEWCUST")
                .WithName("New Customer")
                .WithEmail("new@customer.com")
                .Build();

            // Act
            var result = await _customerService.CreateCustomerAsync(newCustomer);

            // Assert
            result.Should().NotBeNull();
            result!.CustomerCode.Should().Be("NEWCUST");
            result.CustomerName.Should().Be("New Customer");
            result.Email.Should().Be("new@customer.com");

            // Verify customer was actually saved to database
            var savedCustomer = await _context.Customers.FindAsync("NEWCUST");
            savedCustomer.Should().NotBeNull();
            savedCustomer!.CustomerName.Should().Be("New Customer");
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldUpdateAndReturnCustomer_WhenValidCustomerProvided()
        {
            // Arrange
            var existingCustomer = TestDataBuilder.Customer()
                .WithCode("EXISTING")
                .WithName("Original Name")
                .WithEmail("original@email.com")
                .Build();

            _context.Customers.Add(existingCustomer);
            await _context.SaveChangesAsync();

            var updatedCustomer = TestDataBuilder.Customer()
                .WithCode("EXISTING")
                .WithName("Updated Name")
                .WithEmail("updated@email.com")
                .Build();

            // Act
            var result = await _customerService.UpdateCustomerAsync(updatedCustomer);

            // Assert
            result.Should().NotBeNull();
            result!.CustomerCode.Should().Be("EXISTING");
            result.CustomerName.Should().Be("Updated Name");
            result.Email.Should().Be("updated@email.com");

            // Verify customer was actually updated in database
            var savedCustomer = await _context.Customers.FindAsync("EXISTING");
            savedCustomer.Should().NotBeNull();
            savedCustomer!.CustomerName.Should().Be("Updated Name");
            savedCustomer.Email.Should().Be("updated@email.com");
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var nonExistentCustomer = TestDataBuilder.Customer()
                .WithCode("NONEXISTENT")
                .Build();

            // Act
            var result = await _customerService.UpdateCustomerAsync(nonExistentCustomer);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldReturnTrueAndDeleteCustomer_WhenCustomerExists()
        {
            // Arrange
            var customer = TestDataBuilder.Customer()
                .WithCode("DELETEME")
                .WithName("Customer to Delete")
                .Build();

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Verify customer exists before deletion
            var existingCustomer = await _context.Customers.FindAsync("DELETEME");
            existingCustomer.Should().NotBeNull();

            // Act
            var result = await _customerService.DeleteCustomerAsync("DELETEME");

            // Assert
            result.Should().BeTrue();

            // Verify customer was actually deleted from database
            var deletedCustomer = await _context.Customers.FindAsync("DELETEME");
            deletedCustomer.Should().BeNull();
        }

        [Fact]
        public async Task DeleteCustomerAsync_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            // Act
            var result = await _customerService.DeleteCustomerAsync("NONEXISTENT");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetCustomersPagedAsync_ShouldLogError_WhenDatabaseThrowsException()
        {
            // Arrange
            _context.Dispose(); // Force database errors

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => 
                _customerService.GetCustomersPagedAsync(1, 10));

            // Verify error was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving customers page")),
                    It.IsAny<ObjectDisposedException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SearchCustomersAsync_ShouldLogError_WhenDatabaseThrowsException()
        {
            // Arrange
            _context.Dispose(); // Force database errors

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => 
                _customerService.SearchCustomersAsync("test"));

            // Verify error was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error searching customers")),
                    It.IsAny<ObjectDisposedException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldLogError_WhenDatabaseThrowsException()
        {
            // Arrange
            var customer = TestDataBuilder.Customer().WithCode("TEST").Build();
            _context.Dispose(); // Force database errors

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => 
                _customerService.CreateCustomerAsync(customer));

            // Verify error was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error creating customer")),
                    It.IsAny<ObjectDisposedException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}