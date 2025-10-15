# WFRP Dice & Character API - Endpoints Summary

## 🌐 **Swagger Documentation**
- **Development**: http://localhost:5000/swagger
- **Production**: http://localhost:5000/api-docs

## 🔐 **Authentication Endpoints**

### POST `/auth/register`
Register a new user account
```json
{
  "username": "string",
  "password": "string", 
  "email": "string"
}
```
**Response**: `AuthResponse` with user details

### POST `/auth/login`
Login with existing credentials
```json
{
  "username": "string",
  "password": "string"
}
```
**Response**: `AuthResponse` with user details and character list

## 👤 **Character Management Endpoints**

### GET `/character/user/{userId}`
Get all characters for a specific user
**Response**: Array of `Character` objects

### GET `/character/{id}`
Get detailed character information
**Response**: Single `Character` object with skills, talents, items

### POST `/character`
Create a new character
```json
{
  "userId": 1,
  "name": "Character Name",
  "species": "Human",
  "career": "Soldier",
  "weaponSkill": 35,
  "ballisticSkill": 30,
  // ... all WFRP characteristics
}
```
**Response**: Created `Character` object

### PUT `/character/{id}`
Update existing character
**Request**: Full `Character` object
**Response**: Updated `Character` object

### DELETE `/character/{id}`
Delete a character
**Response**: 204 No Content

### POST `/character/{characterId}/skills`
Add a skill to character
```json
{
  "skillName": "Melee (Basic)",
  "characteristic": "WS",
  "advances": 5,
  "isSpecialisation": false
}
```

### POST `/character/{characterId}/talents`
Add a talent to character
```json
{
  "talentName": "Strike Mighty Blow",
  "description": "Deal extra damage",
  "timesAdvanced": 1
}
```

### POST `/character/{characterId}/items`
Add an item to character
```json
{
  "itemName": "Hand Weapon",
  "itemType": "Weapon",
  "damage": "SB+4",
  "weaponGroup": "Basic",
  "isEquipped": true
}
```

## 🎲 **Dice Rolling Endpoints**

### GET `/dice/roll`
Basic dice roll
**Parameters**:
- `sides` (default: 20) - Number of sides on the die
- `player` (default: "unknown") - Player name
- `userId` (optional) - User ID for automatic player name

**Response**: `DiceRoll` object

### POST `/dice/character-roll`
Character-based WFRP d100 roll
```json
{
  "characterId": 1,
  "characteristic": "WS",
  "skillAdvances": 5,
  "testName": "Weapon Skill Test",
  "modifier": 10
}
```
**Response**: `CharacterRollResult` with success/failure

### GET `/dice/history`
Get recent dice roll history
**Response**: Array of recent `DiceRoll` objects

## 📡 **SignalR Hub**
**Endpoint**: `/DiceHub`
**Events**:
- `OnRollReceived` - Basic dice rolls
- `OnCharacterRollReceived` - Character-based rolls

## 🎯 **Key Features**
- ✅ Full WFRP 4th Edition character support
- ✅ Real-time dice rolling with SignalR
- ✅ User authentication & character ownership
- ✅ Complete CRUD operations for characters
- ✅ Skills, talents, and items management
- ✅ d100 characteristic-based rolling
- ✅ Comprehensive Swagger documentation

## 🧪 **Testing with Swagger**
1. Navigate to http://localhost:5000/swagger
2. Register a new user via `/auth/register`
3. Login via `/auth/login` to get user details
4. Create a character via `/character`
5. Test character rolls via `/dice/character-roll`
6. View roll history via `/dice/history`

All endpoints are fully documented and testable through the Swagger UI!