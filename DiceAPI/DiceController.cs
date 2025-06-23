using Microsoft.AspNetCore.Mvc;

namespace DiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DiceController : ControllerBase
{
    private readonly DiceContext _context;

    public DiceController(DiceContext context)
    {
        _context = context;
    }

    [HttpGet("roll")]
    public async Task<IActionResult> Roll([FromQuery] int sides = 20, [FromQuery] string player = "unknown")
    {
        var result = Random.Shared.Next(1, sides + 1);
        var roll = new DiceRoll
        {
            PlayerName = player,
            Sides = sides,
            Result = result,
            Timestamp = DateTime.UtcNow
        };

        _context.DiceRolls.Add(roll);
        await _context.SaveChangesAsync();

        return Ok(new { result });
    }

}