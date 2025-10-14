using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthenticationAndCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponSkill = table.Column<int>(type: "INTEGER", nullable: false),
                    BallisticSkill = table.Column<int>(type: "INTEGER", nullable: false),
                    Strength = table.Column<int>(type: "INTEGER", nullable: false),
                    Toughness = table.Column<int>(type: "INTEGER", nullable: false),
                    Initiative = table.Column<int>(type: "INTEGER", nullable: false),
                    Agility = table.Column<int>(type: "INTEGER", nullable: false),
                    Dexterity = table.Column<int>(type: "INTEGER", nullable: false),
                    Intelligence = table.Column<int>(type: "INTEGER", nullable: false),
                    Willpower = table.Column<int>(type: "INTEGER", nullable: false),
                    Fellowship = table.Column<int>(type: "INTEGER", nullable: false),
                    Wounds = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentWounds = table.Column<int>(type: "INTEGER", nullable: false),
                    Movement = table.Column<int>(type: "INTEGER", nullable: false),
                    Species = table.Column<string>(type: "TEXT", nullable: false),
                    Career = table.Column<string>(type: "TEXT", nullable: false),
                    CareerLevel = table.Column<string>(type: "TEXT", nullable: false),
                    CareerPath = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<string>(type: "TEXT", nullable: false),
                    Height = table.Column<string>(type: "TEXT", nullable: false),
                    Hair = table.Column<string>(type: "TEXT", nullable: false),
                    Eyes = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    StatusTier = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentExp = table.Column<int>(type: "INTEGER", nullable: false),
                    SpentExp = table.Column<int>(type: "INTEGER", nullable: false),
                    Fate = table.Column<int>(type: "INTEGER", nullable: false),
                    Fortune = table.Column<int>(type: "INTEGER", nullable: false),
                    Resilience = table.Column<int>(type: "INTEGER", nullable: false),
                    Resolve = table.Column<int>(type: "INTEGER", nullable: false),
                    ShortTermAmbition = table.Column<string>(type: "TEXT", nullable: false),
                    LongTermAmbition = table.Column<string>(type: "TEXT", nullable: false),
                    Corruption = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemName = table.Column<string>(type: "TEXT", nullable: false),
                    ItemType = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsEquipped = table.Column<bool>(type: "INTEGER", nullable: false),
                    Damage = table.Column<string>(type: "TEXT", nullable: true),
                    WeaponGroup = table.Column<string>(type: "TEXT", nullable: true),
                    Range = table.Column<string>(type: "TEXT", nullable: true),
                    Qualities = table.Column<string>(type: "TEXT", nullable: true),
                    ArmourPoints = table.Column<int>(type: "INTEGER", nullable: true),
                    Coverage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterItems_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillName = table.Column<string>(type: "TEXT", nullable: false),
                    Characteristic = table.Column<string>(type: "TEXT", nullable: false),
                    Advances = table.Column<int>(type: "INTEGER", nullable: false),
                    IsSpecialisation = table.Column<bool>(type: "INTEGER", nullable: false),
                    Specialisation = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterSkills_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTalents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    TalentName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TimesAdvanced = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTalents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterTalents_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterItems_CharacterId",
                table: "CharacterItems",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSkills_CharacterId",
                table: "CharacterSkills",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTalents_CharacterId",
                table: "CharacterTalents",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterItems");

            migrationBuilder.DropTable(
                name: "CharacterSkills");

            migrationBuilder.DropTable(
                name: "CharacterTalents");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
