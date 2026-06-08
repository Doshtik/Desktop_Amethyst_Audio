using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class AlbumControl : UserControl
{
    public AlbumInfoDto Album { get; set; }
    private readonly IAlbumApiClient _albumApiClient;
    public AlbumControl()
    {
        InitializeComponent();
        _albumApiClient = new AlbumApiClient();
    }

    private async void AlbumControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            BitmapImage coverImage = await _albumApiClient.GetAlbumCoverAsync(Album.CoverUrl);
            AlbumCoverImage.Source = coverImage;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.InnerException);
        }
        AlbumNameTextBlock.Text = Album.Name;
    }
}