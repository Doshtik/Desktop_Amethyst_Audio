using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Data;
using Desktop_Amethyst_Audio.Messages.Navigation;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Messages.Navigation.System;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.ModalWindows;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class ProfilePage : Page
{
    private TrackApiClient _trackApiClient;
    private AlbumApiClient _albumApiClient;
    private PlaylistApiClient _playlistApiClient;
    private ProfileApiClient _profileApiClient;

    private SettingsService _settingsService;
    
    private long _userId;
    private bool _isOwnProfile;
    private bool _isFollowed;
    private UserInfoDto _user;
    
    private List<TrackInfoDto> _trackList;
    private List<AlbumInfoDto> _albumList;
    private List<PlaylistInfoDto> _playlistList;
    
    private readonly BitmapImage _avatarHeader = new BitmapImage(new Uri("pack://application:,,,/Assets/default-header.jpg"));
    
    
    public ProfilePage(long userId, bool isOwnProfile)
    {
        InitializeComponent();
        _profileApiClient = new ProfileApiClient();
        _trackApiClient = new TrackApiClient();
        _albumApiClient = new AlbumApiClient();
        _playlistApiClient = new PlaylistApiClient();
        _settingsService = new SettingsService();
        
        _userId = userId;
        _isOwnProfile = isOwnProfile;
    }
    
    private async void ProfilePage_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _user = await _profileApiClient.GetUserByIdAsync(_userId);
            UserNicknameTextBlock.Text = _user.Nickname;
            int amountOfSubs = await _profileApiClient.GetAmountOfSubsAsync(_userId);
            UserAmountOfSubsTextBlock.Text = amountOfSubs.ToString();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }

        try
        {
            BitmapImage avatar = await _profileApiClient.GetUserAvatarAsync(_user.AvatarUrl);
            UserAvatarImage.Source = avatar;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
        
        try
        {
            BitmapImage header = await _profileApiClient.GetUserHeaderAsync(_user.HeaderUrl);
            UserHeaderImage.Source = header;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
            UserHeaderImage.Source = _avatarHeader;
        }
        
        if (_isOwnProfile)
        {
            ActionStackPanel.Visibility = Visibility.Visible;
            UserProfileActionsStackPanel.Visibility = Visibility.Collapsed;
            LoadUserPopup();
        }
        else
        {
            UserProfileActionsStackPanel.Visibility = Visibility.Visible;
            ActionStackPanel.Visibility = Visibility.Collapsed;
            LoadArtistPopup();

            try
            {
                _isFollowed = await _profileApiClient.IsUserFollowedAsync(_user.Id);
                if (_isFollowed)
                {
                    FollowingUserButton.Content = "Отписаться";
                }
                else
                {
                    FollowingUserButton.Content = "Подписаться";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        
        try
        {
            _trackList = new List<TrackInfoDto>();
            _trackList = await _trackApiClient.GetListByUserIdAsync(_userId);
            LoadTrackListBox();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
        try
        {
            _albumList = new List<AlbumInfoDto>();
            _albumList = await _albumApiClient.GetListByUserIdAsync(_userId);
            LoadAlbumListBox();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
        
        try
        {
            _playlistList = new List<PlaylistInfoDto>();
            _playlistList = await _playlistApiClient.GetListByUserIdAsync(_userId);
            LoadPlaylistListBox();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
    }

    private void LoadUserPopup()
    {
        PopupContentPanel.Children.Clear();
        
        //Тут должны быть возможность открыть страницу уведомлений, настроек и выйти из аккаунта
        Button notificationButton = new Button();
        notificationButton.Content = "Уведомления";
        notificationButton.Click += (sender, args) =>
        {
            WeakReferenceMessenger.Default.Send(new NavigateToNotificationMessage());
        };
        PopupContentPanel.Children.Add(notificationButton);
        
        Button settingsButton = new Button();
        settingsButton.Content = "Настройки";
        settingsButton.Click += (sender, args) =>
        {
            WeakReferenceMessenger.Default.Send(new NavigateToSettingsMessage());
        };
        PopupContentPanel.Children.Add(settingsButton);
        
        Button quitButton = new Button();
        quitButton.Content = "Выйти";
        quitButton.Click += (sender, args) =>
        {
            MessageBoxResult result = MessageBox.Show("Выйти?", "Подтверждение", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result is not MessageBoxResult.Yes)
                return;

            AppSettings settings = _settingsService.Load();
            settings.User = null;
            _settingsService.Save(settings);
            WeakReferenceMessenger.Default.Send(new NavigateToAuthMessage());
        };
        PopupContentPanel.Children.Add(quitButton);
    }

    private void LoadArtistPopup()
    {
        //Тут должна быть возможность открыть окно жалобы на пользователя
    }

    private void UserTrackListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TrackControl? control = UserTrackListBox.SelectedItem as TrackControl;
        if (control is not null)
        {
            PlaybackService.CurrentTrack = control.Track;
            WeakReferenceMessenger.Default.Send(new TrackChangedMessage(PlaybackService.CurrentTrack));
        }
    }

    private void UserAlbumListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
        AlbumInfoDto album = (UserAlbumListBox.SelectedItem as AlbumControl).Album;
        WeakReferenceMessenger.Default.Send(new NavigateToAlbumMessage(album, false));
    }

    private void UserPlaylistListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
        PlaylistInfoDto playlist = (UserTrackListBox.SelectedItem as PlaylistControl).Playlist;
        WeakReferenceMessenger.Default.Send(new NavigateToPlaylistMessage(playlist, false));
    }

    private void LoadTrackListBox()
    {
        UserTrackListBox.Items.Clear();
        foreach (var item in _trackList)
        {
            TrackControl trackControl = new TrackControl(_isOwnProfile);
            trackControl.Track = item;
            UserTrackListBox.Items.Add(trackControl);
        }
    }
    private void LoadAlbumListBox()
    {
        UserAlbumListBox.Items.Clear();
        foreach (var item in _albumList)
        {
            AlbumControl trackControl = new AlbumControl();
            trackControl.Album = item;
            trackControl.Width = 130;
            trackControl.Height = 160;
            UserAlbumListBox.Items.Add(trackControl);
        }
    }
    private void LoadPlaylistListBox()
    {
        UserPlaylistListBox.Items.Clear();
        foreach (var item in _playlistList)
        {
            PlaylistControl trackControl = new PlaylistControl();
            trackControl.Playlist = item;
            UserPlaylistListBox.Items.Add(trackControl);
        }
    }

    private async void FollowingUserButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            if (_isFollowed)
            {
                await _profileApiClient.UnfollowUserAsync(_userId);
                FollowingUserButton.Content = "Подписаться";
            }
            else
            {
                await _profileApiClient.FollowUserAsync(_userId);
                FollowingUserButton.Content = "Отписаться";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
    }

    private void ReleaseTrackButton_OnClick(object sender, RoutedEventArgs e)
    {
        TrackFormModalWindow window = new TrackFormModalWindow();
        window.ShowDialog();
        ProfilePage_OnLoaded(sender, e);
    }

    private void ReleaseAlbumButton_OnClick(object sender, RoutedEventArgs e)
    {
        AlbumFormModalWindow window = new AlbumFormModalWindow();
        window.ShowDialog();
        ProfilePage_OnLoaded(sender, e);
    }

    private void OpenReportModalWindow_OnClick(object sender, RoutedEventArgs e)
    {
        //TODO: Тут должно открываться окно жалобы на исполнителя
    }
    
    private void NavigateToNotification_Selected(object sender, RoutedEventArgs e)
        => WeakReferenceMessenger.Default.Send(new NavigateToNotificationMessage());

    private void NavigateToSettings_Selected(object sender, RoutedEventArgs e)
        => WeakReferenceMessenger.Default.Send(new NavigateToSettingsMessage());

    private void NavigateToAuth_Selected(object sender, RoutedEventArgs e)
    {
        try
        {
            AppSettings settings = _settingsService.Load();
            settings.User = null;
            _settingsService.Save(settings); 
            WeakReferenceMessenger.Default.Send(new NavigateToAuthMessage());
        }
        catch (Exception ex)
        {
            MessageBox.Show("Возникла неожиданная ошибка");
        }
    }
}