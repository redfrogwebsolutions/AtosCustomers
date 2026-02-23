using System.Text.Json.Serialization;

namespace AtosCustomers.Api.Dtos;

public class CreateCustomerRequest
{
    [JsonPropertyName("firstName")]
    public required string FirstName { get; init; }
    
    [JsonPropertyName("surname")]
    public required string Surname { get; init; }
}