using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Data;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.ModalWindows;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class TrackControl : UserControl
{
    private IProfileApiClient _profileApiClient;
    private ITrackApiClient _trackApiClient;
    private ISettingsService _settingsService;
    public TrackInfoDto Track { get; set; }
    private bool _isSaved;
    public TrackControl(bool isSaved)
    {
        InitializeComponent();
        _profileApiClient = new ProfileApiClient();
        _trackApiClient = new TrackApiClient();
        _settingsService = new SettingsService();
        _isSaved = isSaved;
    }

    private async void TrackControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        TrackApiClient client = new TrackApiClient();
        try
        {
            BitmapImage image = await client.GetTrackCoverAsync(Track.CoverUrl);
            TrackImage.Source = image;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(Track.CoverUrl);
        }

        if (_isSaved)
        {
            AddTrackInLibrary.Content = "Удалить трек из библиотеки";
        }
        else
        {
            AddTrackInLibrary.Content = "Добавить трек в библиотеку";
        }

        TrackNameTextBlock.Text = Track.Name;
        UserInfoDto author = _settingsService.Load().User;
        foreach (UserInfoDto dto in Track.UserList)
        {
            TextBlock user = new TextBlock();
            Hyperlink link = new Hyperlink();
            link.Click += (sender, e) => NavigateToProfile(dto.Id);
            link.SetResourceReference(Hyperlink.ForegroundProperty, "ContentPrimaryBrush");
            Run runText = new Run(dto.Nickname);
            link.Inlines.Add(runText);
            user.Inlines.Add(link);
            TrackUsersPanel.Children.Add(user);
            TextBlock space = new TextBlock();
            space.Margin = new Thickness(5,0,0,0);
            TrackUsersPanel.Children.Add(space);

            if (dto.Equals(author))
            {
                Button editTrackButton = new Button();
                editTrackButton.Content = "Редактировать";
                editTrackButton.Click += (s, e) =>
                {
                    TrackFormModalWindow window = new TrackFormModalWindow();
                    window.Track = Track;
                };
                Button deleteTrackButton = new Button();
                deleteTrackButton.Content = "Удалить";
                deleteTrackButton.Click += (s, e) =>
                {
                    MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить трек?");
                    if (result is MessageBoxResult.Yes)
                    {
                        try
                        {
                            _trackApiClient.DeleteAsync(Track.Id);
                            MessageBox.Show("Трек успешно удалён");
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show("Не удалось удалить трек");
                            Debug.WriteLine(exception);
                        }
                    }
                };
            }
        }
    }

    private void NavigateToProfile(long userId) 
        => WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(userId, false));

    public void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        Page? parent = sender as Page;
        this.Width = parent.ActualWidth - 30;
    }

    private void AddTrackInLibrary_Click(object sender, RoutedEventArgs e)
    {
        if (_isSaved)
        {
            try
            {
                _profileApiClient.RemoveTrackFromUserLibraryAsync(Track.Id);
                WeakReferenceMessenger.Default.Register<SavedTracksTransferMessage>(this, (recipient, message) =>
                {
                    message.savedTracks.Remove(Track);
                });
                _isSaved = !_isSaved;
                AddTrackInLibrary.Content = "Добавить трек в библиотеку";
                WeakReferenceMessenger.Default.Send(new LibraryChangedMessage());
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
        else
        {
            try
            {
                _profileApiClient.AddTrackToUserLibraryAsync(Track.Id);
                WeakReferenceMessenger.Default.Register<SavedTracksTransferMessage>(this, (recipient, message) =>
                {
                    message.savedTracks.Add(Track);
                });
                _isSaved = !_isSaved;
                AddTrackInLibrary.Content = "Удалить трек из библиотеки";
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
    }

    private void AddTrackInPlaylist_Click(object sender, RoutedEventArgs e)
    {
        
    }
}