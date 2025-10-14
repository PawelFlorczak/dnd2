using DiceAPI.Data;
using DiceAPI.Models;
using DiceAPI.Models.DTOs;
using DiceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Tags("Authentication")]
public class AuthController : ControllerBase
{
    private readonly DiceContext _context;

    public AuthController(DiceContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(AuthResponse), 400)]
    [ProducesResponseType(typeof(AuthResponse), 409)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Password) || 
            string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new AuthResponse 
            { 
                Success = false, 
                Message = "Username, password, and email are required." 
            });
        }

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

        if (existingUser != null)
        {
            return Conflict(new AuthResponse 
            { 
                Success = false, 
                Message = "Username or email already exists." 
            });
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = PasswordService.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email
        };

        return Ok(new AuthResponse 
        { 
            Success = true, 
            Message = "User registered successfully.", 
            User = userDto 
        });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(AuthResponse), 400)]
    [ProducesResponseType(typeof(AuthResponse), 401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new AuthResponse 
            { 
                Success = false, 
                Message = "Username and password are required." 
            });
        }

        var user = await _context.Users
            .Include(u => u.Characters)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null || !PasswordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new AuthResponse 
            { 
                Success = false, 
                Message = "Invalid username or password." 
            });
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Characters = user.Characters.Select(c => new CharacterSummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                Species = c.Species,
                Career = c.Career,
                CurrentWounds = c.CurrentWounds,
                Wounds = c.Wounds
            }).ToList()
        };

        return Ok(new AuthResponse 
        { 
            Success = true, 
            Message = "Login successful.", 
            User = userDto 
        });
    }
}