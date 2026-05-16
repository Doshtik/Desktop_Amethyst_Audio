using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5278";
    
    private const string AUTH_API_PATH = "api/auth";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };
    
    public async Task<UserInfoDto> RegisterAsync(CreateUserDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = AUTH_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/signin";
    
        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
        {
            Content = content
        };
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(responseJson, JsonOptions);
    }

    public async Task<UserInfoDto> LoginUserAsync(LoginDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = AUTH_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/login";
    
        using var response = await _httpClient.PostAsync(fullUrl, content);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(responseJson, JsonOptions);
    }

    public async Task<UserInfoDto> ExternalLoginAsync(ExternalLoginDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = AUTH_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/external-login";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl)
        {
            Content = content
        };
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserInfoDto>(responseJson, JsonOptions);
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