namespace csharp_ecommerce_resource.Services;

public interface IProcessor<T>
{
    void HandleMessage(string message);
}