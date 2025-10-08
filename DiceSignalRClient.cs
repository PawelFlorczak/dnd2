using Godot;
using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using DiceAPI.Models; // Jeśli używasz klasy DiceRoll
using DiceAPI.Hubs;

public partial class DiceSignalRClient : Node
{
    [Export] public string ServerUrl { get; set; } = "http://localhost:5254/DiceHub";

    private HubConnection _connection;

    [Signal]
    public delegate void RollReceivedEventHandler(string player, int result, int sides, string timestamp);

    public override async void _Ready()
    {
        GD.Print("Łączenie z SignalR...");

        _connection = new HubConnectionBuilder()
            .WithUrl(ServerUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<DiceRoll>("ReceiveRoll", (roll) =>
        {
            // Używamy CallDeferred, żeby przenieść wywołanie na główny wątek Godota
            CallDeferred(nameof(EmitSignal), nameof(RollReceived), roll.PlayerName, roll.Result, roll.Sides, roll.Timestamp.ToString("HH:mm:ss"));
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

    public async Task SendRollAsync(string player, int result, int sides)
    {
        if (_connection == null || _connection.State != HubConnectionState.Connected)
        {
            GD.PrintErr("SignalR nie jest połączony!");
            return;
        }

        var roll = new DiceRoll
        {
            PlayerName = player,
            Result = result,
            Sides = sides,
            Timestamp = DateTime.UtcNow
        };

        await _connection.InvokeAsync("SendRoll", roll);
    }
}