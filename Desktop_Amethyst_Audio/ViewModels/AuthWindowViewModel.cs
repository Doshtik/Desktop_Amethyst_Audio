using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation;

namespace Desktop_Amethyst_Audio.ViewModels;

public partial class AuthWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _currentPage;

    public PageViewModels.AuthPageViewModel AuthPageViewModel { get; } = new();
    public PageViewModels.RegisterPageViewModel RegisterPageViewModel { get; } = new();

    public AuthWindowViewModel()
    {
        CurrentPage = AuthPageViewModel;
        
        WeakReferenceMessenger.Default.Register<NavigateToAuthMessage>(this, (r, m) => CurrentPage = AuthPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToRegisterMessage>(this, (r, m) => CurrentPage = RegisterPageViewModel);
    }
}