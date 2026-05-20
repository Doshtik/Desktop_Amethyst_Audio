using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using System.Windows.Controls;

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
        List<AlbumInfoDto> albums = await _albumApiClient.GetAllAsync();
        albums = albums
            .OrderByDescending(x => x.Id)
            .Take(10)
            .ToList();
        
    }
}