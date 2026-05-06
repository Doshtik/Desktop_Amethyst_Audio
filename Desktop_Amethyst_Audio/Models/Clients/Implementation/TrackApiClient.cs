using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class TrackApiClient : ITrackApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    
    private const string TRACK_API_PATH = "/api/tracks/";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };
    
    public async Task<TrackInfoDto> GetByIdAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TrackInfoDto>(json, JsonOptions);
    }

    public async Task<List<TrackInfoDto>> GetAllAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TrackInfoDto>>(json, JsonOptions);
    }

    public async Task<List<TrackInfoDto>> GetListByUserIdAsync(long userId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/user/{userId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TrackInfoDto>>(json, JsonOptions);
    }

    public async Task<TrackInfoDto> CreateAsync(CreateTrackDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/";
    
        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TrackInfoDto>(responseJson, JsonOptions);
    }

    public async Task<TrackInfoDto> UpdateAsync(long trackId, ChangeTrackInfoDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{trackId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Put, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TrackInfoDto>(responseJson, JsonOptions);
    }

    public async Task DeleteAsync(long trackId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{trackId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<byte[]> GetTrackFileAsync(string trackFileUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, trackFileUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<byte[]>(responseJson, JsonOptions);
    }

    public async Task<BitmapImage> GetTrackCoverAsync(string trackCoverUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, trackCoverUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BitmapImage>(responseJson, JsonOptions);
    }
}