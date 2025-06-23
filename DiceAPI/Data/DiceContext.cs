using Microsoft.EntityFrameworkCore;
using DiceAPI.Models;

namespace DiceAPI.Data;

public class DiceContext : DbContext
{
    public DiceContext(DbContextOptions<DiceContext> options) : base(options) { }

    public DbSet<DiceRoll> DiceRolls { get; set; }
}