namespace DiceAPI.Models;

public class CharacterItem
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character? Character { get; set; }
    
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // Weapon, Armour, Equipment, etc.
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal Weight { get; set; }
    public bool IsEquipped { get; set; }
    
    // Weapon specific properties
    public string? Damage { get; set; }
    public string? WeaponGroup { get; set; }
    public string? Range { get; set; }
    public string? Qualities { get; set; }
    
    // Armour specific properties
    public int? ArmourPoints { get; set; }
    public string? Coverage { get; set; }
}