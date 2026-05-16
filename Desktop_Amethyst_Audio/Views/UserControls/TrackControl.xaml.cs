using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class TrackControl : UserControl
{
    public TrackInfoDto Track { get; set; }
    public TrackControl()
    {
        InitializeComponent();
    }

    private async void TrackControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        TrackApiClient client = new TrackApiClient();
        try
        {
            var image = await client.GetTrackCoverAsync(Track.CoverUrl);
            TrackImage.Source = image;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
        TrackNameTextBlock.Text = Track.Name;
        string users = string.Empty;
        foreach (UserInfoDto dto in Track.UserList)
        {
            users += dto.Nickname + ", ";
        }
        TrackAuthorsTextBlock.Text = users.Substring(0, users.Length - 2);
    }

    public void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Page? parent = sender as Page;
        this.Width = parent.ActualWidth - 30;
    }
}