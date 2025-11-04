using Godot;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using DiceAPI.Models; // Jeśli używasz klasy DiceRoll
using DiceAPI.Hubs;
using DiceAPI;

public partial class DiceSignalRClient : Node
{
    [Export] public string ServerUrl { get; set; } = "http://localhost:5000/DiceHub";

    private HubConnection _connection;

    [Signal]
    public delegate void OnRollReceivedEventHandler(string player, int result, int sides, string timestamp, string testname);
    
    [Signal]
    public delegate void OnCharacterRollReceivedEventHandler(Godot.Collections.Dictionary rollResult);

    [Signal]
    public delegate void OnSkillRollReceivedEventHandler(Godot.Collections.Dictionary skillRollResult);

    public override async void _Ready()
    {
        GD.Print("Łączenie z SignalR...");

        _connection = new HubConnectionBuilder()
            .WithUrl(ServerUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<DiceRoll>("OnRollReceived", (roll) =>
        {
            GD.Print($"🎲 Otrzymano rzut od {roll.PlayerName}: {roll.Result}/{roll.Sides}");
            // Używamy CallDeferred, żeby przenieść wywołanie na główny wątek Godota
            CallDeferred(nameof(EmitSignalDeferred), 
                roll.PlayerName, roll.Result, roll.Sides, roll.Timestamp.ToString("HH:mm:ss"), "");
        });

        _connection.On<object>("OnCharacterRollReceived", (rollResult) =>
        {
            GD.Print($"🎲 Otrzymano rzut postaci: {rollResult}");
            // Convert to Godot dictionary for easier handling in Godot scripts
            var jsonString = rollResult?.ToString() ?? "";
            CallDeferred(nameof(EmitCharacterRollSignalDeferred), jsonString);
        });

        _connection.On<object>("OnSkillRollReceived", (skillRollResult) =>
        {
            GD.Print($"🎲 Otrzymano rzut umiejetnosci: {skillRollResult}");
            var jsonString = skillRollResult?.ToString() ?? "";
            CallDeferred(nameof(EmitSkillRollSignalDeferred), jsonString);
        });
        

        try
        {
            await _connection.StartAsync();
            GD.Print("Połączono z SignalR!");
        }
        catch (Exception ex)
        {
            GD.PrintErr("Błąd połączenia z SignalR: ", ex.Message);
        }
    }

    private void EmitSignalDeferred(string playerName, int result, int sides, string timestamp, string testname)
    {
        EmitSignal(SignalName.OnRollReceived, playerName, result, sides, timestamp, testname);
    }

    private void EmitSkillRollSignalDeferred(string jsonString)
    {
        var json = new Json();
        var parseResult = json.Parse(jsonString);

        if (parseResult == Error.Ok)
        {
            var dict = json.Data.AsGodotDictionary();
            EmitSignal(SignalName.OnSkillRollReceived, dict);
        }
        else
        {
            GD.PrintErr("Failed to parse character roll result");
        }
    }
    
    private void EmitCharacterRollSignalDeferred(string jsonString)
    {
        // Convert the rollResult to a Godot dictionary
        var json = new Json();
        var parseResult = json.Parse(jsonString);
        
        if (parseResult == Error.Ok)
        {
            var dict = json.Data.AsGodotDictionary();
            EmitSignal(SignalName.OnCharacterRollReceived, dict);
        }
        else
        {
            GD.PrintErr("Failed to parse character roll result");
        }
    }

}