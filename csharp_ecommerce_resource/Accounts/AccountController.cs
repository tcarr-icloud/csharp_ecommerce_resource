using Microsoft.AspNetCore.Mvc;

namespace csharp_ecommerce_resource.Accounts;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost]
    public ActionResult<AccountDto> AddAccount([FromBody] AccountDto accountDto)
    {
        accountService.CreateAccount(accountDto);
        var resourceUri = Url.Action(nameof(GetAccount), "Account", new { id = accountDto.Id });
        return Created(resourceUri, accountDto);
    }

    [HttpGet("{id}")]
    public ActionResult<AccountDto> GetAccount(string id)
    {
        var accountDto = accountService.GetAccount(id);
        return Ok(accountDto);
    }

    [HttpGet]
    public ActionResult<List<AccountDto>> GetAllAccounts()
    {
        return Ok(new List<AccountDto>());
    }

    [HttpPut("{id}")]
    public ActionResult<AccountDto> UpdateAccount(string id, [FromBody] AccountDto accountDto)
    {
        accountService.UpdateAccount(id, accountDto);
        return Ok(accountDto);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteAccount(string id)
    {
        accountService.DeleteAccount(id);
        return Ok();
    }
}