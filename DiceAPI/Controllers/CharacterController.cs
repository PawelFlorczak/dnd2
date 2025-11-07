using DiceAPI.Data;
using DiceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Tags("Character Management")]
public class CharacterController : ControllerBase
{
    private readonly DiceContext _context;

    public CharacterController(DiceContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserCharacters(int userId)
    {
        var characters = await _context.Characters
            .Where(c => c.UserId == userId)
            .Include(c => c.Skills)
            .ToListAsync();

        return Ok(characters);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCharacter(int id)
    {
        var character = await _context.Characters
            .Include(c => c.Skills)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character == null)
        {
            return NotFound("Character not found.");
        }

        return Ok(character);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacter([FromBody] Character character)
    {
        if (character == null)
        {
            return BadRequest("Character data is required.");
        }

        // Verify user exists
        var user = await _context.Users.FindAsync(character.UserId);
        if (user == null)
        {
            return BadRequest("Invalid user ID.");
        }

        character.CreatedAt = DateTime.UtcNow;
        character.UpdatedAt = DateTime.UtcNow;

        _context.Characters.Add(character);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCharacter(int id, [FromBody] Character character)
    {
        if (id != character.Id)
        {
            return BadRequest("Character ID mismatch.");
        }

        var existingCharacter = await _context.Characters.FindAsync(id);
        if (existingCharacter == null)
        {
            return NotFound("Character not found.");
        }

        // Update character properties
        existingCharacter.Name = character.Name;
        
        // Core characteristics
        existingCharacter.WeaponSkill = character.WeaponSkill;
        existingCharacter.BallisticSkill = character.BallisticSkill;
        existingCharacter.Strength = character.Strength;
        existingCharacter.Toughness = character.Toughness;
        existingCharacter.Initiative = character.Initiative;
        existingCharacter.Agility = character.Agility;
        existingCharacter.Dexterity = character.Dexterity;
        existingCharacter.Intelligence = character.Intelligence;
        existingCharacter.Willpower = character.Willpower;
        existingCharacter.Fellowship = character.Fellowship;
        
        // Characteristic advances
        existingCharacter.WeaponSkillAdv = character.WeaponSkillAdv;
        existingCharacter.BallisticSkillAdv = character.BallisticSkillAdv;
        existingCharacter.StrengthAdv = character.StrengthAdv;
        existingCharacter.ToughnessAdv = character.ToughnessAdv;
        existingCharacter.InitiativeAdv = character.InitiativeAdv;
        existingCharacter.AgilityAdv = character.AgilityAdv;
        existingCharacter.DexterityAdv = character.DexterityAdv;
        existingCharacter.IntelligenceAdv = character.IntelligenceAdv;
        existingCharacter.WillpowerAdv = character.WillpowerAdv;
        existingCharacter.FellowshipAdv = character.FellowshipAdv;
        
        // Characteristic modifiers
        existingCharacter.WeaponSkillMod = character.WeaponSkillMod;
        existingCharacter.BallisticSkillMod = character.BallisticSkillMod;
        existingCharacter.StrengthMod = character.StrengthMod;
        existingCharacter.ToughnessMod = character.ToughnessMod;
        existingCharacter.InitiativeMod = character.InitiativeMod;
        existingCharacter.AgilityMod = character.AgilityMod;
        existingCharacter.DexterityMod = character.DexterityMod;
        existingCharacter.IntelligenceMod = character.IntelligenceMod;
        existingCharacter.WillpowerMod = character.WillpowerMod;
        existingCharacter.FellowshipMod = character.FellowshipMod;
            
        existingCharacter.Wounds = character.Wounds;
        existingCharacter.CurrentWounds = character.CurrentWounds;
        existingCharacter.Movement = character.Movement;
        existingCharacter.Species = character.Species;
        existingCharacter.Career = character.Career;
        existingCharacter.CareerLevel = character.CareerLevel;
        existingCharacter.CareerPath = character.CareerPath;
        existingCharacter.Age = character.Age;
        existingCharacter.Height = character.Height;
        existingCharacter.Hair = character.Hair;
        existingCharacter.Eyes = character.Eyes;
        existingCharacter.Status = character.Status;
        existingCharacter.StatusTier = character.StatusTier;
        existingCharacter.CurrentExp = character.CurrentExp;
        existingCharacter.SpentExp = character.SpentExp;
        existingCharacter.Fate = character.Fate;
        existingCharacter.Fortune = character.Fortune;
        existingCharacter.Resilience = character.Resilience;
        existingCharacter.Resolve = character.Resolve;
        existingCharacter.ShortTermAmbition = character.ShortTermAmbition;
        existingCharacter.LongTermAmbition = character.LongTermAmbition;
        
        // Basic Skills Advances
        existingCharacter.ArtAdv = character.ArtAdv;
        existingCharacter.AthleticsAdv = character.AthleticsAdv;
        existingCharacter.BriberyAdv = character.BriberyAdv;
        existingCharacter.CharmAdv = character.CharmAdv;
        existingCharacter.CharmAnimalAdv = character.CharmAnimalAdv;
        existingCharacter.ClimbAdv = character.ClimbAdv;
        existingCharacter.CoolAdv = character.CoolAdv;
        existingCharacter.ConsumeAlcoholAdv = character.ConsumeAlcoholAdv;
        existingCharacter.DodgeAdv = character.DodgeAdv;
        existingCharacter.DriveAdv = character.DriveAdv;
        existingCharacter.EnduranceAdv = character.EnduranceAdv;
        existingCharacter.EntertainAdv = character.EntertainAdv;
        existingCharacter.GambleAdv = character.GambleAdv;
        existingCharacter.GossipAdv = character.GossipAdv;
        existingCharacter.HaggleAdv = character.HaggleAdv;
        existingCharacter.IntimidateAdv = character.IntimidateAdv;
        existingCharacter.IntuitionAdv = character.IntuitionAdv;
        existingCharacter.LeadershipAdv = character.LeadershipAdv;
        existingCharacter.MeleeBasicAdv = character.MeleeBasicAdv;
        existingCharacter.MeleeAdv = character.MeleeAdv;
        existingCharacter.NavigationAdv = character.NavigationAdv;
        existingCharacter.OutdoorSurvivalAdv = character.OutdoorSurvivalAdv;
        existingCharacter.PerceptionAdv = character.PerceptionAdv;
        existingCharacter.RideAdv = character.RideAdv;
        existingCharacter.RowAdv = character.RowAdv;
        existingCharacter.StealthAdv = character.StealthAdv;
        
        existingCharacter.Corruption = character.Corruption;
        existingCharacter.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingCharacter);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacter(int id)
    {
        var character = await _context.Characters.FindAsync(id);
        if (character == null)
        {
            return NotFound("Character not found.");
        }

        _context.Characters.Remove(character);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{characterId}/skills")]
    public async Task<IActionResult> AddSkill(int characterId, [FromBody] CharacterSkill skill)
    {
        var character = await _context.Characters.FindAsync(characterId);
        if (character == null)
        {
            return NotFound("Character not found.");
        }

        skill.CharacterId = characterId;
        _context.CharacterSkills.Add(skill);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCharacter), new { id = characterId }, skill);
    }
}