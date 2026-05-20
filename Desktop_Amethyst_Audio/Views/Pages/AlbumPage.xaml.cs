using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class AlbumPage : Page
{
    public AlbumInfoDto Album { get; set; }
    public List<TrackInfoDto> Tracks { get; set; }

    private readonly IAlbumApiClient _albumApiClient;

    public AlbumPage()
    {
        InitializeComponent();
    }

    private async void AlbumPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            AlbumName.Text = Album.Name;
            //AlbumImage.Source = await _albumApiClient.GetAlbumCoverAsync(Album.CoverUrl);
            foreach (UserInfoDto userDto in Album.AuthorList)
            {
                TextBlock user = new TextBlock();
                Hyperlink link = new Hyperlink();
                link.Click += (sender, e) => NavigateToProfile(userDto.Id);
                link.SetResourceReference(Hyperlink.ForegroundProperty, "ContentPrimaryBrush");
                Run runText = new Run(userDto.Nickname);
                link.Inlines.Add(runText);
                user.Inlines.Add(link);
                AuthorListPanel.Children.Add(user);
                TextBlock space = new TextBlock();
                space.Margin = new Thickness(0, 5, 0, 0);
                AuthorListPanel.Children.Add(space);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Console.WriteLine(ex.InnerException);
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