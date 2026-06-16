using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class PlaylistControl : UserControl
{
    private const string DefaultPlaceholderPath = "pack://application:,,,/Assets/default-track-cover.png"; 
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
            bool isLocalResource = !string.IsNullOrEmpty(Playlist.CoverUrl) && 
                                   Playlist.CoverUrl.StartsWith("pack://application:,,,", StringComparison.OrdinalIgnoreCase);

            if (isLocalResource)
            {
                PlaylistCoverImage.Source = LoadPlaceholder(Playlist.CoverUrl);
            }
            else if (!string.IsNullOrEmpty(Playlist.CoverUrl))
            {
                BitmapImage coverImage = await _playlistApiClient.GetPlaylistCoverAsync(Playlist.CoverUrl);
                PlaylistCoverImage.Source = coverImage;
            }
            else
            {
                PlaylistCoverImage.Source = LoadPlaceholder(DefaultPlaceholderPath); 
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Error loading cover: {exception}"); 
            
            PlaylistCoverImage.Source = LoadPlaceholder(DefaultPlaceholderPath);
        }

        PlaylistNameTextBlock.Text = Playlist.Name;
    }

    private BitmapImage LoadPlaceholder(string imagePath)
    {
        try
        {
            var uri = new Uri(imagePath, UriKind.Absolute);
            var resourceInfo = Application.GetResourceStream(uri);
            
            if (resourceInfo?.Stream == null) return null;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = resourceInfo.Stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to load placeholder '{imagePath}': {ex.Message}");
            return null;
        }
    }
}