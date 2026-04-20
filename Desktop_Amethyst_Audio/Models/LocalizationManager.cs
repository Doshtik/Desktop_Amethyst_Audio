using System.ComponentModel;

namespace Desktop_Amethyst_Audio.Models;

public class LocalizationManager
{
    public static event PropertyChangedEventHandler PropertyChanged;

    public static void NotifyLanguageChanged()
    {
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(string.Empty));
    }
}