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
    
    // ==========================================
    // ===== Character Form =====================
    
    // Character Basic Info controls
    [ExportGroup("Character Form")]
    [ExportSubgroup("Basic Info")]
    [Export]
    private LineEdit 
        _nameInput,
        _speciesInput,
        _classInput,
        _careerInput,
        _careerLevelInput,
        _careerPathInput,
        _statusInput,
        _ageInput,
        _heightInput,
        _hairInput, 
        _eyesInput;
    
    // Characteristics
    [ExportSubgroup("Characteristics")]
    [Export]
    private SpinBox _wsInput, _bsInput, _sInput, _tInput, _iInput, _agInput, _dexInput, _intInput, _wpInput, _felInput;
    [Export]
    private SpinBox _wsInputAdv, _bsInputAdv, _sInputAdv, _tInputAdv, _iInputAdv, _agInputAdv, _dexInputAdv, _intInputAdv, _wpInputAdv, _felInputAdv;
    [Export]
    private SpinBox _wsInputMod, _bsInputMod, _sInputMod, _tInputMod, _iInputMod, _agInputMod, _dexInputMod, _intInputMod, _wpInputMod, _felInputMod;
    [Export]
    private Label _wsInputCur, _bsInputCur, _sInputCur, _tInputCur, _iInputCur, _agInputCur, _dexInputCur, _intInputCur, _wpInputCur, _felInputCur;   
    
    // Roll buttons for characteristics
    private Dictionary<string, Button> _rollButtons = new();
    
    // Resilience
    [ExportSubgroup("Resilience")]
    [Export]
    private SpinBox _resilienceInput, _resolveInput, _motivationInput;
    
    //Experience points
    [ExportSubgroup("Experience")]
    [Export]
    private SpinBox _currentExpInput, _spentExpInput, _totalExpInput;
    
    // Fate
    [ExportSubgroup("Fate")]
    [Export]
    private SpinBox _fateInput, _fortuneInput;
    
    // Movement
    [ExportSubgroup("movement")]
    [Export]
    private SpinBox _movementInput, _walkInput, _runInput;
    
    // Basic Skills
    [ExportSubgroup("Basic Skills")]
    [Export]
    private Label 
        _skillBaseCharLabelArt,
        _skillBaseCharLabelAthletics,
        _skillBaseCharLabelBribery,
        _skillBaseCharLabelCharm,
        _skillBaseCharLabelCharmAnimal,
        _skillBaseCharLabelClimb,
        _skillBaseCharLabelCool,
        _skillBaseCharLabelConsumeAlcohol,
        _skillBaseCharLabelDodge,
        _skillBaseCharLabelDrive,
        _skillBaseCharLabelEndurance,
        _skillBaseCharLabelEntertain,
        _skillBaseCharLabelGamble,
        _skillBaseCharLabelGossip,
        _skillBaseCharLabelHaggle,
        _skillBaseCharLabelIntimidate,
        _skillBaseCharLabelIntuition,
        _skillBaseCharLabelLeadership,
        _skillBaseCharLabelMeleeBasic,
        _skillBaseCharLabelMelee,
        _skillBaseCharLabelNavigation,
        _skillBaseCharLabelOutdoorSurvival,
        _skillBaseCharLabelPerception,
        _skillBaseCharLabelRide,
        _skillBaseCharLabelRow,
        _skillBaseCharLabelStealth;

    [Export]
    private SpinBox 
        _skillAdvEditArt,            
        _skillAdvEditAthletics,      
        _skillAdvEditBribery,        
        _skillAdvEditCharm,          
        _skillAdvEditCharmAnimal,    
        _skillAdvEditClimb,          
        _skillAdvEditCool,           
        _skillAdvEditConsumeAlcohol, 
        _skillAdvEditDodge,          
        _skillAdvEditDrive,          
        _skillAdvEditEndurance,      
        _skillAdvEditEntertain,      
        _skillAdvEditGamble,         
        _skillAdvEditGossip,         
        _skillAdvEditHaggle,         
        _skillAdvEditIntimidate,     
        _skillAdvEditIntuition,      
        _skillAdvEditLeadership,     
        _skillAdvEditMeleeBasic,     
        _skillAdvEditMelee,          
        _skillAdvEditNavigation,     
        _skillAdvEditOutdoorSurvival,
        _skillAdvEditPerception,     
        _skillAdvEditRide,           
        _skillAdvEditRow,            
        _skillAdvEditStealth;
    
    [Export]
    private Label 
        _skillSumLabelArt,
        _skillSumLabelAthletics,
        _skillSumLabelBribery,
        _skillSumLabelCharm,
        _skillSumLabelCharmAnimal,
        _skillSumLabelClimb,
        _skillSumLabelCool,
        _skillSumLabelConsumeAlcohol,
        _skillSumLabelDodge,
        _skillSumLabelDrive,
        _skillSumLabelEndurance,
        _skillSumLabelEntertain,
        _skillSumLabelGamble,
        _skillSumLabelGossip,
        _skillSumLabelHaggle,
        _skillSumLabelIntimidate,
        _skillSumLabelIntuition,
        _skillSumLabelLeadership,
        _skillSumLabelMeleeBasic,
        _skillSumLabelMelee,
        _skillSumLabelNavigation,
        _skillSumLabelOutdoorSurvival,
        _skillSumLabelPerception,
        _skillSumLabelRide,
        _skillSumLabelRow,
        _skillSumLabelStealth;    
    
    
    private Dictionary<string, Button> _basicSkillsButtons1 = new();
    private Dictionary<string, Button> _basicSkillsButtons2 = new();
    
    // Grouped & Advanced Skills
    [ExportSubgroup("Grouped & Advanced Skills")]
    [Export]
    private TextEdit _advNameEdit, _advCharacteristicEdit, _advCharacteristicBaseEdit, _advCharacteristicAdvEdit, _advSkillEdit;
    
    // Talents
    [ExportSubgroup("Talents")]
    [Export]
    private TextEdit _talentNameEdit, _talentTimesTakenEdit, _talentDescriptionEdit;
    
    // Ambitions
    [ExportSubgroup("Ambitions")]
    [Export]
    private TextEdit _shortAmbitionEdit, _longAmbitionEdit;
    
    // Party
    [ExportSubgroup("Party")]
    [Export]
    private LineEdit _partyNameEdit;
    [Export]
    private TextEdit _partyShortAmbitionEdit, _partyLongAmbitionEdit, _partyMembersEdit;
    
    // ==========================================
    // ====== Equipment Form ====================
    
    // Armour
    [ExportGroup("Equipment Form")]
    [ExportSubgroup("Armour")]
    [Export]
    private TextEdit 
        _armourNameEdit,
        _armourLocationEdit,
        _armourEncEdit,
        _armourAPEdit,
        _armourQualitiesEdit;
    
    
    // Armour Points
    [ExportSubgroup("Armour Points")]
    [Export]
    private SpinBox 
        _headAPEdit,
        _leftArmAPEdit,
        _rightArmAPEdit,
        _bodyAPEdit,
        _rightLegAPEdit,
        _leftLegAPEdit,
        _shiledAPEdit;
    
    // Trappings
    [ExportSubgroup("Trappings")]
    [Export]
    private TextEdit _trappingNameEdit, _trappingEncEdit;
    
    // Psychology
    [ExportSubgroup("Psychology")]
    [Export]
    private TextEdit _psychologyTextEdit;
    
    // Corruption & Mutation
    [ExportSubgroup("Corruption & Mutation")]
    [Export]
    private TextEdit _corruptionMutationTextEdit;
    
    // Wealth
    [ExportSubgroup("Wealth")]
    [Export]
    private SpinBox _wealthInputD, _wealthInputSS, _wealthInputGC;
    
    // Encumbrance
    [ExportSubgroup("Encumbrance")]
    [Export]
    private SpinBox 
        _encumbranceInputWeapons,
        _encumbranceInputArmour,
        _encumbranceInputTrappings,
        _encumbranceInputMaxEnc,
        _encumbranceInputTotal;
    
    // Wounds
    [ExportSubgroup(("Wounds"))]
    [Export]
    private SpinBox _woundsInputSB, _woundsInputTB, _woundsInputWPB, _woundsInputHardy, _woundsInputCurrent;
    private TextEdit _woundsMiscTextEdit;
    
    // Weapons
    [ExportSubgroup("Weapons")]
    [Export]
    private TextEdit 
        _weaponsNameEdit,
        _weaponsGroupEdit,
        _weaponsEncEdit,
        _weaponsRangeEdit,
        _weaponsDamageEdit,
        _weaponsQualitiesEdit;
    
    // Spells & Prayers
    [ExportSubgroup("Spells & Prayers")] [Export]
    private TextEdit
        _spellsPrayersNameEdit,
        _spellsPrayersTNEdit,
        _spellsPrayersRangeEdit,
        _spellsPrayersTargetEdit,
        _spellsPrayersDurationEdit,
        _spellsPrayersEffectEdit;
    [Export]
    private LineEdit _spellsPrayersSinEdit;
    
    
    // ===== Form End =====
    // ====================
    
    public override void _Ready()
    {
        SetupUI();
        SetupBasicSkillsButtons();
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
        // Setup characteristic bindings for live updates
        SetupCharacteristicBindings();
    }

    private void SetupCharacteristicBindings()
    {
        // WS bindings
        _wsInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("WS");
        _wsInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("WS");
        _wsInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("WS");
        
        // BS bindings
        _bsInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("BS");
        _bsInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("BS");
        _bsInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("BS");
        
        // S bindings
        _sInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("S");
        _sInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("S");
        _sInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("S");
        
        // T bindings
        _tInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("T");
        _tInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("T");
        _tInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("T");
        
        // I bindings
        _iInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("I");
        _iInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("I");
        _iInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("I");
        
        // AG bindings
        _agInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("AG");
        _agInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("AG");
        _agInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("AG");
        
        // DEX bindings
        _dexInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("DEX");
        _dexInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("DEX");
        _dexInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("DEX");
        
        // INT bindings
        _intInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("INT");
        _intInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("INT");
        _intInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("INT");
        
        // WP bindings
        _wpInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("WP");
        _wpInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("WP");
        _wpInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("WP");
        
        // FEL bindings
        _felInput.ValueChanged += (double value) => UpdateCurrentCharacteristic("FEL");
        _felInputAdv.ValueChanged += (double value) => UpdateCurrentCharacteristic("FEL");
        _felInputMod.ValueChanged += (double value) => UpdateCurrentCharacteristic("FEL");
    }

    private void UpdateCurrentCharacteristic(string characteristic)
    {
        switch (characteristic)
        {
            case "WS":
                _wsInputCur.Text = ((int)(_wsInput.Value + _wsInputAdv.Value + _wsInputMod.Value)).ToString();  
                break;
            case "BS":
                _bsInputCur.Text = ((int)(_bsInput.Value + _bsInputAdv.Value + _bsInputMod.Value)).ToString();
                break;
            case "S":
                _sInputCur.Text = ((int)(_sInput.Value + _sInputAdv.Value + _sInputMod.Value)).ToString();
                break;
            case "T":
                _tInputCur.Text = ((int)(_tInput.Value + _tInputAdv.Value + _tInputMod.Value)).ToString();
                break;
            case "I":
                _iInputCur.Text = ((int)(_iInput.Value + _iInputAdv.Value + _iInputMod.Value)).ToString();
                break;
            case "AG":
                _agInputCur.Text = ((int)(_agInput.Value + _agInputAdv.Value + _agInputMod.Value)).ToString();
                break;
            case "DEX":
                _dexInputCur.Text = ((int)(_dexInput.Value + _dexInputAdv.Value + _dexInputMod.Value)).ToString();
                break;
            case "INT":
                _intInputCur.Text = ((int)(_intInput.Value + _intInputAdv.Value + _intInputMod.Value)).ToString();
                break;
            case "WP":
                _wpInputCur.Text = ((int)(_wpInput.Value + _wpInputAdv.Value + _wpInputMod.Value)).ToString();
                break;
            case "FEL":
                _felInputCur.Text = ((int)(_felInput.Value + _felInputAdv.Value + _felInputMod.Value)).ToString();
                break;
        }
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
        var skills1 = new[]
        {
            "Art", "Athletics", "Bribery", "Charm", "CharmAnimal", "Climb","Cool",
            "ConsumeAlcohol","Dodge", "Drive", "Endurance", "Entertain", "Gamble"
        };

        var skills2 = new[]
        {
            "Gossip", "Haggle", "Intimidate", "Intuition", "Leadership", "MeleeBasic", "Melee", "Navigation",
            "OutdoorSurvival", "Perception", "Ride", "Row", "Stealth"
        };
        
        foreach (var skill in skills1)
        {
            var button1 = GetNode<Button>($"VBox/TabContainer/Character/TextureCharacter/CharacterForm/BasicSkillsContainer1/HBox{skill}/BasicSkillButton");
            
            _basicSkillsButtons1[skill] = button1;
            button1.Pressed += () => OnBasicSkillRoll(skill);
        }

        foreach (var skill in skills2)
        {
            var button2 = GetNode<Button>($"VBox/TabContainer/Character/TextureCharacter/CharacterForm/BasicSkillsContainer2/HBox{skill}/BasicSkillButton");
            
            _basicSkillsButtons2[skill] = button2;
            button2.Pressed += () => OnBasicSkillRoll(skill);
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
        GD.Print($"{characteristic}");
        if (_currentCharacterId == 0) return;
        
        // Get the current Adv and Mod values for this characteristic
        int advValue = 0;
        int modValue = 0;
        
        switch (characteristic)
        {
            case "WS":
                advValue = (int)_wsInputAdv.Value;
                modValue = (int)_wsInputMod.Value;
                break;
            case "BS":
                advValue = (int)_bsInputAdv.Value;
                modValue = (int)_bsInputMod.Value;
                break;
            case "S":
                advValue = (int)_sInputAdv.Value;
                modValue = (int)_sInputMod.Value;
                break;
            case "T":
                advValue = (int)_tInputAdv.Value;
                modValue = (int)_tInputMod.Value;
                break;
            case "I":
                advValue = (int)_iInputAdv.Value;
                modValue = (int)_iInputMod.Value;
                break;
            case "AG":
                advValue = (int)_agInputAdv.Value;
                modValue = (int)_agInputMod.Value;
                break;
            case "DEX":
                advValue = (int)_dexInputAdv.Value;
                modValue = (int)_dexInputMod.Value;
                break;
            case "INT":
                advValue = (int)_intInputAdv.Value;
                modValue = (int)_intInputMod.Value;
                break;
            case "WP":
                advValue = (int)_wpInputAdv.Value;
                modValue = (int)_wpInputMod.Value;
                break;
            case "FEL":
                advValue = (int)_felInputAdv.Value;
                modValue = (int)_felInputMod.Value;
                break;
        }
        
        var rollData = new
        {
            characterId = _currentCharacterId,
            characteristic = characteristic,
            characteristicAdv = advValue,
            characteristicMod = modValue,
            testName = $"{characteristic} Test"
        };

        var json = JsonSerializer.Serialize(rollData);
        var headers = new[] { "Content-Type: application/json" };
        
        _httpRequest.Request("http://localhost:5000/dice/character-roll", headers, HttpClient.Method.Post, json);
    }

    private void OnBasicSkillRoll(string skill)
    {
        GD.Print($"{skill}");
        if (_currentCharacterId == 0) return;
        
        var rollData = new
        {
            characterId = _currentCharacterId,
            skill = skill,
            testName = $"{skill} Test"
        };

        var json = JsonSerializer.Serialize(rollData);
        var headers = new[] { "Content-Type: application/json" };
        
        _httpRequest.Request("http://localhost:5000/dice/skill-roll", headers, HttpClient.Method.Post, json);
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
            // Base characteristics
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
            // Adv characteristics
            weaponSkillAdv = (int)_wsInputAdv.Value,
            ballisticSkillAdv = (int)_bsInputAdv.Value,
            strengthAdv = (int)_sInputAdv.Value,
            toughnessAdv = (int)_tInputAdv.Value,
            initiativeADv = (int)_iInputAdv.Value,
            agilityAdv = (int)_agInputAdv.Value,
            dexterityAv = (int)_dexInputAdv.Value,
            intelligenceAdv = (int)_intInputAdv.Value,
            willpowerAdv = (int)_wpInputAdv.Value,
            fellowshipADv = (int)_felInputAdv.Value,
            // Mod characteristics
            weaponSkillMod = (int)_wsInputMod.Value,
            ballisticSkillMod = (int)_bsInputMod.Value,
            strengthMod = (int)_sInputMod.Value,
            toughnessMod = (int)_tInputMod.Value,
            initiativeMod = (int)_iInputMod.Value,
            agilityMod = (int)_agInputMod.Value,
            dexterityMod = (int)_dexInputMod.Value,
            intelligenceMod = (int)_intInputMod.Value,
            willpowerMod = (int)_wpInputMod.Value,
            fellowshipMod = (int)_felInputMod.Value,
            // Current characteristics
            weaponSkillCur = (string)_wsInputCur.Text,
            // ballisticSkillCur = (string)_bsInputCur.Text,
            // strengthCur = (int)_sInput.Value,
            // toughnessCur = (int)_tInput.Value,
            // initiativeCur = (int)_iInput.Value,
            // agilityCur = (int)_agInput.Value,
            // dexterityCur = (int)_dexInput.Value,
            // intelligenceCur = (int)_intInput.Value,
            // willpowerCur = (int)_wpInput.Value,
            // fellowshipCur = (int)_felInput.Value,            
            // ---
            currentWounds = (int)_woundsInputCurrent.Value,
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
        
        // Load Adv values for all characteristics
        _wsInputAdv.Value = character.GetProperty("weaponSkillAdv").GetInt32();
        _bsInputAdv.Value = character.GetProperty("ballisticSkillAdv").GetInt32();
        _sInputAdv.Value = character.GetProperty("strengthAdv").GetInt32();
        _tInputAdv.Value = character.GetProperty("toughnessAdv").GetInt32();
        _iInputAdv.Value = character.GetProperty("initiativeADv").GetInt32();
        _agInputAdv.Value = character.GetProperty("agilityAdv").GetInt32();
        _dexInputAdv.Value = character.GetProperty("dexterityAv").GetInt32();
        _intInputAdv.Value = character.GetProperty("intelligenceAdv").GetInt32();
        _wpInputAdv.Value = character.GetProperty("willpowerAdv").GetInt32();
        _felInputAdv.Value = character.GetProperty("fellowshipADv").GetInt32();
        
        // Load Mod values for all characteristics
        _wsInputMod.Value = character.GetProperty("weaponSkillMod").GetInt32();
        _bsInputMod.Value = character.GetProperty("ballisticSkillMod").GetInt32();
        _sInputMod.Value = character.GetProperty("strengthMod").GetInt32();
        _tInputMod.Value = character.GetProperty("toughnessMod").GetInt32();
        _iInputMod.Value = character.GetProperty("initiativeMod").GetInt32();
        _agInputMod.Value = character.GetProperty("agilityMod").GetInt32();
        _dexInputMod.Value = character.GetProperty("dexterityMod").GetInt32();
        _intInputMod.Value = character.GetProperty("intelligenceMod").GetInt32();
        _wpInputMod.Value = character.GetProperty("willpowerMod").GetInt32();
        _felInputMod.Value = character.GetProperty("fellowshipMod").GetInt32();
        
        // Calculate and update all current values
        UpdateCurrentCharacteristic("WS");
        UpdateCurrentCharacteristic("BS");
        UpdateCurrentCharacteristic("S");
        UpdateCurrentCharacteristic("T");
        UpdateCurrentCharacteristic("I");
        UpdateCurrentCharacteristic("AG");
        UpdateCurrentCharacteristic("DEX");
        UpdateCurrentCharacteristic("INT");
        UpdateCurrentCharacteristic("WP");
        UpdateCurrentCharacteristic("FEL");
        
        _woundsInputCurrent.Value = character.GetProperty("currentWounds").GetInt32();
        _movementInput.Value = character.GetProperty("movement").GetInt32();
        
        _statusInput.Text = character.GetProperty("status").GetString() ?? "";
        _currentExpInput.Value = character.GetProperty("currentExp").GetInt32();
        _spentExpInput.Value = character.GetProperty("spentExp").GetInt32();
        _fateInput.Value = character.GetProperty("fate").GetInt32();
        _fortuneInput.Value = character.GetProperty("fortune").GetInt32();
        
        _currentCharacterId = character.GetProperty("id").GetInt32();
    }
}