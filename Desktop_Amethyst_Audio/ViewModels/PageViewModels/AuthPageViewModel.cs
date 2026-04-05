using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class AuthPageViewModel : ObservableObject
{
    private readonly Action _navigateToRegister;

    public AuthPageViewModel(Action navigateToRegister)
    {
        _navigateToRegister = navigateToRegister;
    }
    
    [RelayCommand]
    private void NavigateToRegister() => _navigateToRegister?.Invoke();

    private bool ValidateFields()
    {
        string emailPattern = @"^[А-ЯЁа-яёA-Za-z\s\.\_]+$";
        return true;
    }
}