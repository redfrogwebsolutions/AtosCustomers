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
        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAllAsync()
        {
            try
            {

                var customers = await customerRepository.GetAllAsync();

                if (customers.Any() is false)
                {
                    return NotFound();
                }

                //we could use AutoMapper but in this simple task I'm going to keep it simple
                return Ok(customers.Select(c => new CustomerResponse
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    Surname = c.Surname
                }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("AddCustomer")]
        public async Task<ActionResult<CreatedCustomerResponse>> AddCustomer(CreateCustomerRequest customer)
        {
            try
            {
                if (string.IsNullOrEmpty(customer.FirstName) || string.IsNullOrEmpty(customer.Surname))
                {
                    return BadRequest("First name and Surname are required");
                }
                
                var addedCustomer = await customerRepository.AddAsync(new Customer
                    {
                        FirstName = customer.FirstName,
                        Surname = customer.Surname
                    }
                );
                
                logger.LogInformation($"Customer created: {addedCustomer.Id}");
                //I will return 'CreatedAtAction' but then I will need add GetById what is not on the controllers list 
                //in the instruction
                return StatusCode(StatusCodes.Status201Created,new CreatedCustomerResponse
                {
                    Id = addedCustomer.Id,
                    FirstName = addedCustomer.FirstName,
                    Surname = addedCustomer.Surname
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteCustomer/{id:guid}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            try
            {
                var customer = await customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }
                
                await customerRepository.DeleteAsync(id);
                
                logger.LogInformation($"Customer deleted: {id}");
                
                return NoContent();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }

}
       