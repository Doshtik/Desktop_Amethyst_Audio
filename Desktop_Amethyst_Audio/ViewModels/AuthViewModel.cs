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

    public PageViewModels.AuthPageViewModel AuthPageViewModel { get; }
    public PageViewModels.RegisterPageViewModel RegisterPageViewModel { get; }

    public AuthViewModel()
    {
        AuthPageViewModel = new PageViewModels.AuthPageViewModel(NavigateToRegister);
        RegisterPageViewModel = new PageViewModels.RegisterPageViewModel(NavigateToAuth);
        
        CurrentPage = AuthPageViewModel;
    }
    
    public void NavigateToAuth() => CurrentPage = AuthPageViewModel;
    public void NavigateToRegister() => CurrentPage = RegisterPageViewModel;
}