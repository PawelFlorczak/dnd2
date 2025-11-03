namespace DiceAPI.Models.DTOs;

/// <summary>
/// Request model for skill-based dice rolls
/// This matches exactly what the CharacterSheetUI.cs sends from the client
/// </summary>
public class SkillRollRequest
{
    /// <summary>
    /// The ID of the character performing the skill roll
    /// </summary>
    public int CharacterId { get; set; }
    
    /// <summary>
    /// The name of the skill being tested (e.g., "Athletics", "Stealth", "Lore")
    /// This comes directly from the client button press
    /// </summary>
    public string Skill { get; set; } = string.Empty;
    
    /// <summary>
    /// A descriptive name for the test being performed (e.g., "Athletics Test")
    /// </summary>
    public string TestName { get; set; } = string.Empty;
}

/// <summary>
/// Result model for skill-based dice rolls
/// Contains all the information needed to display the roll result
/// </summary>
public class SkillRollResult
{
    /// <summary>
    /// The actual dice roll information (result, timestamp, etc.)
    /// </summary>
    public DiceRoll Roll { get; set; } = null!;
    
    /// <summary>
    /// The target number that needed to be rolled under or equal to succeed
    /// This is calculated as: Characteristic + Skill Advances
    /// </summary>
    public int TargetNumber { get; set; }
    
    /// <summary>
    /// Whether the roll was successful (dice result <= target number)
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// The name of the character who performed the roll
    /// </summary>
    public string CharacterName { get; set; } = string.Empty;
    
    /// <summary>
    /// The name of the test that was performed
    /// </summary>
    public string TestName { get; set; } = string.Empty;
    
    /// <summary>
    /// The name of the skill that was tested
    /// </summary>
    public string SkillName { get; set; } = string.Empty;
    
    /// <summary>
    /// The characteristic that governs this skill (e.g., "AG" for Athletics)
    /// </summary>
    public string Characteristic { get; set; } = string.Empty;
    
    /// <summary>
    /// The number of advances the character has in this skill
    /// </summary>
    public int SkillAdvances { get; set; }
}