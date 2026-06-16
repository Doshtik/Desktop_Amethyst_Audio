using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Data;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;
using Desktop_Amethyst_Audio.Views.Windows;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class PlaylistPage : Page
{
    public PlaylistInfoDto Playlist { get; private set; }
    public ObservableCollection<TrackInfoDto> Tracks { get; set; }
    
    private bool _isOwnPlaylist;
    private bool _isSaved;
    
    private readonly IPlaylistApiClient _playlistApiClient;
    private readonly IProfileApiClient _profileApiClient;
    private readonly ISettingsService _settingsService;
    
    public PlaylistPage(PlaylistInfoDto playlist, bool isOwnPlaylist)
    {
        InitializeComponent();
        _playlistApiClient = new PlaylistApiClient();
        _profileApiClient = new ProfileApiClient();
        _settingsService = new SettingsService();
        Playlist = playlist;
        _isOwnPlaylist = isOwnPlaylist;
    }

    private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        PlaylistName.Text = Playlist.Name;
        try
        {
            AuthorNameTextBlock.Text = (await _profileApiClient.GetUserByIdAsync(Playlist.OwnerId)).Nickname;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
        
        try
        {
            PlaylistImage.Source = await _playlistApiClient.GetPlaylistCoverAsync(Playlist.CoverUrl);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }

        if (_isOwnPlaylist)
        {
            SavePlaylistButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            _isSaved = await _playlistApiClient.IsPlaylistSavedAsync(Playlist.Id);
            if (_isSaved)
            {
                SavePlaylistButton.Content = "Удалить";
            }
            else
            {
                SavePlaylistButton.Content = "Сохранить";
            }
        }
        LoadActionsPopup();
        
        TrackListBox.Items.Clear();
        List<TrackInfoDto> savedTracks = new List<TrackInfoDto>();
        WeakReferenceMessenger.Default.Register<SavedTracksTransferMessage>(this, (recipient, message) => savedTracks = message.savedTracks);
        foreach (TrackInfoDto trackDto in Playlist.TrackList)
        {
            bool isSaved = savedTracks.Contains(trackDto);
            TrackControl control =  new TrackControl(isSaved);
            control.Track = trackDto;
            control.Width = TrackListBox.ActualWidth - 40;
            TrackListBox.Items.Add(control);
        }

        TrackAmountTextBlock.Text = TrackListBox.Items.Count.ToString();
    }

    private void LoadActionsPopup()
    {
        if (_isOwnPlaylist)
        {
            Button editButton = new Button();
            editButton.Content = "Редактировать";
            editButton.Click += EditPlaylist_Selected;
            ActionsStackPanel.Children.Add(editButton);
        }
        else
        {
            
        }
    }

    private void AuthorNameHyperLink_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(Playlist.OwnerId, false));
    }

    private void PlayPlaylistButton_OnClick(object sender, RoutedEventArgs e)
    {
        var tracks = TrackListBox.Items
            .Cast<TrackControl>()
            .Where(c => c is not null)
            .Select(c => c.Track)
            .ToList();
        PlaybackService.SetQueue(tracks);
    }

    private void AddToSaved_Selected(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_isSaved)
        {
            try
            {
                _playlistApiClient.UnsavePlaylistAsync(Playlist.Id);
                _isSaved = false;
                SavePlaylistButton.Content = "Сохранить";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Debug.WriteLine(exception);
            }
        }
        else
        {
            try
            {
                _playlistApiClient.SavePlaylistAsync(Playlist.Id);
                _isSaved = true;
                SavePlaylistButton.Content = "Удалить";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Debug.WriteLine(exception);
            }
        }
    }

    private void EditPlaylist_Selected(object sender, System.Windows.RoutedEventArgs e)
    {
        //TODO: Сделать форму созданияя/редактирования плейлиста
    }

    private void TrackListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TrackControl? control = TrackListBox.SelectedItem as TrackControl;
        if (control is not null)
        {
            PlaybackService.CurrentTrack = control.Track;
            WeakReferenceMessenger.Default.Send(new TrackChangedMessage(PlaybackService.CurrentTrack));
        }
    }
}