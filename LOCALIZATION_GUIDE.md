# Godot Localization Setup Guide

This guide explains how to use the two-language localization system (English/Polish) implemented in your WFRP project.

## What's Been Set Up

### 1. Files Created
- `localization/en.csv` - Translation file with English and Polish text
- `LocalizationManager.cs` - Singleton that manages language switching
- `LocalizedUI.cs` - Base class for UI components that need localization
- Updated `LoginUI.cs` - Example implementation

### 2. Project Configuration
- Added internationalization settings to `project.godot`
- Added LocalizationManager as autoload singleton
- Added language toggle button to login UI

## How to Use

### For Users
- Click the language button (shows "EN → PL" or "PL → EN") to switch languages
- Language preference is automatically saved and restored

### For Developers

#### Step 1: Add New Translations
Edit `localization/en.csv` and add new lines:
```csv
NEW_KEY,English Text,Polish Text
SAVE,Save,Zapisz
CANCEL,Cancel,Anuluj
```

#### Step 2: Make UI Components Localizable
Change your UI class inheritance:
```csharp
// Before
public partial class MyUI : Control

// After  
public partial class MyUI : LocalizedUI
```

#### Step 3: Register Controls for Localization
```csharp
public override void _Ready()
{
    // Set up your nodes first
    _myButton = GetNode<Button>("MyButton");
    _myLabel = GetNode<Label>("MyLabel");
    
    // Call base _Ready to initialize localization
    base._Ready();
}

protected override void RegisterLocalizedControls()
{
    RegisterControl("SAVE", _myButton);        // For button text
    RegisterControl("TITLE", _myLabel);        // For label text
    RegisterControl("USERNAME", _myLineEdit);  // For placeholder text
}
```

#### Step 4: Handle Custom Text Updates (Optional)
```csharp
protected override void UpdateTexts()
{
    base.UpdateTexts(); // Handle registered controls
    
    // Custom text handling
    someOtherLabel.Text = Tr("CUSTOM_KEY");
}
```

## Available Translation Keys

Current keys in the CSV file:
- `TITLE` - Main title
- `USERNAME`, `PASSWORD`, `EMAIL` - Form fields
- `LOGIN`, `REGISTER` - Action buttons
- `SWITCH_TO_REGISTER`, `SWITCH_TO_LOGIN` - Mode toggles
- `WELCOME`, `LOGOUT` - Game UI
- `CHARACTERISTICS`, `SKILLS`, `TALENTS`, etc. - WFRP terms
- `ROLL` - Dice rolling

## Quick Implementation for Existing Components

1. **DiceUI.cs**: Change to inherit from `LocalizedUI`, register button texts
2. **CharacterSheetUI.cs**: Change to inherit from `LocalizedUI`, register all labels and buttons
3. **Any new UI**: Follow the pattern shown in `LoginUI.cs`

## Testing

1. Run the project
2. On login screen, click the language button
3. Observe text changes between English and Polish
4. Restart the project - language preference should be remembered

## Adding More Languages

To add a third language (e.g., German):
1. Add a `de` column to `en.csv`
2. Add `German` to the `Language` enum in `LocalizationManager.cs`
3. Update the language switching logic in `LocalizationManager.cs`