using Aluparts.API.Models;
using Aluparts.BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aluparts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.Email, request.Password, request.Role);
        if (result) return Ok("User created Succesfully");

        return BadRequest("Registration Failed.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var token = await _authService.LoginAsync(email, password);

        if (token == null) return Unauthorized("Invalid Credentials");

        return Ok(new { Token = token });
    }

    [HttpPost("mfa/setup")]
    [Authorize] 
    public async Task<IActionResult> SetupMfa()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var qrCodeUrl = await _authService.SetupMfaAsync(email!);
        return Ok(new { QrCodeUrl = qrCodeUrl });
    }

    [HttpPost("mfa/enable")]
    [Authorize]
    public async Task<IActionResult> EnableMfa(string code)
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var result = await _authService.VerifyAndEnableMfaAsync(email!, code);
        return result ? Ok("MFA geactiveerd!") : BadRequest("Ongeldige code.");
    }
}