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
		
		var signalR = GetNode<DiceSignalRClient>("DiceSignalRClient");
		signalR.Connect("OnRollReceived", new Callable(this, nameof(OnRollReceived)));
		
	}


	public void OnRollReceived(string msg)
	{
		GD.Print("UI dostało rzut: " + msg);
		// tu możesz dodać do historii w Label/ListView
	}
	
	private void OnRollPressed()
	{
		var url = "http://localhost:5000/dice/roll?sides=20"; 
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
