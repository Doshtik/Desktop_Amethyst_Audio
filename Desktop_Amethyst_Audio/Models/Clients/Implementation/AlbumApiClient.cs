using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class AlbumApiClient : IAlbumApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5278";
    
    private const string ALBUM_API_PATH = "api/albums";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };

    #region Endpoints

    public async Task<AlbumInfoDto> GetByIdAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}";

        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AlbumInfoDto>(json, JsonOptions);
    }
    
    public async Task<List<AlbumInfoDto>> GetAllAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";

        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<AlbumInfoDto>>(json, JsonOptions);
    }

    public async Task<List<AlbumInfoDto>> GetListByUserIdAsync(long userId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/user/{userId}";

        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<AlbumInfoDto>>(json, JsonOptions);
    }

    public async Task<BitmapImage> GetAlbumCoverAsync(string coverUrl)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, coverUrl);
    
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

    public async Task<AlbumInfoDto> CreateAlbumAsync(CreateAlbumDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();
    
        byte[] fileBytes = await File.ReadAllBytesAsync(dto.AlbumCoverFilePath);
        ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
        
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        
        formData.Add(fileContent, "CoverFile", Path.GetFileName(dto.AlbumCoverFilePath));
        formData.Add(new StringContent(dto.Name), "Name");
        formData.Add(new StringContent(JsonSerializer.Serialize(dto.AuthorsIdList)), "AuthorsIdList");
        formData.Add(new StringContent(JsonSerializer.Serialize(dto.TracksIdList)), "TracksIdList");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";
    
        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
        {
            Content = formData
        };
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AlbumInfoDto>(json, JsonOptions);
    }

    public async Task<AlbumInfoDto> UpdateAlbumAsync(ChangeAlbumInfoDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();

        if (dto.AlbumCoverFilePath is not null)
        {
            byte[] fileBytes = await File.ReadAllBytesAsync(dto.AlbumCoverFilePath);
            ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
        
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(fileContent, "CoverFile", Path.GetFileName(dto.AlbumCoverFilePath));
        }
        formData.Add(new StringContent(dto.Name), "Name");
        formData.Add(new StringContent(JsonSerializer.Serialize(dto.AddedTrackIdList)), "AddedTrackIdList");
        formData.Add(new StringContent(JsonSerializer.Serialize(dto.RemovedTrackIdList)), "RemovedTrackIdList");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{dto.Id}";
    
        var request = new HttpRequestMessage(HttpMethod.Put, fullUrl)
        {
            Content = formData
        };
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AlbumInfoDto>(json, JsonOptions);
    }

    public async Task DeleteAlbumAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}";

        var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
    
    public async Task<bool> IsAlbumSavedAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}/save";

        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        bool result = await response.Content.ReadFromJsonAsync<bool>();
        return result;
    }
    
    public async Task SaveAlbumAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}/save";

        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnsaveAlbumAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}/save";

        var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    #endregion
}