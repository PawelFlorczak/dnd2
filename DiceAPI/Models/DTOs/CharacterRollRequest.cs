namespace DiceAPI.Models.DTOs;

public class CharacterRollRequest
{
    public int CharacterId { get; set; }
    public string Characteristic { get; set; } = string.Empty; // WS, BS, S, T, I, Ag, Dex, Int, WP, Fel
    public int CharacteristicAdv { get; set; }
    public int CharacteristicMod { get; set; }
    public string? TestName { get; set; }
}

public class CharacterRollResult
{
    public DiceRoll Roll { get; set; } = null!;
    public int TargetNumber { get; set; }
    public bool Success { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
}