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

public partial class OwnerTrackSelectorPage : Page
{
    private ITrackApiClient _trackApiClient;
    private ISettingsService _settingsService;
    
    private List<TrackInfoDto> _selectedTracks;
    
    public OwnerTrackSelectorPage(List<TrackInfoDto> selectedTracks)
    {
        InitializeComponent();
        _trackApiClient = new TrackApiClient();
        _settingsService = new SettingsService();
        _selectedTracks = selectedTracks;
    }

    private async void OwnerTrackSelectorPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        AppSettings settings = _settingsService.Load();
        List<TrackInfoDto> tracks = new List<TrackInfoDto>();
        try
        {
            tracks = await _trackApiClient.GetListByUserIdAsync(settings.User.Id);
            TrackListBox.Items.Clear();
            foreach (TrackInfoDto track in tracks)
            {
                TrackControl control = new TrackControl(true);
                control.Track = track;
                control.Width = TrackListBox.ActualWidth;
                TrackListBox.Items.Add(control);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Debug.WriteLine(ex);
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
}