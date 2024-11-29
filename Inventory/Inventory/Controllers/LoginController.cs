using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inventory.Repository.Services;
using Inventory.Models;
using Microsoft.CodeAnalysis.CSharp;
namespace Inventory.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly AuthService _authService;
    public LoginController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> UserAuthentication([FromBody] LoginDTO login_credentials)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (string.IsNullOrEmpty(login_credentials.username) || string.IsNullOrEmpty(login_credentials.password))
        {
            return BadRequest("Invalid login request.");
        }
        try
        {
            Employees? emp = await _authService.authetication(login_credentials.username, login_credentials.password);
            var token = _authService.Generate_Token(emp);
            return Ok(new { Token = token });
        }   
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
