using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

public partial class CharacterSheetUI : Control
{
    private OptionButton _characterSelect;
    private HttpRequest _httpRequest;
    private int _currentUserId;
    private int _currentCharacterId;
    
    // Header Buttons
    private Button _saveButton;
    private Button _newCharacterButton;
    
    // Character Form
    // Character Basic Info controls
    private LineEdit _nameInput;
    private LineEdit _speciesInput;
    private LineEdit _classInput;
    private LineEdit _careerInput;
    private LineEdit _careerLevelInput;
    private LineEdit _careerPathInput;
    private LineEdit _statusInput;
    private LineEdit _ageInput;
    private LineEdit _heightInput;
    private LineEdit _hairInput;
    private LineEdit _eyesInput;
    
    // Characteristics
    private SpinBox _wsInput, _bsInput, _sInput, _tInput, _iInput;
    private SpinBox _agInput, _dexInput, _intInput, _wpInput, _felInput;
    
    private SpinBox _wsInputAdv, _bsInputAdv, _sInputAdv, _tInputAdv, _iInputAdv;
    private SpinBox _agInputAdv, _dexInputAdv, _intInputAdv, _wpInputAdv, _felInputAdv;
    
    private SpinBox _wsInputMod, _bsInputMod, _sInputMod, _tInputMod, _iInputMod;
    private SpinBox _agInputMod, _dexInputMod, _intInputMod, _wpInputMod, _felInputMod;
    
    private Label _wsInputCur, _bsInputCur, _sInputCur, _tInputCur, _iInputCur;
    private Label _agInputCur, _dexInputCur, _intInputCur, _wpInputCur, _felInputCur;   
    
    // Roll buttons for characteristics
    private Dictionary<string, Button> _rollButtons = new();
    
    // Resilience
    private SpinBox _resilienceInput, _resolveInput, _motivationInput;
    
    //Experience points
    private SpinBox _currentExpInput, _spentExpInput, _totalExpInput;
    
    // Fate
    private SpinBox _fateInput, _fortuneInput;
    
    // Movement
    private SpinBox _movementInput, _walkInput, _runInput;
    
    // Basic Skills
    private Dictionary<string, Button> _basicSkillButton = new();
    
    // Grouped & Advanced Skills
    private TextEdit _advNameEdit, _advCharacteristicEdit, _advCharacteristicBaseEdit, _advCharacteristicAdvEdit, _advSkillEdit;
    
    // Talents
    private TextEdit _talentNameEdit, _talentTimesTakenEdit, _talentDescriptionEdit;
    
    // Ambitions
    private TextEdit _shortAmbitionEdit, _longAmbitionEdit;
    
    // Party
    private LineEdit _partyNameEdit;
    private TextEdit _partyShortAmbitionEdit, _partyLongAmbitionEdit, _partyMembersEdit;
    
    
    // Equipment Form
    // Secondary characteristics
    private SpinBox _woundsInput, _currentWoundsInput;
    

    

    
    public override void _Ready()
    {
        SetupUI();
        _httpRequest = GetNode<HttpRequest>("HTTPRequest");
        _httpRequest.RequestCompleted += OnRequestCompleted;
    }

