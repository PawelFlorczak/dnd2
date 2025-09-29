#nullable enable
namespace DiceAPI.Models;

public class DiceRoll
{
    public int Id { get; set; }
    public string? PlayerName { get; set; }
    public int Sides { get; set; }
    public int Result { get; set; }
    public DateTime Timestamp { get; set; }
}