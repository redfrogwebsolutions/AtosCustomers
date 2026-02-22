using AtosCustomers.Api.Dtos;
using AtosCustomers.Data.Model;
using AtosCustomers.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtosCustomers.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ILogger<CustomersController> logger, ICustomerRepository customerRepository) : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger = logger;

        public async Task<string> Get()
        {
            return await Task.FromResult("Hello from CustomersController");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAllAsync()
        {
            var customers = await customerRepository.GetAllAsync();
            
            //we could use AutoMapper but in this simple task I'm going to keep it simple
            return Ok(customers.Select(c=> new CustomerResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                SurnameEmail = c.Surname
            }));
        }

        [HttpPost]
        public async Task<ActionResult<CreatedCustomerResponse>> AddCustomer(CreateCustomerRequest customer)
        {
            try
            {
                var addedCustomer = await customerRepository.AddAsync(new Customer
                    {
                        FirstName = customer.FirstName,
                        Surname = customer.SurnameEmail
                    }
                );
                //I will return 'CreatedAtAction' but then I will need add GetById what is not on the controllers list 
                //in the instruction
                return StatusCode(StatusCodes.Status201Created,new CreatedCustomerResponse
                {
                    Id = addedCustomer.Id,
                    FirstName = addedCustomer.FirstName,
                    SurnameEmail = addedCustomer.Surname
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        

    }

}
       