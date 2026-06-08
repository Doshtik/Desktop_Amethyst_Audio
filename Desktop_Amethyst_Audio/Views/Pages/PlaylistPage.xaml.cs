using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Views.UserControls;
using Desktop_Amethyst_Audio.Views.Windows;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class PlaylistPage : Page
{
    public PlaylistInfoDto Playlist { get; private set; }
    private readonly IPlaylistApiClient _playlistApiClient;
    private readonly IProfileApiClient _profileApiClient;
    public PlaylistPage(PlaylistInfoDto playlist)
    {
        InitializeComponent();
        _playlistApiClient = new PlaylistApiClient();
        _profileApiClient = new ProfileApiClient();
        Playlist = playlist;
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
        
        foreach (TrackInfoDto trackDto in Playlist.TrackList)
        {
            TrackControl control =  new TrackControl();
            control.Track = trackDto;
            control.Width = TrackListBox.ActualWidth - 40;
        }
    }

    private void AuthorNameHyperLink_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(Playlist.OwnerId, false));
    }

    private void AddToSaved_Selected(object sender, System.Windows.RoutedEventArgs e)
    {
        //TODO: Сделать логику добавления плейлиста в сохраненные
    }

    private void EditPlaylist_Selected(object sender, System.Windows.RoutedEventArgs e)
    {
        //TODO: Сделать форму созданияя/редактирования плейлиста
    }
}