using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop_Amethyst_Audio.ViewModels.PageViewModels;
using Desktop_Amethyst_Audio.Views.Pages;

namespace Desktop_Amethyst_Audio.ViewModels;

public partial class LayoutViewModel : ObservableObject
{
    [ObservableProperty] private ObservableObject _currentPage;
    public SearchViewModel SearchViewModel { get; } = new();
    public SearchResultViewModel SearchResultViewModel { get; } = new();
    public ResonanceViewModel ResonanceViewModel { get; } = new();
    public LibraryViewModel LibraryViewModel { get; } = new();
    public ProfileViewModel ProfileViewModel { get; } = new();

    public LayoutViewModel()
    {
        _currentPage = SearchViewModel;
    }

    [RelayCommand]
    public void NavigateToSearch() => CurrentPage = SearchViewModel;

    [RelayCommand]
    public void NavigateToSearchResult() => CurrentPage = SearchResultViewModel;

    [RelayCommand]
    public void NavigateToResonance() => CurrentPage = ResonanceViewModel;

    [RelayCommand]
    public void NavigateToLibrary() => CurrentPage = LibraryViewModel;

    [RelayCommand]
    public void NavigateToProfile() => CurrentPage = ProfileViewModel;
}