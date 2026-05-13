using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
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
    
    private ISettingsService _settingsService;
    
    private readonly IAuthApiClient _authApiClient = new AuthApiClient();
    
    public AuthPageViewModel()
    {
        
    }
    
    [RelayCommand]
    private void NavigateToRegister() 
        => WeakReferenceMessenger.Default.Send(new NavigateToRegisterMessage());

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

        LoginDto loginDto = new LoginDto()
        {
            Email = EmailField,
            Password = pb.Password
        };
        
        try
        {
            UserInfoDto userDto = _authApiClient.LoginUserAsync(loginDto).Result;
            AppSettings settings = _settingsService.Load();
            settings.User = userDto;
            _settingsService.Save(settings);
            
        }
        catch
        {
            MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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