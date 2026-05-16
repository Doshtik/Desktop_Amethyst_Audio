using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.ViewModels.PageViewModels;
using Desktop_Amethyst_Audio.Views.Pages;
using NAudio.Wave;

namespace Desktop_Amethyst_Audio.ViewModels.WindowViewModel;

public partial class LayoutWindowViewModel : ObservableObject
{
    [ObservableProperty] 
    private ObservableObject _currentPage;
    
    private readonly AudioService _audioService;
    private readonly ITrackApiClient _trackApiClient;

    private int _queuePosition;
    private List<TrackInfoDto> _trackQueueList;
    private List<TrackInfoDto> _trackOriginalQueueList;
    
    [ObservableProperty] private bool _isShuffle;
    [ObservableProperty] private bool _isRepeat;
    [ObservableProperty] private Brush _shuffleButtonBackground;
    [ObservableProperty] private Brush _repeatButtonBackground;

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

        _trackApiClient = new TrackApiClient();
        _audioService = new AudioService();
        _audioService.Initialize(new WaveFormat(44100, 16, 2));
        
        WeakReferenceMessenger.Default.Register<NavigateToSearchMessage>(this, (r, m) => CurrentPage = SearchPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToSearchResultMessage>(this, (r, m) => CurrentPage = SearchResultPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToResonanceMessage>(this, (r, m) => CurrentPage = ResonancePageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToLibraryMessage>(this, (r, m) => CurrentPage = LibraryPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToProfileMessage>(this, (r, m) => CurrentPage = ProfilePageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToAlbumMessage>(this, (r, m) => CurrentPage = AlbumPageViewModel);
        WeakReferenceMessenger.Default.Register<NavigateToPlaylistMessage>(this, (r, m) => CurrentPage = PlaylistPageViewModel);
        
        WeakReferenceMessenger.Default.Register<TrackChangedMessage>(this, (r, m) => ChangeTrack(m.Track));
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
    private void Repeat()
    {
        
    }

    [RelayCommand]
    private void Back()
    {
        
    }

    [RelayCommand]
    private void PlayPause()
    {
        
    }

    [RelayCommand]
    private void Next()
    {
        
    }

    [RelayCommand]
    private void Shuffle()
    {
        
    }

    private async void ChangeTrack(TrackInfoDto track)
    {
        Stream response = await _trackApiClient.GetTrackFileAsync(track.TrackUrl);
        await _audioService.StartAsync(response);
    }
    
    [RelayCommand]
    private void GetSavedPlaylists()
    {
        
    }

    [RelayCommand]
    private void GetSavedAlbums()
    {
        
    }
}