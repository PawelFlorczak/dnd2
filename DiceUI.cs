using Godot;
using System;
using System.Data;

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
        
        // Find DiceSignalRClient more reliably by searching from root
        _signalrClient = GetTree().Root.FindChild("DiceSignalRClient", true, false) as DiceSignalRClient;

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

    public void RollReceived(string player, int result, int sides, string timestamp, string testname)
    {
        string formattedTimestamp = timestamp;
        if (DateTime.TryParse(timestamp, out DateTime parsedTime))
        {
            formattedTimestamp = parsedTime.ToString("HH:mm:ss");
        }
        
        var label = new Label();
        if (String.IsNullOrEmpty(testname))
        {
            label.Text = $"{formattedTimestamp} — {player} rzucił {result}/{sides}";
        }
        else
        {
            label.Text = $"{formattedTimestamp} — {player} rzucił {result}/{sides} w {testname}";
        }
        label.AutowrapMode = TextServer.AutowrapMode.WordSmart;  
        _container.AddChild(label);
        
        // Auto-scroll to bottom to show newest roll
        // Wait for next frame to ensure layout is updated with Clip Contents
        GetTree().ProcessFrame += ScrollToBottomNextFrame;
    }
    
    private void ScrollToBottomNextFrame()
    {
        // Disconnect the signal to avoid multiple calls
        GetTree().ProcessFrame -= ScrollToBottomNextFrame;
        
        // Now scroll to bottom after layout is fully updated
        _scrollContainer.ScrollVertical = (int)_scrollContainer.GetVScrollBar().MaxValue;
    }
}