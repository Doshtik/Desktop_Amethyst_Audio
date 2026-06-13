using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Windows;
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
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/";

        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);

        // Создаем multipart form data
        using var content = new MultipartFormDataContent();
        
        // Добавляем текстовые поля
        content.Add(new StringContent(dto.Name), "Name");
        string authorsJson = JsonSerializer.Serialize(dto.AuthorsIdList);
        content.Add(new StringContent(authorsJson, Encoding.UTF8, "application/json"), "AuthorsIdList");
        content.Add(new StringContent(dto.PaceId.ToString()), "PaceId");
        content.Add(new StringContent(dto.MoodId.ToString()), "MoodId");
        string genresJson = JsonSerializer.Serialize(dto.GenresIdList);
        content.Add(new StringContent(genresJson, Encoding.UTF8, "application/json"), "GenresIdList");
        
        if (dto.IsTextless.HasValue)
            content.Add(new StringContent(dto.IsTextless.Value.ToString()), "IsTextless");
        
        if (dto.IsExplicit.HasValue)
            content.Add(new StringContent(dto.IsExplicit.Value.ToString()), "IsExplicit");
        
        if (!string.IsNullOrEmpty(dto.Country))
            content.Add(new StringContent(dto.Country), "Country");

        // Добавляем файлы
        if (!string.IsNullOrEmpty(dto.CoverFilePath) && File.Exists(dto.CoverFilePath))
        {
            var coverStream = File.OpenRead(dto.CoverFilePath);
            var coverContent = new StreamContent(coverStream);
            coverContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(coverContent, "CoverFile", Path.GetFileName(dto.CoverFilePath));
        }

        if (!string.IsNullOrEmpty(dto.TrackFilePath) && File.Exists(dto.TrackFilePath))
        {
            var trackStream = File.OpenRead(dto.TrackFilePath);
            var trackContent = new StreamContent(trackStream);
            trackContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
            content.Add(trackContent, "TrackFile", Path.GetFileName(dto.TrackFilePath));
        }

        request.Content = content;

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TrackInfoDto>(responseJson, JsonOptions);
    }

    public async Task<TrackInfoDto> UpdateAsync(long trackId, ChangeTrackInfoDto dto)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(dto.Id.ToString()), "Id");
        content.Add(new StringContent(dto.Name ?? ""), "Name");
        content.Add(new StringContent(dto.PaceId.ToString()), "PaceId");
        content.Add(new StringContent(dto.MoodId.ToString()), "MoodId");
        
        if (dto.IsExplicit.HasValue) 
            content.Add(new StringContent(dto.IsExplicit.Value.ToString().ToLower()), "IsExplicit");
        if (dto.IsTextless.HasValue) 
            content.Add(new StringContent(dto.IsTextless.Value.ToString().ToLower()), "IsTextless");
        if (!string.IsNullOrEmpty(dto.Country)) 
            content.Add(new StringContent(dto.Country), "Country");
        content.Add(new StringContent(string.Join(",", dto.GenresIdList)), "GenresIdList");
        
        if (!string.IsNullOrEmpty(dto.CoverFilePath) && File.Exists(dto.CoverFilePath))
        {
            var coverStream = File.OpenRead(dto.CoverFilePath);
            var coverContent = new StreamContent(coverStream);
            coverContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(coverContent, "CoverFile", Path.GetFileName(dto.CoverFilePath));
        }

        if (!string.IsNullOrEmpty(dto.TrackFilePath) && File.Exists(dto.TrackFilePath))
        {
            var trackStream = File.OpenRead(dto.TrackFilePath);
            var trackContent = new StreamContent(trackStream);
            trackContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");
            content.Add(trackContent, "TrackFile", Path.GetFileName(dto.TrackFilePath));
        }

        var baseUrl = BaseUrl.TrimEnd('/');
        var path = TRACK_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{trackId}";

        using var request = new HttpRequestMessage(HttpMethod.Put, fullUrl)
        {
            Content = content
        };

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

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

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