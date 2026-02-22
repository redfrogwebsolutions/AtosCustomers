using System.Text.Json.Serialization;

namespace AtosCustomers.Api.Dtos;

public class CreateCustomerRequest
{
    [JsonPropertyName("firstName")]
    public string FirstName { get; init; }
    
    [JsonPropertyName("surname")]
    public string Surname { get; init; }
}