    private void SetupUI()
    {
        // Get UI elements (assuming they're set up in the scene)
        _characterSelect = GetNode<OptionButton>("VBox/Header/CharacterSelect");
        
        try
        {
            _saveButton = GetNode<Button>("VBox/Header/SaveButton");
            GD.Print("✅ SaveButton found!");
        }
        catch (Exception ex)
        {
            GD.PrintErr("❌ SaveButton not found: ", ex.Message);
        }
        
        try
        {
            _newCharacterButton = GetNode<Button>("VBox/Header/NewButton");
            GD.Print("✅ NewButton found!");
        }
        catch (Exception ex)
        {
            GD.PrintErr("❌ NewButton not found: ", ex.Message);
        }
        
        // Character basic info
        _nameInput = GetNode<LineEdit>("VBox/ScrollContainer/CharacterForm/BasicInfo/NameInput");
        _speciesInput = GetNode<LineEdit>("VBox/ScrollContainer/CharacterForm/BasicInfo/SpeciesInput");
        _classInput = GetNode<LineEdit>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/BasicInfoContainer/HBoxContainer/ClassInput");
        _careerInput = GetNode<LineEdit>("VBox/ScrollContainer/CharacterForm/BasicInfo/CareerInput");
        
        // Characteristics
        _wsInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/WSInput");
        _bsInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/BSInput");
        _sInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/SInput");
        _tInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/TInput");
        _iInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/IInput");
        _agInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/AgInput");
        _dexInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/DexInput");
        _intInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/IntInput");
        _wpInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/WPInput");
        _felInput = GetNode<SpinBox>("VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsContainer/FelInput");
        
        // Secondary characteristics
        _woundsInput = GetNode<SpinBox>("VBox/ScrollContainer/CharacterForm/Secondary/WoundsContainer/WoundsInput");
        _currentWoundsInput = GetNode<SpinBox>("VBox/ScrollContainer/CharacterForm/Secondary/WoundsContainer/CurrentWoundsInput");
        _movementInput = GetNode<SpinBox>("VBox/ScrollContainer/CharacterForm/Secondary/MovementContainer/MovementInput");
        
        // Status and XP
        _statusInput = GetNode<LineEdit>("VBox/ScrollContainer/CharacterForm/Status/StatusInput");
        _currentExpInput = GetNode<SpinBox>("VBox/ScrollContainer/CharacterForm/Status/ExpContainer/CurrentExpInput");
        _spentExpInput = GetNode<SpinBox>("VBox/ScrollContainer/CharacterForm/Status/ExpContainer/SpentExpInput");
        _fateInput = GetNode<SpinBox>("VBox/ScrollContainer/CharacterForm/Status/FateContainer/FateInput");
        _fortuneInput = GetNode<SpinBox>("VBox/ScrollContainer/CharacterForm/Status/FateContainer/FortuneInput");
        
        // Connect signals
        _characterSelect.ItemSelected += OnCharacterSelected;
        
        if (_saveButton != null)
        {
            _saveButton.Pressed += OnSavePressed;
            GD.Print("✅ SaveButton signal connected!");
        }
        
        if (_newCharacterButton != null)
        {
            _newCharacterButton.Pressed += OnNewCharacterPressed;
            GD.Print("✅ NewButton signal connected!");
        }
        
        // Setup roll buttons
        SetupRollButtons();
        SetupBasicSkillsButtons();
    }

    private void SetupRollButtons()
    {
        var characteristics = new[] { "WS", "BS", "S", "T", "I", "AG", "DEX", "INT", "WP", "FEL" };
        
        foreach (var characteristic in characteristics)
        {
            var button = GetNode<Button>($"VBox/TabContainer/Character/TextureCharacter/CharacterForm/CharacteristicsRollContainer/{characteristic}RollButton");
            _rollButtons[characteristic] = button;
            button.Pressed += () => OnCharacteristicRoll(characteristic);
        }
    }
    
    private void SetupBasicSkillsButtons()
    {
        var skills = new[]
        {
            "Art", "Athletics", "Bribery", "Charm", "CharmAnimal", "Climb","Cool", "ConsumeAlcohol","Dodge", "Drive", "Endurance", "Entertain", "Gamble",
            "Gossip", "Haggle","Intimidate", "Intuition", "Leadership", "MeleeBasic", "Melee", "Navigation", "OutdoorSurvival", "Perception","Ride","Row","Stealth"
        };
        
        foreach (var skill in skills)
        {
            var button = GetNode<Button>($"VBox/TabContainer/Character/TextureCharacter/CharacterForm/BasicSkillsContainer1/HBox{skill}/Button");
            _rollButtons[skill] = button;
            button.Pressed += () => OnBasicSkillRoll(skill);
        }
    }

    public void LoadUserCharacters(int userId)
    {
        _currentUserId = userId;
        _httpRequest.Request($"http://localhost:5000/character/user/{userId}");
    }

    private void OnCharacterSelected(long index)
    {
        if (index >= 0 && _characterSelect.GetItemMetadata((int)index).AsInt32() is int characterId)
        {
            _currentCharacterId = characterId;
            LoadCharacter(characterId);
        }
    }

    private void LoadCharacter(int characterId)
    {
        _httpRequest.Request($"http://localhost:5000/character/{characterId}");
    }

    private void OnCharacteristicRoll(string characteristic)
    {
        if (_currentCharacterId == 0) return;
        
        var rollData = new
        {
            characterId = _currentCharacterId,
            characteristic = characteristic,
            testName = $"{characteristic} Test"
        };

        var json = JsonSerializer.Serialize(rollData);
        var headers = new[] { "Content-Type: application/json" };
        
        _httpRequest.Request("http://localhost:5000/dice/character-roll", headers, HttpClient.Method.Post, json);
    }

