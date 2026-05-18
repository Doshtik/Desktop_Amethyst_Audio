using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class AlbumPage : Page
{
    public AlbumInfoDto Album { get; set; }
    public List<TrackInfoDto> Tracks { get; set; }
    public AlbumPage()
    {
        InitializeComponent();
    }

    private void AlbumPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void EditAlbum_Selected(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void AuthorNameHyperLink_Click(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void AddToSaved_OnSelected(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void PlayAlbumButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NavigateToPrevious_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}