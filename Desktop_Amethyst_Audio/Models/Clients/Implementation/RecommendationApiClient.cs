using System.Net.Http;
using System.Text;
using System.Text.Json;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Pages;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class RecommendationApiClient : IRecommendationApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    
    private const string RECOMMENDATION_API_PATH = "/api/profiles/";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };
    
    public async Task<ResonanceConfigDto> GetRecommendationConfig()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/config";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ResonanceConfigDto>(json, JsonOptions);
    }

    public async Task<List<TrackInfoDto>> GetPersonalizedRecommendationsAsync(PageResonanceDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/query";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TrackInfoDto>>(responseJson, JsonOptions);
    }
}