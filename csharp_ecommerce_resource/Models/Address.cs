namespace csharp_ecommerce_resource.Models;

public class Address
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? Country { get; set; }
    public string? StreetAddress { get; set; }
    public string? StreetAddress2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public bool IsDefault { get; set; }
}