    private void OnSavePressed()
    {
        if (_currentCharacterId == 0) return;
        
        var characterData = new
        {
            id = _currentCharacterId,
            userId = _currentUserId,
            name = _nameInput.Text,
            species = _speciesInput.Text,
            career = _careerInput.Text,
            weaponSkill = (int)_wsInput.Value,
            ballisticSkill = (int)_bsInput.Value,
            strength = (int)_sInput.Value,
            toughness = (int)_tInput.Value,
            initiative = (int)_iInput.Value,
            agility = (int)_agInput.Value,
            dexterity = (int)_dexInput.Value,
            intelligence = (int)_intInput.Value,
            willpower = (int)_wpInput.Value,
            fellowship = (int)_felInput.Value,
            wounds = (int)_woundsInput.Value,
            currentWounds = (int)_currentWoundsInput.Value,
            movement = (int)_movementInput.Value,
            status = _statusInput.Text,
            currentExp = (int)_currentExpInput.Value,
            spentExp = (int)_spentExpInput.Value,
            fate = (int)_fateInput.Value,
            fortune = (int)_fortuneInput.Value
        };

        var json = JsonSerializer.Serialize(characterData);
        var headers = new[] { "Content-Type: application/json" };
        
        _httpRequest.Request($"http://localhost:5000/character/{_currentCharacterId}", headers, HttpClient.Method.Put, json);
    }

    private void OnNewCharacterPressed()
    {
        // Create new character with default values
        var characterData = new
        {
            userId = _currentUserId,
            name = "New Character",
            species = "Human",
            career = "Peasant",
            weaponSkill = 20,
            ballisticSkill = 20,
            strength = 20,
            toughness = 20,
            initiative = 20,
            agility = 20,
            dexterity = 20,
            intelligence = 20,
            willpower = 20,
            fellowship = 20,
            wounds = 10,
            currentWounds = 10,
            movement = 4,
            status = "Bronze",
            currentExp = 0,
            spentExp = 0,
            fate = 1,
            fortune = 1
        };

        var json = JsonSerializer.Serialize(characterData);
        var headers = new[] { "Content-Type: application/json" };
        
        _httpRequest.Request("http://localhost:5000/character", headers, HttpClient.Method.Post, json);
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var responseText = Encoding.UTF8.GetString(body);
        
        try
        {
            if (responseCode == 200 || responseCode == 201)
            {
                var jsonDoc = JsonDocument.Parse(responseText);
                
                // Handle different response types based on the request
                if (jsonDoc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    // Character list response
                    PopulateCharacterList(jsonDoc.RootElement);
                }
                else if (jsonDoc.RootElement.TryGetProperty("id", out _))
                {
                    // Single character response
                    PopulateCharacterForm(jsonDoc.RootElement);
                }
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr("Character sheet response error: ", ex.Message);
        }
    }

    private void PopulateCharacterList(JsonElement charactersArray)
    {
        _characterSelect.Clear();
        
        foreach (var character in charactersArray.EnumerateArray())
        {
            var id = character.GetProperty("id").GetInt32();
            var name = character.GetProperty("name").GetString();
            var species = character.GetProperty("species").GetString();
            
            _characterSelect.AddItem($"{name} ({species})");
            _characterSelect.SetItemMetadata(_characterSelect.GetItemCount() - 1, id);
        }
        
        if (_characterSelect.GetItemCount() > 0)
        {
            _characterSelect.Selected = 0;
            OnCharacterSelected(0);
        }
    }

    private void PopulateCharacterForm(JsonElement character)
    {
        _nameInput.Text = character.GetProperty("name").GetString() ?? "";
        _speciesInput.Text = character.GetProperty("species").GetString() ?? "";
        _careerInput.Text = character.GetProperty("career").GetString() ?? "";
        
        _wsInput.Value = character.GetProperty("weaponSkill").GetInt32();
        _bsInput.Value = character.GetProperty("ballisticSkill").GetInt32();
        _sInput.Value = character.GetProperty("strength").GetInt32();
        _tInput.Value = character.GetProperty("toughness").GetInt32();
        _iInput.Value = character.GetProperty("initiative").GetInt32();
        _agInput.Value = character.GetProperty("agility").GetInt32();
        _dexInput.Value = character.GetProperty("dexterity").GetInt32();
        _intInput.Value = character.GetProperty("intelligence").GetInt32();
        _wpInput.Value = character.GetProperty("willpower").GetInt32();
        _felInput.Value = character.GetProperty("fellowship").GetInt32();
        
        _woundsInput.Value = character.GetProperty("wounds").GetInt32();
        _currentWoundsInput.Value = character.GetProperty("currentWounds").GetInt32();
        _movementInput.Value = character.GetProperty("movement").GetInt32();
        
        _statusInput.Text = character.GetProperty("status").GetString() ?? "";
        _currentExpInput.Value = character.GetProperty("currentExp").GetInt32();
        _spentExpInput.Value = character.GetProperty("spentExp").GetInt32();
        _fateInput.Value = character.GetProperty("fate").GetInt32();
        _fortuneInput.Value = character.GetProperty("fortune").GetInt32();
        
        _currentCharacterId = character.GetProperty("id").GetInt32();
    }
}