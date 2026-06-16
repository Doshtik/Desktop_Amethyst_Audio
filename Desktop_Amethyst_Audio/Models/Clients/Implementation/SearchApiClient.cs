using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class SearchApiClient : ISearchApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5278";
    
    private const string SEARCH_API_PATH = "api/search";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };
    
    public async Task<List<GenreInfoDto>> GetGenresAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = SEARCH_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/genres";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<GenreInfoDto>>(json, JsonOptions);
    }

    public async Task<List<TrackInfoDto>> GetListByGenreAsync(string genreName)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = SEARCH_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/genres/{genreName}";

        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TrackInfoDto>>(responseJson, JsonOptions);
    }

    public async Task<SearchInfoDto> GetBySearchAsync(string searchLine)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = SEARCH_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{searchLine}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SearchInfoDto>(responseJson, JsonOptions);
    }
}