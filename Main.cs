using Godot;
using System;

public partial class Main : Control
{
    private HttpRequest _http;
    private Label _label;

    public override void _Ready()
    {
        _http = GetNode<HttpRequest>("HTTPRequest");
        _label = GetNode<Label>("Label");

        _http.RequestCompleted += OnRequestCompleted;

        var button = GetNode<Button>("Button");
        button.Pressed += OnRollPressed;
    }

    private void OnRollPressed()
    {
        //var url = "http://localhost:5254/dice/roll?sides=20"; 
        var url = "https://dnd2.onrender.com/dice/roll?sides=20";
        _http.Request(url);
    }

    private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var json = new Json();
        var jsonString = System.Text.Encoding.UTF8.GetString(body);
        var err = json.Parse(jsonString);
        if (err == Error.Ok)
        {
            var data = json.Data.AsGodotDictionary();
            var value = data["result"].AsInt32();
            _label.Text = $"Wynik: {value}";
        }
        else
        {
            _label.Text = "Błąd parsowania odpowiedzi.";
        }
    }
}