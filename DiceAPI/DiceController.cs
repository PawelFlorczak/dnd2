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
        var characteristicAdvances = request.CharacteristicAdv;
        var characteristicMod = request.CharacteristicMod;
        var totalValue = characteristic + characteristicAdvances + characteristicMod;
        
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

    /// <summary>
    /// Performs a skill-based dice roll for a character
    /// This endpoint is called by CharacterSheetUI.cs when a skill button is pressed
    /// </summary>
    /// <param name="request">Contains characterId, skill name, and test name</param>
    /// <returns>The result of the skill roll including success/failure</returns>
    [HttpPost("skill-roll")]
    [ProducesResponseType(typeof(SkillRollResult), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> SkillRoll([FromBody] SkillRollRequest request)
    {
        // Step 1: Find the character in the database
        // We include the User to get the username for display
        var character = await _context.Characters
            .Include(c => c.User)
            .Include(c => c.Skills) // Include skills to check if character has this skill trained
            .FirstOrDefaultAsync(c => c.Id == request.CharacterId);
            
        if (character == null)
        {
            return NotFound("Character not found");
        }

        // Step 2: Look up the skill in the character's skill list
        var characterSkill = character.Skills
            .FirstOrDefault(s => s.SkillName.Equals(request.Skill, StringComparison.OrdinalIgnoreCase));
        
        // Step 3: Determine the characteristic and advances for this skill
        string characteristic = "";
        int skillAdvances = 0;
        
        if (characterSkill != null)
        {
            // Character has this skill trained - use their specific data
            characteristic = characterSkill.Characteristic;
            skillAdvances = characterSkill.Advances;
        }
        else
        {
            // Character doesn't have this skill trained - use default characteristic
            // This maps common skills to their governing characteristics
            characteristic = GetDefaultCharacteristicForSkill(request.Skill);
            skillAdvances = 0; // Untrained skill = 0 advances
        }
        
        // Step 4: Calculate the target number for the roll
        var characteristicValue = GetCharacteristic(character, characteristic);
        
        // Use UI-provided values if available, otherwise use database/default values
        var finalSkillAdvances = request.SkillAdvances != 0 ? request.SkillAdvances : skillAdvances;
        var characteristicAdv = request.CharacteristicAdv;
        var characteristicMod = request.CharacteristicMod;
        
        var totalValue = characteristicValue + characteristicAdv + characteristicMod + finalSkillAdvances;
        
        // Step 5: Roll the dice (d100 in WFRP)
        var diceResult = Random.Shared.Next(1, 101); // d100 roll (1-100)
        var success = diceResult <= totalValue; // Success if roll is under or equal to target
        
        // Step 6: Create the dice roll record for history
        var roll = new DiceRoll
        {
            PlayerName = $"{character.User.Username} ({character.Name})",
            Sides = 100,
            Result = diceResult,
            Timestamp = DateTime.UtcNow
        };

        // Step 7: Save the roll to the database
        _context.DiceRolls.Add(roll);
        await _context.SaveChangesAsync();
        
        // Step 8: Create the detailed result object
        var rollResult = new SkillRollResult
        {
            Roll = roll,
            TargetNumber = totalValue,
            Success = success,
            CharacterName = character.Name,
            TestName = request.TestName,
            SkillName = request.Skill,
            Characteristic = characteristic,
            SkillAdvances = skillAdvances
        };
        
        // Step 9: Broadcast the result to all connected clients via SignalR
        await _hubContext.Clients.All.SendAsync("OnSkillRollReceived", rollResult);

        // Step 10: Return the result to the calling client
        return Ok(rollResult);
    }


    /// <summary>
    /// Maps skill names to their default governing characteristics
    /// This is used when a character doesn't have a specific skill trained
    /// Based on WFRP 4th Edition skill list
    /// </summary>
    /// <param name="skillName">The name of the skill</param>
    /// <returns>The characteristic abbreviation that governs this skill</returns>
    private string GetDefaultCharacteristicForSkill(string skillName)
    {
        // Convert to lowercase for case-insensitive comparison
        var skill = skillName.ToLower();
        
        // Map common WFRP skills to their governing characteristics
        // This covers the most common skills - you can expand this list
        return skill switch
        {
            // Agility-based skills
            "athletics" => "AG",
            "climb" => "AG", 
            "dodge" => "AG",
            "stealth" => "AG",
            
            // Dexterity-based skills
            "art" => "DEX",
            "lockpicking" => "DEX",
            "pickpocket" => "DEX",
            "sleight of hand" => "DEX",
            
            // Intelligence-based skills
            "lore" => "INT",
            "language" => "INT",
            "research" => "INT",
            "trade" => "INT",
            
            // Fellowship-based skills
            "charm" => "FEL",
            "gossip" => "FEL",
            "haggle" => "FEL",
            "intimidate" => "FEL",
            "leadership" => "FEL",
            "perform" => "FEL",
            
            // Willpower-based skills
            "cool" => "WP",
            "endurance" => "WP",
            "pray" => "WP",
            
            // Weapon Skill-based
            "melee" => "WS",
            
            // Ballistic Skill-based  
            "ranged" => "BS",
            
            // Default to Intelligence if skill not found
            // This is a reasonable default for unknown skills
            _ => "INT"
        };
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