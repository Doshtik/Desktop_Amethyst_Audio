namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface ISearchApiClient
{
    Task GetGenres();
    Task GetListByGenre(string genreName);
    Task GetBySearch(string searchLine);
}