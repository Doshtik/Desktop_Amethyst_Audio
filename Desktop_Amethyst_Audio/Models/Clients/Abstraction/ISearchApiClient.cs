using Desktop_Amethyst_Audio.Models.DTO;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface ISearchApiClient
{
    Task<List<GenreInfoDto>> GetGenresAsync();
    Task<List<TrackInfoDto>> GetListByGenreAsync(string genreName);
    Task<SearchInfoDto> GetBySearchAsync(string searchLine);
}