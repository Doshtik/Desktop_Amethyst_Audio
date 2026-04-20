using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Enums;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class AuthPageViewModel : ObservableObject
{
    [ObservableProperty] private string _emailField;
    [ObservableProperty] private string _errorField;
    [ObservableProperty] private Visibility _errorVisibility;

    [ObservableProperty] private string _currentLanguage;
    [ObservableProperty] private AppTheme _currentTheme;
    
    private ISettingsService _settingsService;
    
    public string LanguageButtonText => CurrentLanguage == "ru-RU" ? "Ru" : "En";
    public string ThemeIconPath => CurrentTheme == AppTheme.Dark 
        ? "pack://application:,,,/Assets/sun_icon.png"
        : "pack://application:,,,/Assets/moon_icon.png";

    
    private readonly Action _navigateToRegister;
    private readonly IAuthService _authService = new AuthService();
    
    public AuthPageViewModel(Action navigateToRegister)
    {
        _navigateToRegister = navigateToRegister;
        _emailField = String.Empty;
        _errorField = String.Empty;
        _errorVisibility = Visibility.Collapsed;
        _settingsService = new SettingsService();
        OnLoad();
    }

    private void OnLoad()
    {
        AppSettings settings = _settingsService.Load();
        
        CurrentLanguage = settings.Language;
        CurrentTheme = settings.Theme;

        ApplyTheme(CurrentTheme);
        ApplyLanguage(CurrentLanguage);
        
        OnPropertyChanged(nameof(ThemeIconPath));
        OnPropertyChanged(nameof(LanguageButtonText));
    }
    
    [RelayCommand]
    private void OnChangeTheme()
    {
        CurrentTheme = CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        ApplyTheme(CurrentTheme);
        OnPropertyChanged(nameof(ThemeIconPath)); // Обновляем иконку кнопки
        SaveSettings();
    }

    [RelayCommand]
    private void OnChangeLanguage()
    {
        CurrentLanguage = CurrentLanguage == "ru-RU" ? "en-US" : "ru-RU";
        ApplyLanguage(CurrentLanguage);
        OnPropertyChanged(nameof(LanguageButtonText)); // Обновляем текст кнопки
        LocalizationManager.NotifyLanguageChanged();   // Глобальное обновление всех привязок
        SaveSettings();
    }

    private void ApplyTheme(AppTheme theme)
    {
        string themeFileName = theme == AppTheme.Light ? "Themes.Light.xaml" : "Themes.Dark.xaml";
        Uri uri = new Uri($"pack://application:,,,/Desktop_Amethyst_Audio;component/Resources/Themes/{themeFileName}", UriKind.Absolute);
    
        Collection<ResourceDictionary> dicts = Application.Current.Resources.MergedDictionaries;
    
        List<ResourceDictionary> themesToRemove = dicts
            .Where(d => d.Source?.OriginalString.Contains("Themes.", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();
    
        foreach (var themeDict in themesToRemove)
        {
            dicts.Remove(themeDict);
        }
        
        dicts.Add(new ResourceDictionary { Source = uri });
    }

    private void ApplyLanguage(string language)
    {
        var culture = CultureInfo.GetCultureInfo(language);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    private void SaveSettings()
    {
        var settings = _settingsService.Load();
        settings.Theme = CurrentTheme;
        settings.Language = CurrentLanguage;
        _settingsService.Save(settings);
    }
    
    [RelayCommand]
    private void NavigateToRegister() => _navigateToRegister?.Invoke();

    [RelayCommand]
    private void EnterButtonOnClick(object password)
    {
        PasswordBox pb = password as PasswordBox;
        
        if (!IsFieldsValidated(pb.Password, out string errorMessage))
        {
            ErrorField = errorMessage;
            ErrorVisibility = Visibility.Visible;
            return;
        }
    }

    [RelayCommand]
    private void ExternalLoginButtonOnClick(ExternalLoginTypeEnum type)
    {
        switch (type)
        {
            case ExternalLoginTypeEnum.ByGoogle:
                MessageBox.Show("Google Login");
                break;
            case ExternalLoginTypeEnum.ByYandex:
                MessageBox.Show("Yandex Login");
                break;
        }
    }
    
    private bool IsFieldsValidated(string password, out string errorMessage)
    {
        errorMessage = "Ошибка: ";
        
        string emailPattern = @"^(?:[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)$";
        string passwordPattern = @"^[A-Za-z0-9_-]+$";
        
        Regex emailRegex = new Regex(emailPattern);
        Regex passwordRegex = new Regex(passwordPattern);

        if (!emailRegex.IsMatch(EmailField))
        {
            errorMessage += "Неправильный email\n";
        }
        
        return true;
    }
}