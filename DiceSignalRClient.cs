using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

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

        _connection.On<string>("ReceiveRoll", (msg) =>
        {
            GD.Print("Otrzymano rzut: ", msg);
            EmitSignal(nameof(RollReceived), msg);
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

    public async Task SendRoll(string player, int sides, int result)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("BroadcastRoll", player, sides, result);
        }
    }
}