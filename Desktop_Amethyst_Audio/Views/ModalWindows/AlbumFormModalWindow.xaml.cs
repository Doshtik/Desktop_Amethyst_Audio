using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Data;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Enums;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;
using Microsoft.Win32;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class AlbumFormModalWindow : Window
{
    public AlbumInfoDto? Album { get; set; }
    private FormMode Mode { get; set; } = FormMode.Add;
    private string? _trackCoverPath;
    
    private IAlbumApiClient _albumApiClient;
    private ITrackApiClient _trackApiClient;
    private IProfileApiClient _profileApiClient;
    private ISettingsService _settingsService;
    
    public AlbumFormModalWindow()
    {
        InitializeComponent();
        _albumApiClient = new AlbumApiClient();
        _trackApiClient = new TrackApiClient();
        _profileApiClient = new ProfileApiClient();
        _settingsService = new SettingsService();
    }

    private async void AlbumFormModalWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        AppSettings settings = _settingsService.Load();
        List<TrackInfoDto> tracks = new List<TrackInfoDto>();
        try
        {
            tracks = await _trackApiClient.GetListByUserIdAsync(settings.User.Id);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
        
        TrackListBox.Items.Clear();
        List<TrackInfoDto> savedTracks = new List<TrackInfoDto>();
        WeakReferenceMessenger.Default.Register<SavedTracksTransferMessage>(this, (recipient, message) => savedTracks = message.savedTracks);
        foreach (TrackInfoDto track in tracks)
        {
            bool isSaved = savedTracks.Contains(track);
            TrackControl control = new TrackControl(isSaved);
            control.Track = track;
            control.Width = TrackListBox.ActualWidth;
            TrackListBox.Items.Add(control);
        }

        if (Album is null)
            return;
        Mode = FormMode.Edit;
        try
        {
            BitmapImage coverImage = await _albumApiClient.GetAlbumCoverAsync(Album.CoverUrl);
            CoverImage.Source = coverImage;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        NameTextBox.Text = Album.Name;
        TrackListBox.SelectedItems.Clear();
        foreach (TrackControl control in TrackListBox.Items)
        {
            if (Album.TrackList.Any(t => t.Id == control.Track.Id))
            {
                TrackListBox.SelectedItems.Add(control);
            }
        }
    }

    private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        AppSettings settings = _settingsService.Load();
        if (!IsFieldsCorrect(out string errorMessage))
        {
            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        
        var selectedControls = TrackListBox.SelectedItems.Cast<TrackControl>().ToList();
        var selectedTrackIds = selectedControls.Select(c => c.Track.Id).ToList();

        if (Mode is FormMode.Add)
        {
            CreateAlbumDto dto = new CreateAlbumDto()
            {
                Name = NameTextBox.Text.Trim(),
                AlbumCoverFilePath = _trackCoverPath,
                AuthorsIdList = new List<long> { settings.User.Id },
                TracksIdList = (TrackListBox.SelectedItems as List<TrackControl>).Select(x => x.Track.Id).ToList()
            };
            try
            {
                _albumApiClient.CreateAlbumAsync(dto);
                MessageBox.Show("Альбом создан", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Не удалось создать трек");
                Debug.WriteLine(exception);
            }
        }
        else
        {
            var originalTrackIds = Album.TrackList.Select(t => t.Id).ToList();

            var addedTrackIds = selectedTrackIds.Except(originalTrackIds).ToList();

            var removedTrackIds = originalTrackIds.Except(selectedTrackIds).ToList();
            
            ChangeAlbumInfoDto dto = new ChangeAlbumInfoDto()
            {
                Id = Album.Id,
                Name = NameTextBox.Text.Trim(),
                AlbumCoverFilePath = _trackCoverPath,
                AddedTrackIdList = addedTrackIds,
                RemovedTrackIdList = removedTrackIds
            };
            
            try
            {
                _albumApiClient.UpdateAlbumAsync(dto);
                MessageBox.Show("Альбом обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Не удалось обновить альбом");
                Debug.WriteLine(exception);
            }
        }
    }

    private bool IsFieldsCorrect(out string errorMessage)
    {
        errorMessage = string.Empty;
        
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            errorMessage = "Введите название альбома";
        }
    
        if (Mode == FormMode.Add)
        {
            if (string.IsNullOrEmpty(_trackCoverPath) || !File.Exists(_trackCoverPath))
            {
                errorMessage = "Выберите обложку";
            }
        }
    
        if (TrackListBox.SelectedItems.Count == 0)
        {
            errorMessage = "Треки не выбраны";
        }

        if (errorMessage.Length > 0)
            return false;
        
        return true;
    }

    private void ChooseCoverFileButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Выберите обложку",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Filter = "Изображения (*.jpg, *.png)|*.jpg;*.png",
            FilterIndex = 1,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() is true)
        {
            _trackCoverPath = openFileDialog.FileName;
        
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; 
            bitmap.UriSource = new Uri(_trackCoverPath, UriKind.Absolute);
            bitmap.EndInit();
        
            CoverImage.Source = bitmap;
        }
    }
}