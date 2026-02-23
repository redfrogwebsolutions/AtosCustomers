using AtosCustomers.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace AtosCustomers.Data.Repositories;

public class CustomerRepository(CustomersDbContext dataContext) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        var customer = await dataContext.Customers.FirstOrDefaultAsync(c => c.Id == id);
        return customer;
    }
    
    public async Task<List<Customer>> GetAllAsync()
    {
        return await dataContext.Customers.ToListAsync();
    }
     
    public async Task<Customer> AddAsync(Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer));
        }
        
        if (string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.Surname))
        {
            throw new ArgumentException("Customer must have a first name and surname.");
        }
        
        dataContext.Customers.Add(customer);
        await dataContext.SaveChangesAsync();
        return customer;
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await dataContext.Customers.FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            throw new Exception($"Customer with id {id} not found.");
        }
       
        dataContext.Customers.Remove(customer);
        await dataContext.SaveChangesAsync();
    }
    
}

