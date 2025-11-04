# Skill Roll Endpoint Implementation Summary

## What Was Created/Modified

### 1. **SkillRollRequest.cs** (Updated)
- **Purpose**: Defines the data structure that the client sends when requesting a skill roll
- **Properties**:
  - `CharacterId`: Which character is making the roll
  - `Skill`: Name of the skill being tested (e.g., "Athletics")
  - `TestName`: Descriptive name for the test (e.g., "Athletics Test")

### 2. **SkillRollResult.cs** (Updated)
- **Purpose**: Defines the detailed response sent back to the client
- **Properties**:
  - `Roll`: The actual dice roll data
  - `TargetNumber`: The value that needed to be rolled under
  - `Success`: Whether the roll succeeded
  - `CharacterName`, `TestName`, `SkillName`: Display information
  - `Characteristic`: Which characteristic governs this skill
  - `SkillAdvances`: How many advances the character has in this skill

### 3. **DiceController.cs** - New `SkillRoll` Method
- **Purpose**: Handles skill-based dice rolls from the client

## How the SkillRoll Method Works (Step by Step)

### Step 1: Find the Character
```csharp
var character = await _context.Characters
    .Include(c => c.User)        // Get username for display
    .Include(c => c.Skills)      // Get character's trained skills
    .FirstOrDefaultAsync(c => c.Id == request.CharacterId);
```
**What it does**: Looks up the character in the database and loads their skills

### Step 2: Check if Character Has This Skill
```csharp
var characterSkill = character.Skills
    .FirstOrDefault(s => s.SkillName.Equals(request.Skill, StringComparison.OrdinalIgnoreCase));
```
**What it does**: Searches the character's skill list to see if they have this specific skill trained

### Step 3: Determine Characteristic and Advances
```csharp
if (characterSkill != null)
{
    // Character has this skill trained
    characteristic = characterSkill.Characteristic;
    skillAdvances = characterSkill.Advances;
}
else
{
    // Character doesn't have this skill - use default
    characteristic = GetDefaultCharacteristicForSkill(request.Skill);
    skillAdvances = 0;
}
```
**What it does**: 
- If the character has the skill trained → use their specific data
- If not → use default characteristic mapping and 0 advances

### Step 4: Calculate Target Number
```csharp
var characteristicValue = GetCharacteristic(character, characteristic);
var totalValue = characteristicValue + skillAdvances;
```
**What it does**: Adds the character's characteristic value + their skill advances

### Step 5: Roll the Dice
```csharp
var diceResult = Random.Shared.Next(1, 101); // d100 roll (1-100)
var success = diceResult <= totalValue;       // Success if roll ≤ target
```
**What it does**: Rolls a d100 and checks if it's ≤ the target number

### Step 6-10: Save and Broadcast
- Saves the roll to the database for history
- Creates a detailed result object
- Broadcasts to all connected clients via SignalR
- Returns the result to the calling client

## Key Helper Method: GetDefaultCharacteristicForSkill

This method maps skill names to their governing characteristics when a character doesn't have the skill trained:

```csharp
return skill switch
{
    "athletics" => "AG",      // Agility
    "stealth" => "AG",        // Agility
    "lore" => "INT",          // Intelligence
    "charm" => "FEL",         // Fellowship
    "cool" => "WP",           // Willpower
    // ... many more
    _ => "INT"                // Default to Intelligence
};
```

## How It Connects to the Client

The client code in `CharacterSheetUI.cs` calls:
```csharp
private void OnBasicSkillRoll(string skill)
{
    var rollData = new
    {
        characterId = _currentCharacterId,
        skill = skill,
        testName = $"{skill} Test"
    };
    
    _httpRequest.Request("http://localhost:5000/dice/skill-roll", headers, HttpClient.Method.Post, json);
}
```

This perfectly matches our `SkillRollRequest` structure!

## What You Learned

1. **API Endpoint Creation**: How to create a new REST endpoint
2. **Database Queries**: Using Entity Framework with `.Include()` to load related data
3. **Conditional Logic**: Handling cases where data might or might not exist
4. **Data Mapping**: Converting between different data formats
5. **SignalR Integration**: Broadcasting real-time updates to connected clients
6. **WFRP Game Logic**: How skill rolls work in the game system

## Testing the Implementation

You can test this by:
1. Starting the API: `dotnet run` in the DiceAPI folder
2. Running the Godot client
3. Clicking any skill button in the character sheet
4. The skill roll should now work and broadcast to all connected clients!

## Possible Extensions

- Add modifiers support (situational bonuses/penalties)
- Add critical success/failure detection
- Add different difficulty levels
- Support for specialized skills
- Add skill roll history filtering