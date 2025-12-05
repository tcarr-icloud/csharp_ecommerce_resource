using Microsoft.AspNetCore.Mvc;

namespace csharp_ecommerce_resource.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderDto orderDto)
    {
        var createdOrder = orderService.Create(orderDto);
        return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
    }
    
    [HttpGet("{id}")]
    public IActionResult GetOrder(string id)
    {
        var order = orderService.Get(id);
        return Ok(order);
    }
    
    [HttpGet]
    public IActionResult GetAllOrders()
    {
        var orders = orderService.GetAll();
        return Ok(orders);
    }
    
    [HttpDelete("{id}")]
    public IActionResult DeleteOrder(string id)
    {
        orderService.Delete(id);
        return NoContent();
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateOrder(string id, [FromBody] OrderDto orderDto)
    {
        var updatedOrder = orderService.Update(id, orderDto);
        if (updatedOrder == null)
        {
            return NotFound();
        }
        return Ok(updatedOrder);
    }
    
}