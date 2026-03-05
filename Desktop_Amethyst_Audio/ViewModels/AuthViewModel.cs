using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Desktop_Amethyst_Audio.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _currentPage;
    public PageViewModels.AuthViewModel AuthPageViewModel { get; }
    public PageViewModels.RegisterViewModel RegisterPageViewModel { get; }

    public AuthViewModel()
    {
        _currentPage =  new PageViewModels.AuthViewModel();
    }
    
    public void NavigateToAuth() => CurrentPage = AuthPageViewModel;
    
    public void NavigateToRegister() => CurrentPage = RegisterPageViewModel;
    
    
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