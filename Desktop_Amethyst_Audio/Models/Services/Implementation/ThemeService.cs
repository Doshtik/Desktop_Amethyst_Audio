using System.Collections.ObjectModel;
using System.Windows;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class ThemeService
{
    public void ChangeTheme(string themeName)
    {
        Uri uri = new Uri($"Themes/Theme.{themeName}.xaml", UriKind.Relative);
        ResourceDictionary newTheme = new ResourceDictionary { Source = uri };
        Collection<ResourceDictionary> dictionaries = Application.Current.Resources.MergedDictionaries;
        for (int i = 0; i < dictionaries.Count; i++)
        {
            if (dictionaries[i].Source.OriginalString.Contains("Theme."))
            {
                dictionaries.RemoveAt(i);
                dictionaries.Insert(i, newTheme);
                return;
            }
        }
        dictionaries.Add(newTheme);
    }
}