using System.Globalization;
using System.Resources;
using System.Windows.Markup;
using Desktop_Amethyst_Audio.Resources.Locales;

namespace Desktop_Amethyst_Audio.Resources.Locales;

public class LocalizationExtension : MarkupExtension
{
    public string Key { get; set; }
    
    public LocalizationExtension(string key)
    {
        Key = key;
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new LocalizationExtension(Key);
        /*
         * return Resources.ResourceManager.GetString(Key, 
            CultureInfo.CurrentUICulture) ?? Key;
         */
    }
}