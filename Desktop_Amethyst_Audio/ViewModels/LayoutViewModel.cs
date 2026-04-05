using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop_Amethyst_Audio.ViewModels.PageViewModels;
using Desktop_Amethyst_Audio.Views.Pages;

namespace Desktop_Amethyst_Audio.ViewModels;

public partial class LayoutViewModel : ObservableObject
{
    [ObservableProperty] 
    private ObservableObject _currentPage;
    public SearchPageViewModel SearchPageViewModel { get; } = new();
    public SearchResultPageViewModel SearchResultPageViewModel { get; } = new();
    public ResonancePageViewModel ResonancePageViewModel { get; } = new();
    public LibraryPageViewModel LibraryPageViewModel { get; } = new();
    public ProfilePageViewModel ProfilePageViewModel { get; } = new();

    public LayoutViewModel()
    {
        _currentPage = SearchPageViewModel;
    }

    [RelayCommand]
    public void NavigateToSearch() => CurrentPage = SearchPageViewModel;

    [RelayCommand]
    public void NavigateToSearchResult() => CurrentPage = SearchResultPageViewModel;

    [RelayCommand]
    public void NavigateToResonance() => CurrentPage = ResonancePageViewModel;

    [RelayCommand]
    public void NavigateToLibrary() => CurrentPage = LibraryPageViewModel;

    [RelayCommand]
    public void NavigateToProfile() => CurrentPage = ProfilePageViewModel;
}