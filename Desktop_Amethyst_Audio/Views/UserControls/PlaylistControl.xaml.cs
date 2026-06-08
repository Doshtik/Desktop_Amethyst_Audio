using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class PlaylistControl : UserControl
{
    public PlaylistInfoDto Playlist { get; set; }
    private  readonly IPlaylistApiClient _playlistApiClient;
    public PlaylistControl()
    {
        InitializeComponent();
    }

    private async void PlaylistControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            BitmapImage coverImage = await _playlistApiClient.GetPlaylistCoverAsync(Playlist.CoverUrl);
            PlaylistCoverImage.Source = coverImage;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.InnerException);
        }
        PlaylistNameTextBlock.Text = Playlist.Name;
    }
}