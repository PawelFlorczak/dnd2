using Godot;
using System;
using System.Text;
using System.Text.Json;

public partial class LoginUI : Control
{
    private LineEdit _usernameInput;
    private LineEdit _passwordInput;
    private LineEdit _emailInput;
    private Button _loginButton;
    private Button _registerButton;
    private Button _toggleButton;
    private Label _statusLabel;
    private VBoxContainer _registerContainer;
    private HttpRequest _httpRequest;
    
    private bool _isRegisterMode = false;
    
    [Signal]
    public delegate void OnLoginSuccessEventHandler(int userId, string username);

    public override void _Ready()
    {
        _usernameInput = GetNode<LineEdit>("CenterContainer/VBox/UsernameInput");
        _passwordInput = GetNode<LineEdit>("CenterContainer/VBox/PasswordInput");
        _emailInput = GetNode<LineEdit>("CenterContainer/VBox/RegisterContainer/EmailInput");
        _loginButton = GetNode<Button>("CenterContainer/VBox/LoginButton");
        _registerButton = GetNode<Button>("CenterContainer/VBox/RegisterButton");
        _toggleButton = GetNode<Button>("CenterContainer/VBox/ToggleButton");
        _statusLabel = GetNode<Label>("CenterContainer/VBox/StatusLabel");
        _registerContainer = GetNode<VBoxContainer>("CenterContainer/VBox/RegisterContainer");
        _httpRequest = GetNode<HttpRequest>("HTTPRequest");

        _loginButton.Pressed += OnLoginPressed;
        _registerButton.Pressed += OnRegisterPressed;
        _toggleButton.Pressed += OnTogglePressed;
        _httpRequest.RequestCompleted += OnRequestCompleted;
        
        _passwordInput.Secret = true;
        _registerContainer.Visible = false;
    }

    private void OnTogglePressed()
    {
        _isRegisterMode = !_isRegisterMode;
        _registerContainer.Visible = _isRegisterMode;
        _registerButton.Visible = _isRegisterMode;
        _loginButton.Visible = !_isRegisterMode;
        _toggleButton.Text = _isRegisterMode ? "Switch to Login" : "Switch to Register";
        _statusLabel.Text = "";
    }

    private void OnLoginPressed()
    {
        if (string.IsNullOrWhiteSpace(_usernameInput.Text) || string.IsNullOrWhiteSpace(_passwordInput.Text))
        {
            _statusLabel.Text = "Please enter username and password.";
            return;
        }

        var loginData = new
        {
            username = _usernameInput.Text.Trim(),
            password = _passwordInput.Text
        };

        var json = JsonSerializer.Serialize(loginData);
        var headers = new[] { "Content-Type: application/json" };
        
        _httpRequest.Request("http://localhost:5000/auth/login", headers, HttpClient.Method.Post, json);
        _statusLabel.Text = "Logging in...";
    }

    private void OnRegisterPressed()
    {
        if (string.IsNullOrWhiteSpace(_usernameInput.Text) || 
            string.IsNullOrWhiteSpace(_passwordInput.Text) || 
            string.IsNullOrWhiteSpace(_emailInput.Text))
        {
            _statusLabel.Text = "Please fill all fields.";
            return;
        }

        var registerData = new
        {
            username = _usernameInput.Text.Trim(),
            password = _passwordInput.Text,
            email = _emailInput.Text.Trim()
        };

        var json = JsonSerializer.Serialize(registerData);
        var headers = new[] { "Content-Type: application/json" };
        
        _httpRequest.Request("http://localhost:5000/auth/register", headers, HttpClient.Method.Post, json);
        _statusLabel.Text = "Registering...";
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var responseText = Encoding.UTF8.GetString(body);
        
        try
        {
            var jsonDoc = JsonDocument.Parse(responseText);
            var root = jsonDoc.RootElement;
            
            if (root.TryGetProperty("success", out var successProp) && successProp.GetBoolean())
            {
                _statusLabel.Text = root.GetProperty("message").GetString();
                
                if (root.TryGetProperty("user", out var userProp))
                {
                    var userId = userProp.GetProperty("id").GetInt32();
                    var username = userProp.GetProperty("username").GetString();
                    
                    EmitSignal(SignalName.OnLoginSuccess, userId, username);
                }
            }
            else
            {
                _statusLabel.Text = root.GetProperty("message").GetString();
            }
        }
        catch (Exception ex)
        {
            _statusLabel.Text = "Error processing response: " + ex.Message;
            GD.PrintErr("Login response error: ", ex.Message);
        }
    }
}