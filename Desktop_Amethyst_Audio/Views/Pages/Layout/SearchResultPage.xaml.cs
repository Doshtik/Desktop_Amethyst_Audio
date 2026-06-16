using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Data;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class SearchResultPage : Page
{
    private string _searchQuery;
    private ISearchApiClient _searchApiClient;

    private List<UserInfoDto> _users = new();
    private List<TrackInfoDto> _tracks = new();
    private List<AlbumInfoDto> _albums = new();
    private List<PlaylistInfoDto> _playlists = new();
    
    public SearchResultPage(string searchLine)
    {
        InitializeComponent();
        _searchApiClient = new SearchApiClient();
        _searchQuery = searchLine;
    }

    private async void SearchResultPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            SearchInfoDto result = await _searchApiClient.GetBySearchAsync(_searchQuery);
            _users = result.Users;
            _tracks = result.Tracks;
            _albums = result.Albums;
            _playlists = result.Playlists;
            
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Debug.WriteLine(ex);
        }

        UserListBox.Items.Clear();
        foreach (UserInfoDto userItem in _users)
        {
            UsersControl control = new UsersControl();
            control.User = userItem;
            UserListBox.Items.Add(control);
        }

        TrackListBox.Items.Clear();
        List<TrackInfoDto> savedTracks = new List<TrackInfoDto>();
        WeakReferenceMessenger.Default.Register<SavedTracksTransferMessage>(this, (recipient, message) => savedTracks = message.savedTracks);
        foreach (TrackInfoDto trackItem in _tracks)
        {
            bool isSaved = savedTracks.Contains(trackItem);
            TrackControl control = new TrackControl(isSaved);
            control.Track = trackItem;
            TrackListBox.Items.Add(control);
        }

        AlbumListBox.Items.Clear();
        foreach (AlbumInfoDto albumItem in _albums)
        {
            AlbumControl control = new  AlbumControl();
            control.Album = albumItem;
            AlbumListBox.Items.Add(control);
        }

        PlaylistListBox.Items.Clear();
        foreach (PlaylistInfoDto playlistItem in _playlists)
        {
            PlaylistControl control = new  PlaylistControl();
            control.Playlist = playlistItem;
            PlaylistListBox.Items.Add(control);
        }
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

    private void UserListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UsersControl control = UserListBox.SelectedItem as UsersControl;
        WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(control.User.Id, false));
    }

    private void AlbumListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        AlbumControl control = AlbumListBox.SelectedItem as AlbumControl;
        WeakReferenceMessenger.Default.Send(new NavigateToAlbumMessage(control.Album,  false));
    }

    private void PlaylistListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PlaylistControl control = AlbumListBox.SelectedItem as PlaylistControl;
        WeakReferenceMessenger.Default.Send(new NavigateToPlaylistMessage(control.Playlist,  false));
    }
}