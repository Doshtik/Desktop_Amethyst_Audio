using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.ModalWindow;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class PlaylistSelectorModalWindow : Window
{
    private IPlaylistApiClient _playlistApiClient;
    private ISettingsService _settingsService;
    private TrackInfoDto _track;
    
    public PlaylistSelectorModalWindow(TrackInfoDto track)
    {
        InitializeComponent();
        _playlistApiClient = new PlaylistApiClient();
        _settingsService = new SettingsService();
        _track = track;
    }

    private async void PlaylistSelectorModalWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        AppSettings settings = _settingsService.Load();
        try
        {
            List<PlaylistInfoDto> playlists = await _playlistApiClient.GetListByUserIdAsync(settings.User.Id);
            
            PlaylistListBox.Items.Clear();
            PlaylistListBox.SelectedItems.Clear();
            foreach (PlaylistInfoDto playlist in playlists)
            {
                PlaylistControl control = new PlaylistControl();
                control.Playlist = playlist;
                PlaylistListBox.Items.Add(control);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
            Debug.WriteLine(exception);
        }
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
            Debug.WriteLine(exception);
        }
        Close();
    }
}