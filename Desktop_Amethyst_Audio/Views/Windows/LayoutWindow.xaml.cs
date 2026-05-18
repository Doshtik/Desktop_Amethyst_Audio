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
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
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
    private readonly ISettingsService _settingsService;
    private readonly IProfileApiClient _profileApiClient;
    private readonly ITrackApiClient _trackApiClient;

    private int _queuePosition;
    private List<TrackInfoDto> _trackQueueList;
    private List<TrackInfoDto> _trackOriginalQueueList;

    private bool _isShuffle;
    private bool _isRepeat;
    private Brush _shuffleButtonBackground;
    private Brush _repeatButtonBackground;
    
    private SearchPage SearchPage { get; } = new();
    private ResonancePage ResonancePage { get; } = new();
    private LibraryPage LibraryPage { get; } = new();
    private ProfilePage ProfilePage { get; } = new();
    private AlbumPage AlbumPage { get; } = new();
    private PlaylistPage PlaylistPage { get; } = new();
    
    public LayoutWindow()
    {
        InitializeComponent();
        
        _profileApiClient = new ProfileApiClient();
        _trackApiClient = new TrackApiClient();
        _settingsService = new SettingsService();
        _audioService = new AudioService();
        try
        {
            _audioService.Initialize(new WaveFormat(44100, 16, 2));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        _isShuffle = false;
        _isRepeat = false;
        BrushConverter cc = new BrushConverter();
        //_shuffleButtonBackground = (Brush)cc.ConvertFrom("");
        //_repeatButtonBackground = (Brush)cc.ConvertFrom("");

        WeakReferenceMessenger.Default.Register<NavigateToSearchMessage>(this, (r, m) => ContentFrame.Navigate(SearchPage));
        WeakReferenceMessenger.Default.Register<NavigateToSearchResultMessage>(this, (r, m) => ContentFrame.Navigate(new SearchResultPage("")));
        WeakReferenceMessenger.Default.Register<NavigateToResonanceMessage>(this, (r, m) => ContentFrame.Navigate(ResonancePage));
        WeakReferenceMessenger.Default.Register<NavigateToLibraryMessage>(this, (r, m) => ContentFrame.Navigate(LibraryPage));
        WeakReferenceMessenger.Default.Register<NavigateToProfileMessage>(this, (r, m) => ContentFrame.Navigate(ProfilePage));
        WeakReferenceMessenger.Default.Register<NavigateToAlbumMessage>(this, (r, m) => ContentFrame.Navigate(AlbumPage));
        WeakReferenceMessenger.Default.Register<NavigateToPlaylistMessage>(this, (r, m) => ContentFrame.Navigate(PlaylistPage));
        
        WeakReferenceMessenger.Default.Register<TrackChangedMessage>(this, (r, m) => ChangeTrack(m.Track));
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            UserInfoDto user = _settingsService.Load().User;
            UserNicknameTextBlock.Text = user.Nickname;
            BitmapImage image = await _profileApiClient.GetUserAvatarAsync(user.AvatarUrl);
            UserAvatarImage.Source = image;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось загрузить данные пользователя");
        }
    }

    public void NavigateToSearch(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToSearchMessage());
    public void NavigateToResonance(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToResonanceMessage());
    public void NavigateToLibrary(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToLibraryMessage());


    private async void ChangeTrack(TrackInfoDto track)
    {
        try
        {
            Stream response = await _trackApiClient.GetTrackFileAsync(track.TrackUrl);
            TrackPanel.Visibility = Visibility.Visible;
            await _audioService.StartAsync(response);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось воспроизвести трек: ", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RepeatButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isRepeat is true)
        {
            _isRepeat = false;
            RepeatButton.Background = _repeatButtonBackground;
        }
        else
        {
            _isRepeat = true;
            RepeatButton.Background = _repeatButtonBackground;
        }
    }

    private void ShuffleButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isShuffle is true)
        {
            _isShuffle = false;
            ShuffleButton.Background = _shuffleButtonBackground;
        }
        else
        {
            _isShuffle = true;
            ShuffleButton.Background = _shuffleButtonBackground;
        }
    }

    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void NavigateToProfile_Selected(object sender, RoutedEventArgs e)
    {
        try
        {
            UserInfoDto user = _settingsService.Load().User;
            WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(user.Id, true));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось перейти в профиль");
        }
    }

    private void NavigateToNotification_Selected(object sender, RoutedEventArgs e)
    {

    }

    private void NavigateToSettings_Selected(object sender, RoutedEventArgs e)
    {

    }

    private void NavigateToAuth_Selected(object sender, RoutedEventArgs e)
    {

    }

    private void VolumeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        return;
    }

    private void ChangeCollectiontoPlaylistButton_OnChecked(object sender, RoutedEventArgs e)
    {
        
    }

    private void ChangeCollectiontoAlbumButton_OnChecked(object sender, RoutedEventArgs e)
    {
        
    }
}