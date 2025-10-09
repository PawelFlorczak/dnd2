using DiceAPI.Data;
using DiceAPI.Models;
using DiceAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace DiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DiceController : ControllerBase
{
    private readonly DiceContext _context;
    private readonly IHubContext<DiceHub> _hubContext;

    public DiceController(DiceContext context, IHubContext<DiceHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
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
        
        await _hubContext.Clients.All.SendAsync("OnRollReceived", roll);

        return Ok(roll);
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] string? player = null)
    {
        var query = _context.DiceRolls.AsQueryable();

        if (!string.IsNullOrWhiteSpace(player))
        {
            query = query.Where(r => r.PlayerName == player);
        }

        var rolls = await query
            .OrderByDescending(r => r.Timestamp)
            .Take(50)
            .ToListAsync();

        return Ok(rolls);
    }

    

}