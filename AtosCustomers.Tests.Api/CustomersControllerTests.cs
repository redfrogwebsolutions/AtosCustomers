using AtosCustomers.Api.Controllers;
using AtosCustomers.Api.Dtos;
using AtosCustomers.Data.Model;
using AtosCustomers.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AtosCustomers.Tests.Api
{
    public class CustomersControllerTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsNotFound_WhenNoCustomers()
        {
            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Customer>());

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var result = await controller.GetAllAsync();

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOk_WithMappedCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), FirstName = "Alice", Surname = "Smith" },
                new Customer { Id = Guid.NewGuid(), FirstName = "Bob", Surname = "Jones" }
            };

            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var result = await controller.GetAllAsync();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<CustomerResponse>>(ok.Value);
            var list = value.ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(list, c => c.FirstName == "Alice" && c.Surname == "Smith");
            Assert.Contains(list, c => c.FirstName == "Bob" && c.Surname == "Jones");
        }

        [Fact]
        public async Task AddCustomer_ReturnsBadRequest_WhenMissingNames()
        {
            var repoMock = new Mock<ICustomerRepository>();
            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var invalid = new CreateCustomerRequest { FirstName = "", Surname = "Valid" };
            var res1 = await controller.AddCustomer(invalid);
            Assert.IsType<BadRequestObjectResult>(res1.Result);

            var invalid2 = new CreateCustomerRequest { FirstName = "Valid", Surname = "" };
            var res2 = await controller.AddCustomer(invalid2);
            Assert.IsType<BadRequestObjectResult>(res2.Result);
        }

        [Fact]
        public async Task AddCustomer_Returns201Created_WhenValid()
        {
            var id = Guid.NewGuid();
            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .ReturnsAsync((Customer c) =>
                {
                    c.Id = id;
                    return c;
                });

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var request = new CreateCustomerRequest { FirstName = "New", Surname = "Customer" };
            var result = await controller.AddCustomer(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);

            var created = Assert.IsType<CreatedCustomerResponse>(objectResult.Value);
            Assert.Equal(id, created.Id);
            Assert.Equal("New", created.FirstName);
            Assert.Equal("Customer", created.Surname);
        }
        
        [Fact]
        public async Task AddCustomer_Valid_LogsInformation()
        {
            var id = Guid.NewGuid();
            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<Customer>()))
                .ReturnsAsync((Customer c) =>
                {
                    c.Id = id;
                    return c;
                });

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var request = new CreateCustomerRequest { FirstName = "New", Surname = "Customer" };
            var result = await controller.AddCustomer(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);

            loggerMock.Verify(
                l => l.Log(
                    It.Is<LogLevel>(ll => ll == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenCustomerMissing()
        {
            var id = Guid.NewGuid();
            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Customer?)null);

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var result = await controller.DeleteAsync(id);

            Assert.IsType<NotFoundResult>(result);
            repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenCustomerExists()
        {
            var id = Guid.NewGuid();
            var existing = new Customer { Id = id, FirstName = "X", Surname = "Y" };

            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var result = await controller.DeleteAsync(id);

            Assert.IsType<NoContentResult>(result);
        }
        
        [Fact]
        public async Task DeleteAsync_Existing_LogsInformation()
        {
            var id = Guid.NewGuid();
            var existing = new Customer { Id = id, FirstName = "X", Surname = "Y" };

            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var result = await controller.DeleteAsync(id);

            Assert.IsType<NoContentResult>(result);

            loggerMock.Verify(
                l => l.Log(
                    It.Is<LogLevel>(ll => ll == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }
        
         [Fact]
        public async Task AddCustomer_RepositoryThrows_LogsErrorAndReturns500()
        {
            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.AddAsync(It.IsAny<Customer>())).ThrowsAsync(new Exception("add-failed"));

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var request = new CreateCustomerRequest { FirstName = "New", Surname = "Customer" };
            var result = await controller.AddCustomer(request);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            loggerMock.Verify(
                l => l.Log(
                    It.Is<LogLevel>(ll => ll == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task DeleteAsync_DeleteThrows_LogsErrorAndReturns500()
        {
            var id = Guid.NewGuid();
            var existing = new Customer { Id = id, FirstName = "X", Surname = "Y" };

            var repoMock = new Mock<ICustomerRepository>();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            repoMock.Setup(r => r.DeleteAsync(id)).ThrowsAsync(new Exception("delete-failed"));

            var loggerMock = new Mock<ILogger<CustomersController>>();
            var controller = new CustomersController(loggerMock.Object, repoMock.Object);

            var result = await controller.DeleteAsync(id);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            loggerMock.Verify(
                l => l.Log(
                    It.Is<LogLevel>(ll => ll == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);

            repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
