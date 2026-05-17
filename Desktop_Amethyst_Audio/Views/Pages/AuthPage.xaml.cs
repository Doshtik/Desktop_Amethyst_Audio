using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class AuthPage : Page
{
    private ISettingsService _settingsService = new SettingsService();
    
    private readonly IAuthApiClient _authApiClient = new AuthApiClient();
    public AuthPage()
    {
        InitializeComponent();
    }

    public void NavigateToRegister(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new NavigateToRegisterMessage());
    }

    private void AuthChangeThemeButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void AuthChangeLanguageButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void AuthEnterButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!IsFieldsValidated(out string errorMessage))
            {
                ErrorTextBlock.Text = errorMessage;
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }

            var loginDto = new LoginDto
            {
                Email = AuthEmailTextBox.Text.Trim(), 
                Password = AuthPasswordBox.Password.Trim()
            };

            var userDto = await _authApiClient.LoginUserAsync(loginDto);
        
            Debug.WriteLine($"[DEBUG] userDto is null: {userDto == null}");
            if (userDto is null)
            {
                MessageBox.Show("Пользователь не найден", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AppSettings settings = _settingsService?.Load() ?? new AppSettings();
            settings.User = userDto;
        
            _settingsService.Save(settings);
            
            ErrorBorder.Visibility = Visibility.Collapsed;
            WeakReferenceMessenger.Default.Send(new NavigateToMainLayoutMessage());
        }
        catch (Exception ex)
        {
            ErrorTextBlock.Text = $"Ошибка: {ex.Message}";
            ErrorBorder.Visibility = Visibility.Visible;
        
            Debug.WriteLine($"[EXCEPTION] {ex}");
            if (ex.InnerException != null)
                Debug.WriteLine($"[INNER] {ex.InnerException}");
        }
    }
    
    private bool IsFieldsValidated(out string errorMessage)
    {
        errorMessage = "Ошибка:\n";
        
        string emailPattern = @"^(?:[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)$";
        
        Regex emailRegex = new Regex(emailPattern);

        if (!emailRegex.IsMatch(AuthEmailTextBox.Text.Trim()))
        {
            errorMessage += "Неправильный формат email\n";
        }

        return true;
    }
}