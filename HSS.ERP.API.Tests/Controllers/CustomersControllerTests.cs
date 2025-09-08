using HSS.ERP.API.Controllers.Api;
using HSS.ERP.API.Models;
using HSS.ERP.API.Services;
using HSS.ERP.API.Tests.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HSS.ERP.API.Tests.Controllers
{
    /// <summary>
    /// Unit tests for CustomersController functionality.
    /// Tests cover all HTTP endpoints, response codes, and error handling scenarios.
    /// </summary>
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<ILogger<CustomersController>> _loggerMock;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _loggerMock = new Mock<ILogger<CustomersController>>();
            _controller = new CustomersController(_customerServiceMock.Object, _loggerMock.Object);

            // Set up HttpContext for response headers
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        #region GetCustomers Tests

        [Fact]
        public async Task GetCustomers_ShouldReturnOkWithPaginationHeaders_WhenCustomersExist()
        {
            // Arrange
            var customers = new[]
            {
                TestDataBuilder.Customer().WithCode("CUST001").Build(),
                TestDataBuilder.Customer().WithCode("CUST002").Build()
            };
            var totalCount = 25;
            var page = 1;
            var pageSize = 10;

            _customerServiceMock.Setup(s => s.GetCustomersPagedAsync(page, pageSize))
                .ReturnsAsync(customers);
            _customerServiceMock.Setup(s => s.GetCustomerCountAsync())
                .ReturnsAsync(totalCount);

            // Act
            var result = await _controller.GetCustomers(page, pageSize);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<Customer>>().Subject;
            returnedCustomers.Should().HaveCount(2);

            // Verify pagination headers
            _controller.Response.Headers["X-Total-Count"].Should().Equal(totalCount.ToString());
            _controller.Response.Headers["X-Page"].Should().Equal(page.ToString());
            _controller.Response.Headers["X-Page-Size"].Should().Equal(pageSize.ToString());
            _controller.Response.Headers["X-Total-Pages"].Should().Equal("3"); // Math.Ceiling(25/10) = 3
        }

        [Theory]
        [InlineData(1, 50)] // Default values
        [InlineData(2, 25)] // Custom values
        public async Task GetCustomers_ShouldUseProvidedParameters_WhenCalled(int page, int pageSize)
        {
            // Arrange
            var customers = new[] { TestDataBuilder.Customer().Build() };
            _customerServiceMock.Setup(s => s.GetCustomersPagedAsync(page, pageSize))
                .ReturnsAsync(customers);
            _customerServiceMock.Setup(s => s.GetCustomerCountAsync())
                .ReturnsAsync(10);

            // Act
            await _controller.GetCustomers(page, pageSize);

            // Assert
            _customerServiceMock.Verify(s => s.GetCustomersPagedAsync(page, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            _customerServiceMock.Setup(s => s.GetCustomersPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
            statusCodeResult.Value.Should().Be("Internal server error occurred while retrieving customers");

            // Verify error was logged
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error retrieving customers")),
                    It.IsAny<InvalidOperationException>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        #endregion

        #region GetAllCustomers Tests

        [Fact]
        public async Task GetAllCustomers_ShouldReturnOkWithCustomers_WhenCalled()
        {
            // Arrange
            var customers = new[]
            {
                TestDataBuilder.Customer().WithCode("CUST001").Build(),
                TestDataBuilder.Customer().WithCode("CUST002").Build()
            };

            _customerServiceMock.Setup(s => s.GetAllCustomersAsync())
                .ReturnsAsync(customers);

            // Act
            var result = await _controller.GetAllCustomers();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<Customer>>().Subject;
            returnedCustomers.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllCustomers_ShouldLogWarning_WhenCalled()
        {
            // Arrange
            _customerServiceMock.Setup(s => s.GetAllCustomersAsync())
                .ReturnsAsync(new List<Customer>());

            // Act
            await _controller.GetAllCustomers();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("GetAllCustomers called")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllCustomers_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            _customerServiceMock.Setup(s => s.GetAllCustomersAsync())
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.GetAllCustomers();

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        #endregion

        #region GetCustomer Tests

        [Fact]
        public async Task GetCustomer_ShouldReturnOkWithCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerCode = "CUST001";
            var customer = TestDataBuilder.Customer()
                .WithCode(customerCode)
                .WithName("Test Customer")
                .Build();

            _customerServiceMock.Setup(s => s.GetCustomerByCodeAsync(customerCode))
                .ReturnsAsync(customer);

            // Act
            var result = await _controller.GetCustomer(customerCode);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCustomer = okResult.Value.Should().BeOfType<Customer>().Subject;
            returnedCustomer.CustomerCode.Should().Be(customerCode);
            returnedCustomer.CustomerName.Should().Be("Test Customer");
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerCode = "NONEXISTENT";
            _customerServiceMock.Setup(s => s.GetCustomerByCodeAsync(customerCode))
                .ReturnsAsync((Customer?)null);

            // Act
            var result = await _controller.GetCustomer(customerCode);

            // Assert
            var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be($"Customer with code {customerCode} not found");
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var customerCode = "CUST001";
            _customerServiceMock.Setup(s => s.GetCustomerByCodeAsync(customerCode))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.GetCustomer(customerCode);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        #endregion

        #region CreateCustomer Tests

        [Fact]
        public async Task CreateCustomer_ShouldReturnCreatedAtAction_WhenCustomerCreatedSuccessfully()
        {
            // Arrange
            var newCustomer = TestDataBuilder.Customer()
                .WithCode("NEWCUST")
                .WithName("New Customer")
                .Build();

            _customerServiceMock.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync(newCustomer);

            // Act
            var result = await _controller.CreateCustomer(newCustomer);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetCustomer));
            createdResult.RouteValues!["code"].Should().Be("NEWCUST");
            
            var returnedCustomer = createdResult.Value.Should().BeOfType<Customer>().Subject;
            returnedCustomer.CustomerCode.Should().Be("NEWCUST");
        }

        [Fact]
        public async Task CreateCustomer_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var customer = TestDataBuilder.Customer().Build();
            _customerServiceMock.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.CreateCustomer(customer);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        #endregion

        #region UpdateCustomer Tests

        [Fact]
        public async Task UpdateCustomer_ShouldReturnNoContent_WhenCustomerUpdatedSuccessfully()
        {
            // Arrange
            var customerCode = "CUST001";
            var customer = TestDataBuilder.Customer()
                .WithCode(customerCode)
                .WithName("Updated Customer")
                .Build();

            _customerServiceMock.Setup(s => s.UpdateCustomerAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCustomer(customerCode, customer);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateCustomer_ShouldReturnBadRequest_WhenCustomerCodeMismatch()
        {
            // Arrange
            var urlCode = "CUST001";
            var customer = TestDataBuilder.Customer()
                .WithCode("CUST002") // Different from URL
                .Build();

            // Act
            var result = await _controller.UpdateCustomer(urlCode, customer);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Customer code mismatch");
        }

        [Fact]
        public async Task UpdateCustomer_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var customerCode = "CUST001";
            var customer = TestDataBuilder.Customer().WithCode(customerCode).Build();
            
            _customerServiceMock.Setup(s => s.UpdateCustomerAsync(It.IsAny<Customer>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.UpdateCustomer(customerCode, customer);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        #endregion

        #region DeleteCustomer Tests

        [Fact]
        public async Task DeleteCustomer_ShouldReturnNoContent_WhenCustomerDeletedSuccessfully()
        {
            // Arrange
            var customerCode = "CUST001";
            _customerServiceMock.Setup(s => s.DeleteCustomerAsync(customerCode))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteCustomer(customerCode);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var customerCode = "CUST001";
            _customerServiceMock.Setup(s => s.DeleteCustomerAsync(customerCode))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.DeleteCustomer(customerCode);

            // Assert
            var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        #endregion

        #region SearchCustomers Tests

        [Fact]
        public async Task SearchCustomers_ShouldReturnOkWithCustomers_WhenValidQueryProvided()
        {
            // Arrange
            var query = "test";
            var customers = new[]
            {
                TestDataBuilder.Customer().WithCode("CUST001").WithName("Test Customer 1").Build(),
                TestDataBuilder.Customer().WithCode("CUST002").WithName("Test Customer 2").Build()
            };

            _customerServiceMock.Setup(s => s.SearchCustomersAsync(query))
                .ReturnsAsync(customers);

            // Act
            var result = await _controller.SearchCustomers(query);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCustomers = okResult.Value.Should().BeAssignableTo<IEnumerable<Customer>>().Subject;
            returnedCustomers.Should().HaveCount(2);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task SearchCustomers_ShouldReturnBadRequest_WhenQueryIsNullOrEmpty(string query)
        {
            // Act
            var result = await _controller.SearchCustomers(query);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Search query cannot be empty");
        }

        [Fact]
        public async Task SearchCustomers_ShouldReturnInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var query = "test";
            _customerServiceMock.Setup(s => s.SearchCustomersAsync(query))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _controller.SearchCustomers(query);

            // Assert
            var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        #endregion

        #region Service Verification Tests

        [Fact]
        public async Task GetCustomers_ShouldCallServiceMethods_InCorrectOrder()
        {
            // Arrange
            var customers = new[] { TestDataBuilder.Customer().Build() };
            _customerServiceMock.Setup(s => s.GetCustomersPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(customers);
            _customerServiceMock.Setup(s => s.GetCustomerCountAsync())
                .ReturnsAsync(10);

            // Act
            await _controller.GetCustomers();

            // Assert
            _customerServiceMock.Verify(s => s.GetCustomersPagedAsync(1, 50), Times.Once);
            _customerServiceMock.Verify(s => s.GetCustomerCountAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCustomer_ShouldPassCustomerToService()
        {
            // Arrange
            var customer = TestDataBuilder.Customer()
                .WithCode("TEST")
                .WithName("Test Customer")
                .Build();

            _customerServiceMock.Setup(s => s.CreateCustomerAsync(It.IsAny<Customer>()))
                .ReturnsAsync(customer);

            // Act
            await _controller.CreateCustomer(customer);

            // Assert
            _customerServiceMock.Verify(s => s.CreateCustomerAsync(
                It.Is<Customer>(c => c.CustomerCode == "TEST" && c.CustomerName == "Test Customer")), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateCustomer_ShouldPassCustomerToService()
        {
            // Arrange
            var customer = TestDataBuilder.Customer()
                .WithCode("TEST")
                .WithName("Updated Customer")
                .Build();

            _customerServiceMock.Setup(s => s.UpdateCustomerAsync(It.IsAny<Customer>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.UpdateCustomer("TEST", customer);

            // Assert
            _customerServiceMock.Verify(s => s.UpdateCustomerAsync(
                It.Is<Customer>(c => c.CustomerCode == "TEST" && c.CustomerName == "Updated Customer")), 
                Times.Once);
        }

        [Fact]
        public async Task SearchCustomers_ShouldPassQueryToService()
        {
            // Arrange
            var query = "search term";
            _customerServiceMock.Setup(s => s.SearchCustomersAsync(query))
                .ReturnsAsync(new List<Customer>());

            // Act
            await _controller.SearchCustomers(query);

            // Assert
            _customerServiceMock.Verify(s => s.SearchCustomersAsync(query), Times.Once);
        }

        #endregion
    }
}