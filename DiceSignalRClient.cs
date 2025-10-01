using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using DiceAPI.Models;
using System.Text.Json;
using Godot.Collections;

public partial class DiceSignalRClient : Node
{
    private HubConnection _connection;

    public override async void _Ready()
    {
        GD.Print("Łączenie z SignalR...");

        _connection = new HubConnectionBuilder()
            .WithUrl("https://dnd2.onrender.com/DiceHub")
            .WithAutomaticReconnect()
            .Build();

        // _connection.On<DiceRoll>("ReceiveRoll", (msg) =>
        // {
        //     GD.Print("Otrzymano rzut: ", msg);
        //     EmitSignal(nameof(RollReceived), msg);
        // });
        
        _connection.On<DiceRoll>("ReceiveRoll", (roll) =>
        {
            var dict = new Dictionary
            {
                { "id", roll.Id },
                { "playerName", roll.PlayerName },
                { "sides", roll.Sides },
                { "result", roll.Result },
                { "timestamp", roll.Timestamp.ToString("O") }
            };

            EmitSignal(SignalName.RollReceived, dict);
        });
        
        
        try
        {
            await _connection.StartAsync();
            GD.Print("Połączono z SignalR!");
        }
        catch (System.Exception ex)
        {
            GD.PrintErr("Błąd połączenia z SignalR: ", ex.Message);
        }
    }

    [Signal]
    public delegate void RollReceivedEventHandler(string message);

    public async Task SendRoll(string playerName, int sides, int result)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("BroadcastRoll", playerName, sides, result);
        }
    }
}