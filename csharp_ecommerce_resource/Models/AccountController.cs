using csharp_ecommerce_resource.Services;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ecommerce_resource.Models;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Account>> GetAllAccounts()
    {
        return Ok(new List<Account>());
    }

    [HttpGet("{id}")]
    public ActionResult<Account> GetAccount(string id)
    {
        return Ok(new Account());
    }

    [HttpPost]
    public ActionResult<Account> AddAccount([FromBody] Account newAccount)
    {
        accountService.AddAccount(newAccount);
        var resourceUri = Url.Action(nameof(GetAccount), "Account", new { id = newAccount.Id });
        return Created(resourceUri, newAccount);
    }
}