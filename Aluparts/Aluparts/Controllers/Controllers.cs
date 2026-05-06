using Aluparts.API.DTO_s;
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
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.Success) return Unauthorized("Invalid Credentials");

        if (result.RequiresMfa)
        {
            return Ok(new { RequiresMfa = true, Message = "MFA code needed" });
        }

        return Ok(new { Token = result.Token });
    }

    [HttpPost("mfa/verify-login")]
    public async Task<IActionResult> VerifyLogin([FromBody] VerifyLoginRequest request)
    {
        var token = await _authService.VerifyMfaAndLoginAsync(request.Email, request.Code);

        if (token == null) return Unauthorized("Invalid MFA code.");

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
        return result ? Ok("MFA Activated!") : BadRequest("Invalid code.");
    }



    [HttpGet("status")]
    [Authorize]
    public async Task<IActionResult> GetStatus()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return Unauthorized();

        var isEnabled = await _authService.IsMfaEnabledAsync(email);

        return Ok(new
        {
            isMfaEnabled = isEnabled,
            email = email
        });
    }

}