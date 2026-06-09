using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class SearchResultPage : Page
{
    private string _SearchQuery;
    private ISearchApiClient _searchApiClient;

    private List<UserInfoDto> _users;
    private List<UserInfoDto> _tracks;
    private List<UserInfoDto> _albums;
    private List<UserInfoDto> _playlists;
    
    public SearchResultPage(string searchLine)
    {
        InitializeComponent();
        _searchApiClient = new SearchApiClient();
        _SearchQuery = searchLine;
    }

    private void SearchResultPage_OnLoaded(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}