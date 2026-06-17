using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Data;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Enums;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class LibraryPage : Page
{
    private readonly IProfileApiClient _profileApiClient;
    private SortTracksEnum _sortEnum = SortTracksEnum.Default;
    
    private List<TrackInfoDto> _savedTracks;
    public LibraryPage()
    {
        InitializeComponent();
        _profileApiClient = new ProfileApiClient();
        WeakReferenceMessenger.Default.Register<SavedTracksTransferMessage>(this, (recipient, message) 
            => _savedTracks = message.savedTracks);
        WeakReferenceMessenger.Default.Register<LibraryChangedMessage>(this, (recipient, message) => LoadTrackListBox());
        Loaded += (s, e) => LoadTrackListBox();
    }
    
    public async void LoadTrackListBox()
    {
        if (!IsLoaded || TrackListBox == null)
            return;
        
        TrackListBox.Items.Clear();
        List<TrackInfoDto> tracks = new List<TrackInfoDto>();
        try
        {
            tracks = _savedTracks;
        }
        catch (Exception e)
        {
            MessageBox.Show("Не удалось прогрузить библиотеку");
            return;
        }

        AmountOfTrackTextBlock.Text = tracks.Count.ToString();
        
        //Да, switch бесполезен, пофиг, don't care
        switch (_sortEnum)
        {
            case SortTracksEnum.ByName:
                tracks = tracks.OrderBy(x => x.Name).ToList();
                break;
            case SortTracksEnum.ByAuthor:
                tracks = tracks.OrderBy(x => x.UserList.OrderBy(t => t.Nickname)).ToList();
                break;
            case SortTracksEnum.ByDate:
                tracks = tracks.OrderBy(x => x.Name).ToList();
                break;
            default: break;
        }
        
        foreach (TrackInfoDto track in tracks)
        {
            TrackControl control = new TrackControl(true);
            control.Track = track;
            control.Width = ActualWidth - 40;
            TrackListBox.Items.Add(control);
        }
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

    private void LibraryPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        foreach (TrackControl control in TrackListBox.Items)
        {
            control.Width = ActualWidth - 20;
        }
    }

    private void SortComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SortComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            string selectedContent = selectedItem.Content.ToString();

            switch (selectedContent)
            {
                case "По-умолчанию":
                    _sortEnum = SortTracksEnum.Default;
                    break;
                case "По названию":
                    _sortEnum = SortTracksEnum.ByName;
                    break;
                case "По дате добавления":
                    _sortEnum = SortTracksEnum.ByDate;
                    break;
                case "По исполнителю":
                    _sortEnum = SortTracksEnum.ByAuthor;
                    break;
            }
        }
        LoadTrackListBox();
    }

    private void PlayLibraryButton_OnClick(object sender, RoutedEventArgs e)
    {
        var tracks = TrackListBox.Items
            .Cast<TrackControl>()
            .Where(c => c is not null)
            .Select(c => c.Track)
            .ToList();
        PlaybackService.SetQueue(tracks);
    }
}