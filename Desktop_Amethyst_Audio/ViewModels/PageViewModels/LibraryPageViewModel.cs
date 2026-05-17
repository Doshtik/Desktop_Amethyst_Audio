using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.ViewModels.PageViewModels;

public partial class LibraryPageViewModel: ObservableObject
{
    [ObservableProperty] private List<TrackControl> _controls; 
    private readonly IProfileApiClient _profileApiClient;
    
    [ObservableProperty] private double _pageWidth;

    public LibraryPageViewModel()
    {
        _controls = new List<TrackControl>();
        _profileApiClient = new ProfileApiClient();
        LoadTrackListBox();
    }
    
    [RelayCommand]
    public async void LoadTrackListBox()
    {
        Controls.Clear();
        List<TrackInfoDto> tracks = await _profileApiClient.GetUserLibraryAsync();

        foreach (TrackInfoDto track in tracks)
        {
            TrackControl control = new TrackControl();
            control.Track = track;
            Controls.Add(control);
        }
    }
}