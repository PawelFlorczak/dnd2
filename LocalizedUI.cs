using Godot;
using System.Collections.Generic;

public partial class LocalizedUI : Control
{
    private Dictionary<string, Control> _localizedControls = new Dictionary<string, Control>();
    
    public override void _Ready()
    {
        // Connect to language change signal
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.LanguageChanged += OnLanguageChanged;
        }
        
        // Initialize localized controls
        RegisterLocalizedControls();
        UpdateTexts();
    }
    
    public override void _ExitTree()
    {
        // Disconnect from language change signal
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.LanguageChanged -= OnLanguageChanged;
        }
    }
    
    protected virtual void RegisterLocalizedControls()
    {
        // Override in derived classes to register controls that need localization
        // Example: RegisterControl("TITLE", GetNode<Label>("Title"));
    }
    
    protected void RegisterControl(string key, Control control)
    {
        _localizedControls[key] = control;
    }
    
    protected virtual void UpdateTexts()
    {
        foreach (var kvp in _localizedControls)
        {
            string key = kvp.Key;
            Control control = kvp.Value;
            
            if (control is Label label)
            {
                label.Text = Tr(key);
            }
            else if (control is Button button)
            {
                button.Text = Tr(key);
            }
            else if (control is LineEdit lineEdit)
            {
                lineEdit.PlaceholderText = Tr(key);
            }
        }
    }
    
    private void OnLanguageChanged()
    {
        UpdateTexts();
    }
}