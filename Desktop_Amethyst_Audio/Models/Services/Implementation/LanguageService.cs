using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class LanguageService
{
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