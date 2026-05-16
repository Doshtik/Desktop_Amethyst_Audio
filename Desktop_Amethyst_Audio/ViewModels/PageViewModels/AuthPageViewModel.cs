using System.Diagnostics;
using System.Text.RegularExpressions;
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
using Desktop_Amethyst_Audio.Models.Enums;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class AuthPageViewModel : ObservableObject
{
    [ObservableProperty] private string _emailField;
    [ObservableProperty] private string _errorField;
    [ObservableProperty] private Visibility _errorVisibility;
    
    private ISettingsService _settingsService = new SettingsService();
    
    private readonly IAuthApiClient _authApiClient = new AuthApiClient();
    
    public AuthPageViewModel() { }
    
    [RelayCommand]
    private void NavigateToRegister() 
        => WeakReferenceMessenger.Default.Send(new NavigateToRegisterMessage());

    [RelayCommand]
    private async void EnterButtonOnClick(object password)
    {
        try
        {
            if (password is not PasswordBox pb)
                throw new InvalidOperationException("CommandParameter не является PasswordBox");

            if (!IsFieldsValidated(pb.Password, out string errorMessage))
            {
                ErrorField = errorMessage;
                ErrorVisibility = Visibility.Visible;
                return;
            }

            var loginDto = new LoginDto { Email = EmailField, Password = pb.Password };

            var userDto = await _authApiClient.LoginUserAsync(loginDto);
        
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