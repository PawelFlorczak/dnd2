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
    
    // Characteristic Advances
    public int WeaponSkillAdv { get; set; }
    public int BallisticSkillAdv { get; set; }
    public int StrengthAdv { get; set; }
    public int ToughnessAdv { get; set; }
    public int InitiativeAdv { get; set; }
    public int AgilityAdv { get; set; }
    public int DexterityAdv { get; set; }
    public int IntelligenceAdv { get; set; }
    public int WillpowerAdv { get; set; }
    public int FellowshipAdv { get; set; }
    
    // Characteristic Modifiers
    public int WeaponSkillMod { get; set; }
    public int BallisticSkillMod { get; set; }
    public int StrengthMod { get; set; }
    public int ToughnessMod { get; set; }
    public int InitiativeMod { get; set; }
    public int AgilityMod { get; set; }
    public int DexterityMod { get; set; }
    public int IntelligenceMod { get; set; }
    public int WillpowerMod { get; set; }
    public int FellowshipMod { get; set; }

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

    // Basic Skills Advances
    public int ArtAdv { get; set; }
    public int AthleticsAdv { get; set; }
    public int BriberyAdv { get; set; }
    public int CharmAdv { get; set; }
    public int CharmAnimalAdv { get; set; }
    public int ClimbAdv { get; set; }
    public int CoolAdv { get; set; }
    public int ConsumeAlcoholAdv { get; set; }
    public int DodgeAdv { get; set; }
    public int DriveAdv { get; set; }
    public int EnduranceAdv { get; set; }
    public int EntertainAdv { get; set; }
    public int GambleAdv { get; set; }
    public int GossipAdv { get; set; }
    public int HaggleAdv { get; set; }
    public int IntimidateAdv { get; set; }
    public int IntuitionAdv { get; set; }
    public int LeadershipAdv { get; set; }
    public int MeleeBasicAdv { get; set; }
    public int MeleeAdv { get; set; }
    public int NavigationAdv { get; set; }
    public int OutdoorSurvivalAdv { get; set; }
    public int PerceptionAdv { get; set; }
    public int RideAdv { get; set; }
    public int RowAdv { get; set; }
    public int StealthAdv { get; set; }

    // Corruption
    public int Corruption { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<CharacterSkill> Skills { get; set; } = new();
}
