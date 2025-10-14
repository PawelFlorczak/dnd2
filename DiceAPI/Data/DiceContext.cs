using Microsoft.EntityFrameworkCore;
using DiceAPI.Models;

namespace DiceAPI.Data;

public class DiceContext : DbContext
{
    public DiceContext(DbContextOptions<DiceContext> options) : base(options) { }

    public DbSet<DiceRoll> DiceRolls { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterSkill> CharacterSkills { get; set; }
    public DbSet<CharacterTalent> CharacterTalents { get; set; }
    public DbSet<CharacterItem> CharacterItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        // Configure Character entity
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany(e => e.Characters)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CharacterSkill entity
        modelBuilder.Entity<CharacterSkill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Character)
                  .WithMany(e => e.Skills)
                  .HasForeignKey(e => e.CharacterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CharacterTalent entity
        modelBuilder.Entity<CharacterTalent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Character)
                  .WithMany(e => e.Talents)
                  .HasForeignKey(e => e.CharacterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CharacterItem entity
        modelBuilder.Entity<CharacterItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Character)
                  .WithMany(e => e.Items)
                  .HasForeignKey(e => e.CharacterId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Update DiceRoll to link with User
        modelBuilder.Entity<DiceRoll>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlayerName).HasMaxLength(50);
        });
    }
}