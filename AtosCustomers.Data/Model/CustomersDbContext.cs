using Microsoft.EntityFrameworkCore;

namespace AtosCustomers.Data.Model;

public class CustomersDbContext(DbContextOptions<CustomersDbContext> options): DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
}