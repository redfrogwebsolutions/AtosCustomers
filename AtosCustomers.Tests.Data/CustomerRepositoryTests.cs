// csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AtosCustomers.Data.Model;
using AtosCustomers.Data.Repositories;

namespace AtosCustomers.Tests.Data
{
    public class CustomerRepositoryTests
    {
        private static CustomersDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<CustomersDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new CustomersDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsCustomer_And_GetAllReturnsIt()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repository = new CustomerRepository(context);

            var toAdd = new Customer { FirstName = "Alice", Surname = "Smith" };
            var added = await repository.AddAsync(toAdd);

            var all = (await repository.GetAllAsync()).ToList();

            Assert.Single(all);
            Assert.Equal("Alice", all[0].FirstName);
            Assert.Equal("Smith", all[0].Surname);
        }
        
        [Fact]
        public async Task AddAsync_NullCustomer_ThrowsArgumentNullException()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repository = new CustomerRepository(context);

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await repository.AddAsync(null!);
            });
        }

        [Fact]
        public async Task AddAsync_EmptyFirstName_ThrowsArgumentException()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repository = new CustomerRepository(context);

            var invalid = new Customer { FirstName = "", Surname = "Valid" };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await repository.AddAsync(invalid);
            });
        }

        [Fact]
        public async Task AddAsync_EmptySurname_ThrowsArgumentException()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repository = new CustomerRepository(context);

            var invalid = new Customer { FirstName = "Valid", Surname = "" };
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await repository.AddAsync(invalid);
            });
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllSeededCustomers()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            context.Customers.AddRange(
                new Customer { FirstName = "A", Surname = "One" },
                new Customer { FirstName = "B", Surname = "Two" }
            );
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            var all = (await repository.GetAllAsync()).ToList();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, c => c.FirstName == "A" && c.Surname == "One");
            Assert.Contains(all, c => c.FirstName == "B" && c.Surname == "Two");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsExpectedCustomer()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var seeded = new Customer { FirstName = "Find", Surname = "Me" };
            context.Customers.Add(seeded);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            var fetched = await repository.GetByIdAsync(seeded.Id);

            Assert.NotNull(fetched);
            Assert.Equal("Find", fetched.FirstName);
            Assert.Equal("Me", fetched.Surname);
        }

        [Fact]
        public async Task DeleteAsync_RemovesCustomer()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var seeded = new Customer { FirstName = "To", Surname = "Delete" };
            context.Customers.Add(seeded);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            await repository.DeleteAsync(seeded.Id);

            var found = await repository.GetByIdAsync(seeded.Id);
            Assert.Null(found);
            var all = (await repository.GetAllAsync()).ToList();
            Assert.Empty(all);
        }
        
        [Fact]
        public async Task DeleteAsync_NonexistentCustomer_ThrowsException()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repository = new CustomerRepository(context);

            var id = Guid.NewGuid();

            var ex = await Assert.ThrowsAsync<Exception>(async () =>
            {
                await repository.DeleteAsync(id);
            });

            Assert.Contains(id.ToString(), ex.Message);
        }
    }
}
