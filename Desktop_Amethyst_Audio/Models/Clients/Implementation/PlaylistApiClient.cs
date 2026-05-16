using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class PlaylistApiClient : IPlaylistApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5278";
    
    private const string PLAYLIST_API_PATH = "api/playlists";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };
    
    public async Task<PlaylistInfoDto> GetPlaylistByIdAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlaylistInfoDto>(json, JsonOptions);
    }

    public async Task<List<PlaylistInfoDto>> GetPlaylistAllAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PlaylistInfoDto>>(json, JsonOptions);
    }

    public async Task<PlaylistInfoDto> GetListPlaylistByUserIdAsync(long userId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/user/{userId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlaylistInfoDto>(json, JsonOptions);
    }

    public async Task<BitmapImage> GetPlaylistCoverAsync(string coverUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, coverUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
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

    public async Task<PlaylistInfoDto> CreatePlaylistAsync(CreatePlaylistDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();
        
        byte[] coverBytes = await File.ReadAllBytesAsync(dto.CoverFilePath);
        ByteArrayContent coverFileContent = new ByteArrayContent(coverBytes);
        coverFileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        formData.Add(coverFileContent, "CoverFile", Path.GetFileName(dto.CoverFilePath));
        
        formData.Add(new StringContent(dto.UserId.ToString()), "UserId");
        formData.Add(new StringContent(dto.IsPublic.ToString().ToLowerInvariant()), "IsPublic");
        formData.Add(new StringContent(dto.Name), "Name");
        formData.Add(new StringContent(dto.Description), "Description");
        formData.Add(new StringContent(JsonSerializer.Serialize(dto.TracksIdList)), "TracksIdList");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";
    
        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
        {
            Content = formData
        };
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlaylistInfoDto>(json, JsonOptions);
    }

    public async Task<PlaylistInfoDto> UpdatePlaylistAsync(ChangePlaylistInfoDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();

        if (dto.CoverFilePath is not null)
        {
            byte[] coverBytes = await File.ReadAllBytesAsync(dto.CoverFilePath);
            ByteArrayContent coverFileContent = new ByteArrayContent(coverBytes);
            coverFileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(coverFileContent, "CoverFile", Path.GetFileName(dto.CoverFilePath));
        }
        
        formData.Add(new StringContent(dto.IsPublic.ToString().ToLowerInvariant()), "IsPublic");
        formData.Add(new StringContent(dto.Name), "Name");
        formData.Add(new StringContent(dto.Description), "Description");
        formData.Add(new StringContent(JsonSerializer.Serialize(dto.AddedTracksIdList)), "AddedTracksIdList");
        formData.Add(new StringContent(JsonSerializer.Serialize(dto.RemovedTracksIdList)), "RemovedTracksIdList");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";
    
        using var request = new HttpRequestMessage(HttpMethod.Put, fullUrl)
        {
            Content = formData
        };
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlaylistInfoDto>(json, JsonOptions);
    }

    public async Task DeletePlaylistAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}";

        using var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task SavePlaylistAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}/save";

        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnsavePlaylistAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PLAYLIST_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}/save";

        using var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}