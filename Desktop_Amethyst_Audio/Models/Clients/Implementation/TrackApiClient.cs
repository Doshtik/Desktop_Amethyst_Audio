using System.IO;
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

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5278";
    
    private const string TRACK_API_PATH = "api/tracks";
    
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

    public async Task<Stream> GetTrackFileAsync(string trackFileUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, trackFileUrl);
    
        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
    
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task<BitmapImage> GetTrackCoverAsync(string trackCoverUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, trackCoverUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var imageBytes = await response.Content.ReadAsByteArrayAsync();
    
        return LoadBitmapFromBytes(imageBytes);
    }
    
    private BitmapImage LoadBitmapFromBytes(byte[] bytes)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = new MemoryStream(bytes);
        bitmap.EndInit();
        bitmap.Freeze(); // Делаем потокобезопасным для WPF (обязательно для async!)
        return bitmap;
    }
}