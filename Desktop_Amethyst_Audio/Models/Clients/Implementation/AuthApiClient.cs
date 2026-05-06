using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class AuthApiClient : IAuthApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    
    private const string ALBUM_API_PATH = "/api/auth/";
    
    public async Task<UserInfoDto> RegisterAsync(CreateUserDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(dto.Nickname), "Nickname");
        formData.Add(new StringContent(dto.Email), "Email");
        formData.Add(new StringContent(dto.Password), "Password");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/signin";
    
        var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
    
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
    }

    public async Task<UserInfoDto> LoginUserAsync(LoginDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(dto.Email), "Email");
        formData.Add(new StringContent(dto.Password), "Password");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/login";
    
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
    }

    public async Task<UserInfoDto> ExternalLoginAsync(ExternalLoginDto dto)
    {
        using MultipartFormDataContent formData = new MultipartFormDataContent();
        
        formData.Add(new StringContent(dto.Provider), "Provider");
        formData.Add(new StringContent(dto.Token), "Token");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = ALBUM_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/external-login";
    
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(json, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
    }

    public Task RefreshTokenAsync()
    {
        throw new NotImplementedException();
    }

    public Task LogoutUserAsync()
    {
        throw new NotImplementedException();
    }
}