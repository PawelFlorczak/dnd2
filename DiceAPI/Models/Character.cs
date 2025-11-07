namespace DiceAPI.Models;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }

    // WFRP 4th Edition Core Characteristics
    public int WeaponSkill { get; set; }
    public int BallisticSkill { get; set; }
    public int Strength { get; set; }
    public int Toughness { get; set; }
    public int Initiative { get; set; }
    public int Agility { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    public int Willpower { get; set; }
    public int Fellowship { get; set; }
    
    public int WeaponSkillAdv { get; set; }
    
    public int WeaponSkillMod {get;set;}

    // Secondary Characteristics
    public int Wounds { get; set; }
    public int CurrentWounds { get; set; }
    public int Movement { get; set; }

    // Character Details
    public string Species { get; set; } = string.Empty; // Human, Dwarf, Halfling, High Elf, Wood Elf
    public string Career { get; set; } = string.Empty;
    public string CareerLevel { get; set; } = string.Empty;
    public string CareerPath { get; set; } = string.Empty;

    // Background
    public string Age { get; set; } = string.Empty;
    public string Height { get; set; } = string.Empty;
    public string Hair { get; set; } = string.Empty;
    public string Eyes { get; set; } = string.Empty;

    // Status
    public string Status { get; set; } = string.Empty; // Bronze, Silver, Gold
    public int StatusTier { get; set; }

    // Experience
    public int CurrentExp { get; set; }
    public int SpentExp { get; set; }

    // Fate and Fortune
    public int Fate { get; set; }
    public int Fortune { get; set; }

    // Resilience and Resolve  
    public int Resilience { get; set; }
    public int Resolve { get; set; }

    // Motivation and Ambitions
    public string ShortTermAmbition { get; set; } = string.Empty;
    public string LongTermAmbition { get; set; } = string.Empty;

    // Corruption
    public int Corruption { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<CharacterSkill> Skills { get; set; } = new();
}
