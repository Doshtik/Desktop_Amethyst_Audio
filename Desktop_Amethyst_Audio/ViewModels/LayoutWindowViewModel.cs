using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.ViewModels.PageViewModels;
using Desktop_Amethyst_Audio.Views.Pages;

namespace Desktop_Amethyst_Audio.ViewModels;

public partial class LayoutWindowViewModel : ObservableObject
{
    [ObservableProperty] 
    private ObservableObject _currentPage;

    private SearchPageViewModel SearchPageViewModel { get; } = new();
    private SearchResultPageViewModel SearchResultPageViewModel { get; } = new();
    private ResonancePageViewModel ResonancePageViewModel { get; } = new();
    private LibraryPageViewModel LibraryPageViewModel { get; } = new();
    private ProfilePageViewModel ProfilePageViewModel { get; } = new();
    private AlbumPageViewModel AlbumPageViewModel { get; } = new();
    private PlaylistPageViewModel PlaylistPageViewModel { get; } = new();

    public LayoutWindowViewModel()
    {
        CurrentPage = SearchPageViewModel;
        
        WeakReferenceMessenger.Default.Register<NavigateToSearchMessage>(this, (r, m) => CurrentPage = SearchPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToSearchResultMessage>(this, (r, m) => CurrentPage = SearchResultPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToResonanceMessage>(this, (r, m) => CurrentPage = ResonancePageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToLibraryMessage>(this, (r, m) => CurrentPage = LibraryPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToProfileMessage>(this, (r, m) => CurrentPage = ProfilePageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToAlbumMessage>(this, (r, m) => CurrentPage = AlbumPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToPlaylistMessage>(this, (r, m) => CurrentPage = PlaylistPageViewModel);
    }
    
    [RelayCommand]
    private void NavigateToSearch() 
        => WeakReferenceMessenger.Default.Send(new NavigateToSearchMessage());
    
    [RelayCommand]
    private void NavigateToResonance() 
        => WeakReferenceMessenger.Default.Send(new NavigateToResonanceMessage());
    
    [RelayCommand]
    private void NavigateToLibrary() 
        => WeakReferenceMessenger.Default.Send(new NavigateToLibraryMessage());

    [RelayCommand]
    private void GetSavedPlaylists()
    {
        
    }

    [RelayCommand]
    private void GetSavedAlbums()
    {
        
    }
}