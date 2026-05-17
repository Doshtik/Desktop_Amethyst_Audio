using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.Pages;
using NAudio.Wave;

namespace Desktop_Amethyst_Audio.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class LayoutWindow : Window
{
    private readonly AudioService _audioService;
    private readonly ITrackApiClient _trackApiClient;

    private int _queuePosition;
    private List<TrackInfoDto> _trackQueueList;
    private List<TrackInfoDto> _trackOriginalQueueList;
    
    private bool _isShuffle;
    private bool _isRepeat;
    private Brush _shuffleButtonBackground;
    private Brush _repeatButtonBackground;
    
    private SearchPage SearchPage { get; } = new();
    private SearchResultPage SearchResultPage { get; } = new();
    private ResonancePage ResonancePage { get; } = new();
    private LibraryPage LibraryPage { get; } = new();
    private ProfilePage ProfilePage { get; } = new();
    private AlbumPage AlbumPage { get; } = new();
    private PlaylistPage PlaylistPage { get; } = new();
    
    public LayoutWindow()
    {
        InitializeComponent();
        
        _trackApiClient = new TrackApiClient();
        _audioService = new AudioService();
        _audioService.Initialize(new WaveFormat(44100, 16, 2));
        
        WeakReferenceMessenger.Default.Register<NavigateToSearchMessage>(this, (r, m) => ContentFrame.Navigate(SearchPage));
        WeakReferenceMessenger.Default.Register<NavigateToSearchResultMessage>(this, (r, m) => ContentFrame.Navigate(SearchResultPage));
        WeakReferenceMessenger.Default.Register<NavigateToResonanceMessage>(this, (r, m) => ContentFrame.Navigate(ResonancePage));
        WeakReferenceMessenger.Default.Register<NavigateToLibraryMessage>(this, (r, m) => ContentFrame.Navigate(LibraryPage));
        WeakReferenceMessenger.Default.Register<NavigateToProfileMessage>(this, (r, m) => ContentFrame.Navigate(ProfilePage));
        WeakReferenceMessenger.Default.Register<NavigateToAlbumMessage>(this, (r, m) => ContentFrame.Navigate(AlbumPage));
        WeakReferenceMessenger.Default.Register<NavigateToPlaylistMessage>(this, (r, m) => ContentFrame.Navigate(PlaylistPage));
        
        WeakReferenceMessenger.Default.Register<TrackChangedMessage>(this, (r, m) => ChangeTrack(m.Track));
    }
    
    public void NavigateToSearch(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToSearchMessage());
    public void NavigateToResonance(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToResonanceMessage());
    public void NavigateToLibrary(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToLibraryMessage());
    
    private void Repeat()
    {
        
    }
    private void Back()
    {
        
    }
    private void PlayPause()
    {
        
    }
    private void Next()
    {
        
    }
    private void Shuffle()
    {
        
    }
    private async void ChangeTrack(TrackInfoDto track)
    {
        Stream response = await _trackApiClient.GetTrackFileAsync(track.TrackUrl);
        await _audioService.StartAsync(response);
    }
}