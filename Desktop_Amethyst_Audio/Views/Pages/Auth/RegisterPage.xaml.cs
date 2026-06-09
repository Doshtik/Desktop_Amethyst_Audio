using System.Diagnostics;
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

public partial class RegisterPage : Page
{
    private IAuthApiClient _authApiClient;
    private ISettingsService _settingsService;

    public RegisterPage()
    {
        InitializeComponent();
        _authApiClient = new AuthApiClient();
        _settingsService = new SettingsService();
    }

    private void NavigateToAuth(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new NavigateToAuthMessage());
    }

    private void ChangeThemeButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ChangeLanguageButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void RegisterButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var createUserDto = new CreateUserDto()
            {
                Nickname = NicknameTextBox.Text.Trim(), 
                Email = EmailTextBox.Text.Trim(), 
                Password = RegisterPasswordBox.Password.Trim()
            };

            var userDto = await _authApiClient.RegisterAsync(createUserDto);

            if (userDto is null)
            {
                MessageBox.Show("Ошибка пользователя");
            }

            AppSettings settings = _settingsService?.Load() ?? new AppSettings();
            if (settings == null)
                throw new InvalidOperationException("Не удалось загрузить настройки приложения");

            settings.User = userDto;
        
            _settingsService.Save(settings);
            
            ErrorFieldBorder.Visibility = Visibility.Collapsed;
            WeakReferenceMessenger.Default.Send(new NavigateToMainLayoutMessage());
        }
        catch (Exception ex)
        {
            ErrorFieldTextBlock.Text = $"Ошибка: {ex.Message}";
            ErrorFieldBorder.Visibility = Visibility.Visible;
        
            Debug.WriteLine($"[EXCEPTION] {ex}");
            if (ex.InnerException != null)
                Debug.WriteLine($"[INNER] {ex.InnerException}");
        }
    }
}