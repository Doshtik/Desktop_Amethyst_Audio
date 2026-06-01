using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class QueueTrackControl : UserControl
{
    public TrackInfoDto Track { get; set; }
    public QueueTrackControl()
    {
        InitializeComponent();
    }

    private async void TrackInQueueControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        TrackApiClient client = new TrackApiClient();
        try
        {
            BitmapImage image = await client.GetTrackCoverAsync(Track.CoverUrl);
            TrackImage.Source = image;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }

        TrackNameTextBlock.Text = Track.Name;
        foreach (UserInfoDto dto in Track.UserList)
        {
            TextBlock user = new TextBlock();
            Hyperlink link = new Hyperlink();
            link.Click += (sender, e) 
                => WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(dto.Id, false));
            link.SetResourceReference(Hyperlink.ForegroundProperty, "ContentPrimaryBrush");
            Run runText = new Run(dto.Nickname);
            link.Inlines.Add(runText);
            user.Inlines.Add(link);
            TrackUsersPanel.Children.Add(user);
            TextBlock space = new TextBlock();
            space.Margin = new Thickness(5,0,0,0);
            TrackUsersPanel.Children.Add(space);
        }
    }

    private void QueueTrackControl_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        
    }
}