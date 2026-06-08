using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;
using Desktop_Amethyst_Audio.Views.Windows;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class AlbumPage : Page
{
    public AlbumInfoDto Album { get; set; }
    public List<TrackInfoDto> Tracks { get; set; }

    private readonly IAlbumApiClient _albumApiClient;

    public AlbumPage(AlbumInfoDto album)
    {
        InitializeComponent();
        _albumApiClient = new AlbumApiClient();
        Album = album;
    }

    private async void AlbumPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        AlbumNameTextBlock.Text = Album.Name;
        try
        {
            AlbumImage.Source = await _albumApiClient.GetAlbumCoverAsync(Album.CoverUrl);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.InnerException);
        }
        foreach (TrackInfoDto trackDto in Album.TrackList)
        {
            TrackControl control =  new TrackControl();
            control.Track = trackDto;
            control.Width = TrackListBox.ActualWidth - 40;
        }
    }

    private void NavigateToProfile(long id)
        => WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(id, false));

    private void EditAlbum_Selected(object sender, RoutedEventArgs e)
    {
        
    }

    private void AddToSaved_OnSelected(object sender, RoutedEventArgs e)
    {
        
    }

    private void PlayAlbumButton_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void NavigateToPrevious_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}