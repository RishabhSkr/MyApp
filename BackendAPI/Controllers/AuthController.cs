using BackendAPI.Data;
using BackendAPI.Dtos.Auth;
using BackendAPI.Helpers;
using BackendAPI.Models;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwt;

    public AuthController(AppDbContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    // register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var role = await _context.Roles.FindAsync(dto.RoleId);
        if (role == null) return BadRequest("Invalid role");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            RoleId = dto.RoleId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered");
    }

    // login
    // TODO: jwt validation tokens and validators
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
            return Unauthorized();

        if (string.IsNullOrEmpty(user.PasswordHash) || !PasswordHelper.Verify(user.PasswordHash, dto.Password))
            return Unauthorized();
        try
        {
            /* 
                { "error": "Token Generation Failed", "details": "IdX10653: The encryption algorithm 'HS256' requires a key size of at least '128' bits.
                Key '[PII of type 'Microsoft.IdentityModel.Tokens.SymmetricSecurityKey' is hidden. 
                For more details, see https://aka.ms/IdentityModel/PII.]', is of size: '120'. (Parameter 'key')"}
            */
            var token = _jwt.GenerateToken(user);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Token Generation Failed", Details = ex.Message });
        }
    }
}
