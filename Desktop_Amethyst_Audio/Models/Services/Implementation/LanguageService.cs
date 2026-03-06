using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class LanguageService
{
    private static readonly string[] THEMENAME = new string[] 
    {
        "Dark", "Light"
    };

    private static readonly string[] CULTURECODE = new string[]
    {
        "ru-RU",
        "en-US"
    };

    private static readonly string[] CULTURECODEBUTTONCONTENT = new string[]
    {
        "Ru",
        "En"
    };

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
    
    public void ChangeLanguage(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
    
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));
    }
}