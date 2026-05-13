using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class RegisterPageViewModel: ObservableObject
{
    [ObservableProperty] private string _nickname;
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _errorField;
    [ObservableProperty] private Visibility _errorVisibility;
    
    public RegisterPageViewModel()
    {
        
    }
    
    [RelayCommand]
    private void NavigateToAuth() 
        => WeakReferenceMessenger.Default.Send(new NavigateToAuthMessage());

    [RelayCommand]
    private void RegisterButtonOnClick()
    {
        throw new NotImplementedException();
    }
}