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
public class LoginController : ControllerBase
{


    private readonly ILogger<LoginController> _logger;
    private userService _service;
    private readonly VaultSecrets _secrets;

    public LoginController(ILogger<LoginController> logger, userService service,VaultSecrets secrets)
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
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        if (_service.Login(login) == null)
        {
            return Unauthorized();
        }
        var token = GenerateJwtToken(_service.Login(login));
        Console.WriteLine($"{_secrets.vaultSecret}");



        return Ok(new { token });
    }
     [AllowAnonymous]
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateJwtToken([FromBody] string? token)
    {
        Console.WriteLine($"{token}");
        if (token.IsNullOrEmpty())

            return BadRequest("Invalid token submited.");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secrets.vaultSecret!);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var Role = jwtToken.Claims.First(
            x => x.Type == ClaimTypes.Role).Value;
            return Ok($"You're logged in with the current role:{Role}");
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, ex.Message);
            return StatusCode(404);
        }
    }
[Authorize(Roles ="user")]
[HttpGet("testUser")]
public async Task<IActionResult> Get()
{
return Ok("You're authorized");
}
[Authorize(Roles ="admin")]
[HttpGet("testAdmin")]
public async Task<IActionResult> Get1()
{
return Ok("You're authorized");
}
string GenerateJwtToken(LoginModel user)
    {
var securityKey =
        new
        SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secrets.vaultSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {new Claim(ClaimTypes.Role, user.Role)};
        var token = new JwtSecurityToken(
        _secrets.vaultIssuer,
        "http://localhost",
        claims,
        expires: DateTime.Now.AddMinutes(30),
        signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
