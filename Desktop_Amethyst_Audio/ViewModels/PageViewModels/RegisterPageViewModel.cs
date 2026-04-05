using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class RegisterPageViewModel: ObservableObject
{
    private readonly Action _navigateBack;

    public RegisterPageViewModel(Action navigateBack)
    {
        _navigateBack = navigateBack;
    }
    
    [RelayCommand]
    private void NavigateBack() => _navigateBack?.Invoke();
}