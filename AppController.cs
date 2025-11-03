using Godot;
using System;

public partial class AppController : Control
{
    private LoginUI _loginUI;
    private Control _gameUI;
    private CharacterSheetUI _characterSheetUI;
    private DiceUI _diceUI;
    private DiceSignalRClient _signalRClient;
    
    private Button _logoutButton;
    private Label _userLabel;
    
    private int _currentUserId;
    private string _currentUsername;

    public override void _Ready()
    {
        SetupUI();
        ShowLoginScreen();
    }

    private void SetupUI()
    {
        // Assuming the scene structure has these nodes
        _loginUI = GetNode<LoginUI>("LoginUI");
        _gameUI = GetNode<Control>("GameUI");
        _diceUI = GetNode<DiceUI>("GameUI/ColorRect/HSplitContainer/VSplitContainer/PanelContainer/DiceRoll/DiceUI");
        _characterSheetUI = GetNode<CharacterSheetUI>("GameUI/ColorRect/HSplitContainer/CharacterSheet/CharacterSheetUI");
        try
        {
            _signalRClient = GetNode<DiceSignalRClient>("DiceSignalRClient");
        }
        catch (Exception)
        {
            GD.PrintErr("Nie znaleziono DiceSignalRClient!");
            _signalRClient = null;
        }
        
        try
        {
            _logoutButton = GetNode<Button>("GameUI/ColorRect/HSplitContainer/VSplitContainer/Header/LogoutButton");
            GD.Print("✅ LogoutButton found!");
        }
        catch (Exception ex)
        {
            GD.PrintErr("❌ LogoutButton not found: ", ex.Message);
        }
        
        try
        {
            _userLabel = GetNode<Label>("GameUI/ColorRect/HSplitContainer/VSplitContainer/Header/UserLabel");
            GD.Print("✅ UserLabel found!");
        }
        catch (Exception ex)
        {
            GD.PrintErr("❌ UserLabel not found: ", ex.Message);
        }
        
        // Connect signals
        _loginUI.Connect("OnLoginSuccess", new Callable(this, nameof(OnLoginSuccess)));
        
        if (_logoutButton != null)
        {
            _logoutButton.Pressed += OnLogoutPressed;
            GD.Print("✅ LogoutButton signal connected!");
        }
        
        // Connect SignalR events for character rolls
        if (_signalRClient != null)
        {
            _signalRClient.Connect("OnCharacterRollReceived", new Callable(this, nameof(OnCharacterRollReceived)));
        }
        
        if (_signalRClient != null)
        {
            _signalRClient.Connect("OnSkillRollReceived", new Callable(this, nameof(OnSkillRollReceived)));
        }
    }

    private void ShowLoginScreen()
    {
        _loginUI.Visible = true;
        _gameUI.Visible = false;
    }

    private void ShowGameScreen()
    {
        _loginUI.Visible = false;
        _gameUI.Visible = true;
        
        if (_userLabel != null)
        {
            _userLabel.Text = $"Welcome, {_currentUsername}!";
        }
        
        // Load user's characters
        _characterSheetUI.LoadUserCharacters(_currentUserId);
    }

    private void OnLoginSuccess(int userId, string username)
    {
        _currentUserId = userId;
        _currentUsername = username;
        ShowGameScreen();
        
        GD.Print($"User logged in: {username} (ID: {userId})");
    }

    private void OnLogoutPressed()
    {
        _currentUserId = 0;
        _currentUsername = "";
        ShowLoginScreen();
        
        GD.Print("User logged out");
    }

    private void OnCharacterRollReceived(Godot.Collections.Dictionary rollResult)
    {
        try
        {
            var roll = rollResult["roll"].AsGodotDictionary();
            var targetNumber = rollResult["targetNumber"].AsInt32();
            var success = rollResult["success"].AsBool();
            var characterName = rollResult["characterName"].AsString();
            var testName = rollResult["testName"].AsString();
            
            var playerName = roll["playerName"].AsString();
            var result = roll["result"].AsInt32();
            var timestamp = roll["timestamp"].AsString();
            
            // Format the message to show success/failure
            var successText = success ? "SUCCESS" : "FAILURE";
            var message = $"{timestamp} — {playerName}: {testName} = {result}/{targetNumber} ({successText})";
            
            // Add to dice UI display
            _diceUI.RollReceived(playerName, result, 100, timestamp);
            
            GD.Print($"Character roll: {message}");
        }
        catch (Exception ex)
        {
            GD.PrintErr("Error processing character roll: ", ex.Message);
        }
    }
    
    private void OnSkillRollReceived(Godot.Collections.Dictionary rollResult)
    {
        try
        {
            var roll = rollResult["roll"].AsGodotDictionary();
            var targetNumber = rollResult["targetNumber"].AsInt32();
            var success = rollResult["success"].AsBool();
            var characterName = rollResult["characterName"].AsString();
            var testName = rollResult["testName"].AsString();
            
            var playerName = roll["playerName"].AsString();
            var result = roll["result"].AsInt32();
            var timestamp = roll["timestamp"].AsString();
            
            // Format the message to show success/failure
            var successText = success ? "SUCCESS" : "FAILURE";
            var message = $"{timestamp} — {playerName}: {testName} = {result}/{targetNumber} ({successText})";
            
            // Add to dice UI display
            _diceUI.RollReceived(playerName, result, 100, timestamp);
            
            GD.Print($"Character roll: {message}");
        }
        catch (Exception ex)
        {
            GD.PrintErr("Error processing character roll: ", ex.Message);
        }
    }
}