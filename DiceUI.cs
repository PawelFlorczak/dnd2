using Godot;
using System;

public partial class DiceUI : Control
{
    private VBoxContainer _container;
    private ScrollContainer _scrollContainer;
    private DiceSignalRClient _signalrClient;

    public override void _Ready()
    {
        // Get the existing nodes from the scene structure you created
        _scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
        _container = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
        
        _signalrClient = GetNode<DiceSignalRClient>("../DiceSignalRClient");

        if (_signalrClient != null)
        {
            _signalrClient.Connect("OnRollReceived", new Callable(this, nameof(RollReceived)));
            GD.Print("Połączono sygnał OnRollReceived!");
        }
        else
        {
            GD.PrintErr("Nie znaleziono DiceSignalRClient!");
        }
    }

    public void RollReceived(string player, int result, int sides, string timestamp)
    {
        var label = new Label();
        label.Text = $"{timestamp} — {player} rzucił {result}/{sides}";
        _container.AddChild(label);
        
        // Auto-scroll to bottom to show newest roll
        CallDeferred(nameof(ScrollToBottom));
    }
    
    private void ScrollToBottom()
    {
        _scrollContainer.ScrollVertical = (int)_scrollContainer.GetVScrollBar().MaxValue;
    }
}