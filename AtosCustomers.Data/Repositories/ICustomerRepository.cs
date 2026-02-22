using AtosCustomers.Data.Model;

namespace AtosCustomers.Data.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer> AddAsync(Customer customer);
    Task DeleteAsync(Guid id);
    Task<Customer?> GetByIdAsync(Guid id);
}