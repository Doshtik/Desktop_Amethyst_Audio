using System.Diagnostics;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using System.Windows.Controls;
using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class SearchPage : Page
{
    private readonly ISearchApiClient _searchApiClient;
    private readonly IAlbumApiClient _albumApiClient;
    public SearchPage()
    {
        InitializeComponent();
        _searchApiClient = new SearchApiClient();
        _albumApiClient = new AlbumApiClient();
    }

    private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            List<AlbumInfoDto> albums = await _albumApiClient.GetAllAsync();
            AlbumListBox.Items.Clear();
            foreach (AlbumInfoDto albumItem in albums)
            {
                AlbumControl control = new();
                control.Album = albumItem;
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
                GenreListBox.Items.Add(control);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}