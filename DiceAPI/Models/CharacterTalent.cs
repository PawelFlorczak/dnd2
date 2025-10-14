namespace DiceAPI.Models;

public class CharacterTalent
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character? Character { get; set; }
    
    public string TalentName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TimesAdvanced { get; set; } = 1;
}