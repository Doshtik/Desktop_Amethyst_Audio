using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Desktop_Amethyst_Audio.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _currentPage;

    public PageViewModels.AuthViewModel AuthPageViewModel { get; } = new();
    public PageViewModels.RegisterViewModel RegisterPageViewModel { get; } = new();

    public AuthViewModel()
    {
        _currentPage = AuthPageViewModel;
    }
    
    [RelayCommand]
    public void NavigateToAuth() => CurrentPage = AuthPageViewModel;
    [RelayCommand]
    public void NavigateToRegister() => CurrentPage = RegisterPageViewModel;
    
    
}