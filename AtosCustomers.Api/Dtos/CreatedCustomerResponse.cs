using System.Text.Json.Serialization;

namespace AtosCustomers.Api.Dtos;

public class CreatedCustomerResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }
    
    [JsonPropertyName("firstName")]
    public string FirstName { get; init; }
    
    [JsonPropertyName("surname")]
    public string Surname { get; init; }
}