using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class RegisterPageViewModel: ObservableObject
{
    [ObservableProperty] private string _nicknameField;
    [ObservableProperty] private string _emailField;
    [ObservableProperty] private string _errorField;
    [ObservableProperty] private Visibility _errorVisibility = Visibility.Collapsed;
    
    private ISettingsService _settingsService = new SettingsService();
    
    private readonly IAuthApiClient _authApiClient = new AuthApiClient();
    
    public RegisterPageViewModel()
    {
        
    }
    
    [RelayCommand]
    private void NavigateToAuth() 
        => WeakReferenceMessenger.Default.Send(new NavigateToAuthMessage());

    [RelayCommand]
    private async void RegisterButtonOnClick(object password)
    {
        try
        {
            if (password is not PasswordBox pb)
                throw new InvalidOperationException("CommandParameter не является PasswordBox");

            var createUserDto = new CreateUserDto() { Nickname = NicknameField, Email = EmailField, Password = pb.Password };

            var userDto = await _authApiClient.RegisterAsync(createUserDto);
        
            Debug.WriteLine($"[DEBUG] userDto is null: {userDto == null}");
            if (userDto == null)
                throw new InvalidOperationException("Сервер вернул пустой ответ (десериализация не удалась)");

            AppSettings settings = _settingsService?.Load() ?? new AppSettings();
            Debug.WriteLine($"[DEBUG] settings is null: {settings == null}");
            if (settings == null)
                throw new InvalidOperationException("Не удалось загрузить настройки приложения");

            settings.User = userDto;
        
            _settingsService.Save(settings);
            
            ErrorVisibility = Visibility.Collapsed;
            
            WeakReferenceMessenger.Default.Send(new NavigateToMainLayoutMessage());
        }
        catch (Exception ex)
        {
            ErrorField = $"Ошибка: {ex.Message}";
            ErrorVisibility = Visibility.Visible;
        
            Debug.WriteLine($"[EXCEPTION] {ex}");
            if (ex.InnerException != null)
                Debug.WriteLine($"[INNER] {ex.InnerException}");
        }
    }
}