using Microsoft.AspNetCore.Mvc;

namespace csharp_ecommerce_resource.Carts;

[ApiController]
[Route("api/[controller]")]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpPost]
    public ActionResult<CartDto> AddCart(CartDto cartDto)
    {
        try
        {
            cartService.Create(cartDto);
            var resourceUri = Url.Action(nameof(GetCart), "Cart", new { id = cartDto.Id });
            return Created(resourceUri, cartDto);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<CartDto> GetCart(string id)
    {
        var cartDto = cartService.Get(id);
        return Ok(cartDto);
    }

    [HttpGet]
    public ActionResult<List<CartDto>> GetAllCarts()
    {
        var carts = cartService.GetAll();
        return Ok(carts);
    }

    [HttpPut("{id}")]
    public ActionResult<CartDto> UpdateCart(string id, CartDto cartDto)
    {
        cartService.Update(id, cartDto);
        return Ok(cartDto);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCart(string id)
    {
        cartService.Delete(id);
        return Ok();
    }
}