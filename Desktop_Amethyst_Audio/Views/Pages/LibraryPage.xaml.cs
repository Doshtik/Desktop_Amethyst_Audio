using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class LibraryPage : Page
{
    private readonly IProfileApiClient _profileApiClient;
    public LibraryPage()
    {
        InitializeComponent();
        _profileApiClient = new ProfileApiClient();
    }

    private void LibraryPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadTrackListBox();
    }
    
    public async void LoadTrackListBox()
    {
        TrackListBox.Items.Clear();
        List<TrackInfoDto> tracks = await _profileApiClient.GetUserLibraryAsync();

        foreach (TrackInfoDto track in tracks)
        {
            TrackControl control = new TrackControl();
            control.Track = track;
            control.Width = ActualWidth - 20;
            TrackListBox.Items.Add(control);
        }
    }

    private void TrackListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TrackControl? control = TrackListBox.SelectedItem as TrackControl;
        //WeakReferenceMessenger.Default.Send(new TrackChangedMessage(control.Track));
    }

    private void LibraryPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        foreach (TrackControl control in TrackListBox.Items)
        {
            control.Width = ActualWidth - 20;
        }
    }

    private void SortComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        throw new NotImplementedException();
    }
}