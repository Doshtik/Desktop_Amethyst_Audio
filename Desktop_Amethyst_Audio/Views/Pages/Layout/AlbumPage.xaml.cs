using System.Collections.ObjectModel;
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
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Data;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;
using Desktop_Amethyst_Audio.Views.Windows;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class AlbumPage : Page
{
    public AlbumInfoDto Album { get; set; }
    public ObservableCollection<TrackInfoDto> Tracks { get; set; }

    private bool _isOwnAlbum;
    private bool _isSaved;

    private readonly IAlbumApiClient _albumApiClient;

    public AlbumPage(AlbumInfoDto album, bool isOwnAlbum)
    {
        InitializeComponent();
        _albumApiClient = new AlbumApiClient();
        Album = album;
        _isOwnAlbum = isOwnAlbum;
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

        if (_isOwnAlbum)
        {
            SaveAlbumButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            try
            {
                _isSaved = await _albumApiClient.IsAlbumSavedAsync(Album.Id);
                if (_isSaved)
                {
                    SaveAlbumButton.Content = "Удалить";
                }
                else
                {
                    SaveAlbumButton.Content = "Сохранить";
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Debug.WriteLine(exception);
            }
        }
        LoadActionsPopup();
        
        TrackListBox.Items.Clear();
        List<TrackInfoDto> savedTracks = new List<TrackInfoDto>();
        WeakReferenceMessenger.Default.Register<SavedTracksTransferMessage>(this, (recipient, message) => savedTracks = message.savedTracks);
        foreach (TrackInfoDto trackDto in Album.TrackList)
        {
            bool isSaved = savedTracks.Contains(trackDto);
            TrackControl control =  new TrackControl(isSaved);
            control.Track = trackDto;
            control.Width = TrackListBox.ActualWidth - 40;
            TrackListBox.Items.Add(control);
        }

        TrackAmountTextBlock.Text = TrackListBox.Items.Count.ToString();
    }

    private void LoadActionsPopup()
    {
        if (_isOwnAlbum)
        {
            Button editButton = new Button();
            editButton.Content = "Редактировать";
            editButton.Click += EditAlbum_Selected;
            AlbumActionsStackPanel.Children.Add(editButton);
            Button deleteButton = new Button();
            deleteButton.Content = "Удалить";
            deleteButton.Click += (s, e) =>
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить альбом?", "Подтверждениие",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;

                try
                {
                    _albumApiClient.DeleteAlbumAsync(Album.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось удалить альбом");
                    Debug.WriteLine(ex);
                }
            };
            AlbumActionsStackPanel.Children.Add(editButton);
        }
        else
        {
            
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
        var tracks = TrackListBox.Items
            .Cast<TrackControl>()
            .Where(c => c is not null)
            .Select(c => c.Track)
            .ToList();
        PlaybackService.SetQueue(tracks);
    }

    private void TrackListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TrackControl? control = TrackListBox.SelectedItem as TrackControl;
        if (control is not null)
        {
            PlaybackService.CurrentTrack = control.Track;
            WeakReferenceMessenger.Default.Send(new TrackChangedMessage(PlaybackService.CurrentTrack));
        }
    }

    private void SaveAlbumButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (_isSaved)
        {
            try
            {
                _albumApiClient.UnsaveAlbumAsync(Album.Id);
                _isSaved = false;
                SaveAlbumButton.Content = "Сохранить";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Debug.WriteLine(exception);
            }
        }
        else
        {
            try
            {
                _albumApiClient.SaveAlbumAsync(Album.Id);
                _isSaved = true;
                SaveAlbumButton.Content = "Удалить";
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Debug.WriteLine(exception);
            }
        }
    }
}