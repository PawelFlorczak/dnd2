namespace DiceAPI.Models;

public class CharacterSkill
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character? Character { get; set; }
    
    public string SkillName { get; set; } = string.Empty;
    public string Characteristic { get; set; } = string.Empty; // WS, BS, S, T, I, Ag, Dex, Int, WP, Fel
    public int Advances { get; set; }
    public bool IsSpecialisation { get; set; }
    public string Specialisation { get; set; } = string.Empty;
}