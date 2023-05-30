using Microsoft.AspNetCore.Mvc;
using Services.UserService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LoginService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{


    private readonly ILogger<LoginController> _logger;
    private userService _service;
    private readonly VaultSecrets _secrets;

    public UserController(ILogger<LoginController> logger, userService service,VaultSecrets secrets)
    {
        _logger = logger;
        _service = service;
        var vaultSecrets = new VaultSecrets
        {
            vaultSecret = secrets.vaultSecret,
            vaultIssuer = secrets.vaultIssuer

        };
        _secrets = vaultSecrets;
    }

[AllowAnonymous]
[HttpPost("postUser")]
public async Task<IActionResult> postUser([FromBody] LoginModel login)
{
if(login.Role.Count() < 1  | login.Password.Count() < 1| login.Username.Count() < 1)
{
    return BadRequest("all fields must be filled out to create a new user");
}
_service.CreateAsync(login);
return Ok($"user created");
}
[Authorize(Roles ="admin")]
[HttpDelete("deleteUser")]
public async Task<IActionResult> deleteUser(string id)
{
    await _service.RemoveAsync(id);
return Ok($"user {id} was deleted");
}
[Authorize(Roles ="admin")]
[HttpGet("getUsers")]
public List<LoginModel> getAllUsers()
{
 return _service.getAllUsers();

}
  

}
