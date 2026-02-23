using System.Text.Json.Serialization;

namespace AtosCustomers.Api.Dtos;

public class CustomerResponse
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("firstName")]
    public required string FirstName { get; init; }
    
    [JsonPropertyName("surname")]
    public required string Surname { get; init; }
}