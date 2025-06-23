using Microsoft.AspNetCore.Mvc;

namespace DiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DiceController : ControllerBase
{
    private readonly ILogger<DiceController> _logger;

    public DiceController(ILogger<DiceController> logger)
    {
        _logger = logger;
    }

    [HttpGet("roll")]
    public IActionResult Roll([FromQuery] int sides = 6)
    {
        var result = Random.Shared.Next(1, sides + 1);
        return Ok(new { result });
    }
}