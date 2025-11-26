using Microsoft.AspNetCore.Mvc;

namespace csharp_ecommerce_resource.Carts;

[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCart(CartDto cartDto)
    {
        var cart = await cartService.AddCart(cartDto);
        var resourceUri = Url.Action(nameof(GetCart), "Cart", new { id = cartDto.Id });
        return Created(resourceUri, cart);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCart(string id)
    {
        var cart = await cartService.GetCart(id);
        return Ok(cart);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCarts()
    {
        var carts = await cartService.GetAllCarts();
        return Ok(carts);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCart(string id, CartDto cartDto)
    {
        var updatedCart = await cartService.UpdateCart(id, cartDto);
        return Ok(updatedCart);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCart(string id)
    {
        await cartService.DeleteCart(id);
        return Ok();
    }
}