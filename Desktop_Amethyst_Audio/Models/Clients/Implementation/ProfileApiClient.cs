using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class ProfileApiClient : IProfileApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5278";
    
    private const string PROFILE_API_PATH = "api/profiles";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };
    
    public async Task<UserInfoDto> GetUserByIdAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(json, JsonOptions);
    }

    public async Task<List<UserInfoDto>> GetUserAllAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<UserInfoDto>>(json, JsonOptions);
    }

    public async Task<int> GetAmountOfSubsAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/sub-count/{id}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<UserInfoDto> UpdateUserAsync(ChangeUserInfoDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();

        if (dto.AvatarFilePath is not null)
        {
            byte[] avatarBytes = await File.ReadAllBytesAsync(dto.AvatarFilePath);
            ByteArrayContent avatarFileContent = new ByteArrayContent(avatarBytes);
        
            avatarFileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(avatarFileContent, "AvatarFile", Path.GetFileName(dto.AvatarFilePath));
        }

        if (dto.HeaderFilePath is not null)
        {
            byte[] headerBytes = await File.ReadAllBytesAsync(dto.HeaderFilePath);
            ByteArrayContent headerFileContent = new ByteArrayContent(headerBytes);
        
            headerFileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(headerFileContent, "HeaderFile", Path.GetFileName(dto.HeaderFilePath));
        }
        
        formData.Add(new StringContent(dto.Lastname), "Lastname");
        formData.Add(new StringContent(dto.Firstname), "Firstname");
        formData.Add(new StringContent(dto.Nickname), "Nickname");
        formData.Add(new StringContent(dto.Email), "Email");
        formData.Add(new StringContent(dto.Gender), "Gender");
        formData.Add(new StringContent(dto.Country), "Country");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{_settingsService.Load().User.Id}";
    
        var request = new HttpRequestMessage(HttpMethod.Put, fullUrl)
        {
            Content = formData
        };
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(json, JsonOptions);
    }

    public async Task DeleteUserAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{id}";
    
        var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<UserHistoryDto>> GetUserHistoryAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/history/";
    
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<UserHistoryDto>>(json, JsonOptions);
    }

    public async Task AddToHistoryAsync(long trackId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/history/{trackId}";
    
        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateListeningTimeAsync(long trackId, int seconds)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/history/{trackId}?seconds={seconds}";
    
        var request = new HttpRequestMessage(HttpMethod.Put, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
        
    public async Task<List<TrackInfoDto>> GetUserLibraryAsync()
    {
        var settings = _settingsService.Load();
        var user = settings?.User;
        
        Console.WriteLine($"[AUTH] settings is null: {settings == null}");
        Console.WriteLine($"[AUTH] user is null: {user == null}");
        Console.WriteLine($"[AUTH] user.Token: '{user?.Token}'");
        Console.WriteLine($"[AUTH] user.Token is null/empty: {string.IsNullOrWhiteSpace(user?.Token)}");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/library/{user.Id}";
    
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TrackInfoDto>>(json, JsonOptions);
    }

    public async Task AddTrackToUserLibraryAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/library/track/{id}";
    
        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveTrackFromUserLibraryAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/library/track/{id}";
    
        var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<PlaylistInfoDto>> GetUserSavedPlaylistsAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/library/playlists/{id}";
    
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PlaylistInfoDto>>(json, JsonOptions);
    }

    public async Task<List<AlbumInfoDto>> GetUserSavedAlbumsAsync(long id)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/library/albums/{id}";
    
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<AlbumInfoDto>>(json, JsonOptions);
    }

    public async Task<BitmapImage> GetUserAvatarAsync(string avatarUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, avatarUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var imageBytes = await response.Content.ReadAsByteArrayAsync();
    
        return LoadBitmapFromBytes(imageBytes);
    }

    public async Task<BitmapImage> GetUserHeaderAsync(string headerUrl)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, headerUrl);
    
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

    public async Task<bool> IsUserFollowedAsync(long targetId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/subscription/{targetId}";

        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        bool result = await response.Content.ReadFromJsonAsync<bool>();
        return result;
    }

    public async Task FollowUserAsync(long targetId)
    {
        object tmpObj = new
        {
            IdTargetUser = targetId,
            IdSubscriber = _settingsService.Load().User.Id
        };
        var json = JsonSerializer.Serialize(tmpObj);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/subscription";
    
        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
        {
            Content = content
        };
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnfollowUserAsync(long targetId)
    {
        object tmpObj = new
        {
            IdTargetUser = targetId,
            IdSubscriber = _settingsService.Load().User.Id
        };
        var json = JsonSerializer.Serialize(tmpObj);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = PROFILE_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/subscription";
    
        using var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl)
        {
            Content = content
        };
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}