using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.ModalWindow;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages.Select;

public partial class TrackSelectorPage : Page
{
    private ITrackApiClient _trackApiClient;
    private ISettingsService _settingsService;
    
    private List<TrackInfoDto> _selectedTracks;
    
    public TrackSelectorPage(List<TrackInfoDto> selectedTracks)
    {
        InitializeComponent();
        _trackApiClient = new TrackApiClient();
        _settingsService = new SettingsService();
        _selectedTracks = selectedTracks;
    }

    private async void TrackSelectorPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        AppSettings settings = _settingsService.Load();
        try
        {
            List<TrackInfoDto> tracks = await _trackApiClient.GetAllAsync();
            LoadTrackListBox(tracks);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Debug.WriteLine(exception);
        }
    }

    private void LoadTrackListBox(List<TrackInfoDto> tracks)
    {
        TrackListBox.Items.Clear();
        TrackListBox.SelectedItems.Clear();
        foreach (TrackInfoDto track in tracks)
        {
            TrackControl control = new TrackControl(false);
            control.Track = track;
            control.Width = TrackListBox.ActualWidth;
            TrackListBox.Items.Add(control);
        }
    }

    private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (TrackControl control in TrackListBox.SelectedItems)
        {
            _selectedTracks.Add(control.Track);
        }
        WeakReferenceMessenger.Default.Send(new CloseFrameMessage());
    }

    private void BackButton_OnClick(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new CloseFrameMessage());
    }

    private async void SearchButton_OnClick(object sender, RoutedEventArgs e)
    {
        AppSettings settings = _settingsService.Load();
        try
        {
            List<TrackInfoDto> tracks = await _trackApiClient.GetListByTrackNameAsync(TrackNameTextBox.Text.Trim());
            LoadTrackListBox(tracks);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }
}