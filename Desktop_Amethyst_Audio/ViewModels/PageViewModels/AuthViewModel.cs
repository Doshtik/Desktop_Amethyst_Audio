using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class AuthViewModel : ObservableObject
{
    [ObservableProperty] private string _changeLanguageButtonContent;
    
    
}