using System.Diagnostics;
using System.Windows;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using System.Windows.Controls;
using Backend_Amethyst_Audio.DTO;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class SearchPage : Page
{
    private List<AlbumInfoDto> _albums = new List<AlbumInfoDto>();
    private readonly ISearchApiClient _searchApiClient;
    private readonly IAlbumApiClient _albumApiClient;
    public SearchPage()
    {
        InitializeComponent();
        _searchApiClient = new SearchApiClient();
        _albumApiClient = new AlbumApiClient();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            _albums = await _albumApiClient.GetAllAsync();
            AlbumListBox.Items.Clear();
            foreach (AlbumInfoDto albumItem in _albums)
            {
                AlbumControl control = new();
                control.Album = albumItem;
                control.Height = 160;
                control.Width = 130;
                AlbumListBox.Items.Add(control);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        
        try
        {
            List<GenreInfoDto> genres = await _searchApiClient.GetGenresAsync();
            GenreListBox.Items.Clear();
            foreach (GenreInfoDto genreItem in genres)
            {
                GenreControl control = new();
                control.Genre = genreItem;
                control.Width = 250;
                control.Height = 75;
                GenreListBox.Items.Add(control);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new NavigateToSearchResultMessage(SearchBarTextBox.Text.Trim()));
    }

    private void AlbumListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        AlbumControl control = AlbumListBox.SelectedItem as AlbumControl;
        WeakReferenceMessenger.Default.Send(new NavigateToAlbumMessage(control.Album,  false));
    }

    private void GenreListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }
}