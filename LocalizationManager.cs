using Godot;

public partial class LocalizationManager : Node
{
    public static LocalizationManager Instance { get; private set; }
    
    public enum Language
    {
        English,
        Polish
    }
    
    private Language _currentLanguage = Language.English;
    
    [Signal]
    public delegate void LanguageChangedEventHandler();
    
    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
            // Load saved language preference
            LoadLanguagePreference();
        }
        else
        {
            QueueFree();
        }
    }
    
    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                ApplyLanguage();
                SaveLanguagePreference();
                EmitSignal(SignalName.LanguageChanged);
            }
        }
    }
    
    private void ApplyLanguage()
    {
        string locale = _currentLanguage switch
        {
            Language.English => "en",
            Language.Polish => "pl",
            _ => "en"
        };
        
        TranslationServer.SetLocale(locale);
    }
    
    private void SaveLanguagePreference()
    {
        var config = new ConfigFile();
        config.SetValue("settings", "language", (int)_currentLanguage);
        config.Save("user://settings.cfg");
    }
    
    private void LoadLanguagePreference()
    {
        var config = new ConfigFile();
        if (config.Load("user://settings.cfg") == Error.Ok)
        {
            var languageValue = config.GetValue("settings", "language", (int)Language.English);
            _currentLanguage = (Language)(int)languageValue;
            ApplyLanguage();
        }
    }
    
    public string GetText(string key)
    {
        return Tr(key);
    }
    
    public void ToggleLanguage()
    {
        CurrentLanguage = _currentLanguage == Language.English ? Language.Polish : Language.English;
    }
}