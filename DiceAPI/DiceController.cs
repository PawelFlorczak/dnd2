using DiceAPI.Data;
using DiceAPI.Models;
using DiceAPI.Models.DTOs;
using DiceAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace DiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Tags("Dice Rolling")]
public class DiceController : ControllerBase
{
    private readonly DiceContext _context;
    private readonly IHubContext<DiceHub> _hubContext;

    public DiceController(DiceContext context, IHubContext<DiceHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet("roll")]
    [ProducesResponseType(typeof(DiceRoll), 200)]
    public async Task<IActionResult> Roll([FromQuery] int sides = 20, [FromQuery] string player = "unknown", [FromQuery] int? userId = null)
    {
        var result = Random.Shared.Next(1, sides + 1);
        
        // If userId provided, get the character name or username
        string playerName = player;
        if (userId.HasValue)
        {
            var user = await _context.Users.FindAsync(userId.Value);
            if (user != null)
            {
                playerName = user.Username;
            }
        }
        
        var roll = new DiceRoll
        {
            PlayerName = playerName,
            Sides = sides,
            Result = result,
            Timestamp = DateTime.UtcNow
        };

        _context.DiceRolls.Add(roll);
        await _context.SaveChangesAsync();
        
        await _hubContext.Clients.All.SendAsync("OnRollReceived", roll);

        return Ok(roll);
    }

    [HttpPost("character-roll")]
    [ProducesResponseType(typeof(CharacterRollResult), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CharacterRoll([FromBody] CharacterRollRequest request)
    {
        var character = await _context.Characters
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == request.CharacterId);
            
        if (character == null)
        {
            return NotFound("Character not found.");
        }

        var characteristic = GetCharacteristic(character, request.Characteristic);
        var skillAdvances = request.SkillAdvances ?? 0;
        var totalValue = characteristic + skillAdvances + request.Modifier;
        
        var diceResult = Random.Shared.Next(1, 101); // d100 roll
        var success = diceResult <= totalValue;
        
        var roll = new DiceRoll
        {
            PlayerName = $"{character.User.Username} ({character.Name})",
            Sides = 100,
            Result = diceResult,
            Timestamp = DateTime.UtcNow
        };

        _context.DiceRolls.Add(roll);
        await _context.SaveChangesAsync();
        
        var rollResult = new CharacterRollResult
        {
            Roll = roll,
            TargetNumber = totalValue,
            Success = success,
            CharacterName = character.Name,
            TestName = request.TestName ?? request.Characteristic
        };
        
        await _hubContext.Clients.All.SendAsync("OnCharacterRollReceived", rollResult);

        return Ok(rollResult);
    }

    private int GetCharacteristic(Character character, string characteristic)
    {
        return characteristic.ToUpper() switch
        {
            "WS" => character.WeaponSkill,
            "BS" => character.BallisticSkill,
            "S" => character.Strength,
            "T" => character.Toughness,
            "I" => character.Initiative,
            "AG" => character.Agility,
            "DEX" => character.Dexterity,
            "INT" => character.Intelligence,
            "WP" => character.Willpower,
            "FEL" => character.Fellowship,
            _ => 0
        };
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] string? player = null)
    {
        var query = _context.DiceRolls.AsQueryable();

        if (!string.IsNullOrWhiteSpace(player))
        {
            query = query.Where(r => r.PlayerName == player);
        }

        var rolls = await query
            .OrderByDescending(r => r.Timestamp)
            .Take(50)
            .ToListAsync();

        return Ok(rolls);
    }

    

}