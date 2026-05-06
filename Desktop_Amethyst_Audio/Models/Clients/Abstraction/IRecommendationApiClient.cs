using Desktop_Amethyst_Audio.Models.DTO.Pages;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IRecommendationApiClient
{
    Task<ResonanceConfigDto> GetRecommendationConfig();
    Task<List<TrackInfoDto>> GetPersonalizedRecommendationsAsync(PageResonanceDto dto);
}