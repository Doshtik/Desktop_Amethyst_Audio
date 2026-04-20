using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class RegisterPageViewModel: ObservableObject
{
    [ObservableProperty] private string _nickname;
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _errorField;
    [ObservableProperty] private Visibility _errorVisibility;
    
    private readonly Action _navigateBack;

    public RegisterPageViewModel(Action navigateBack)
    {
        _navigateBack = navigateBack;
        _nickname = string.Empty;
        _email = string.Empty;
        _errorField = string.Empty;
        _errorVisibility = Visibility.Collapsed;
    }
    
    [RelayCommand]
    private void NavigateBack() => _navigateBack?.Invoke();

    [RelayCommand]
    private void ChangeTheme()
    {
        //TODO: Add change theme 
    }

    [RelayCommand]
    private void ChangeLanguage()
    {
        //TODO: Add change language 
    }

    [RelayCommand]
    private void EnterButtonOnClick()
    {
        throw new NotImplementedException();
    }
}