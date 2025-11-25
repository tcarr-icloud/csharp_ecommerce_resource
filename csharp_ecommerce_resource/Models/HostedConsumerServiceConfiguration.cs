namespace csharp_ecommerce_resource.Models;

public class HostedConsumerServiceConfiguration
{
    public string? Topic { get; set; }
    public string? BootstrapServers { get; set; }
    public string? GroupId { get; set; }
}