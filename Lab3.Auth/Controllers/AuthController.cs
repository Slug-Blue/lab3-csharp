// Lab3.Auth/Controllers/AuthController.cs
using Lab3.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Auth.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    public record RegisterRequest(string Username, string Password);
    public record LoginRequest(string Username, string Password);
    public record ChangePasswordRequest(string Username, string OldPassword, string NewPassword);

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        var result = _auth.Register(req.Username, req.Password);
        return result.Success ? Created("", result) : BadRequest(result);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var result = _auth.Login(req.Username, req.Password);
        return result.Success ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("change-password")]
    public IActionResult ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var result = _auth.ChangePassword(req.Username, req.OldPassword, req.NewPassword);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("users")]
    public IActionResult GetUsers() => Ok(new { Users = _auth.GetUsers() });
}
