using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Reports;
using Desktop_Amethyst_Audio.Models.DTO.Reports.ReportAnswer;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class ReportApiClient : IReportApiClient
{
    private static readonly HttpClient _httpClient = new();
    
    private static readonly SettingsService _settingsService = new();

    private static string BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5278";
    
    private const string RECOMMENDATION_API_PATH = "api/reports";
    
    private static readonly JsonSerializerOptions JsonOptions = new() 
    { 
        PropertyNameCaseInsensitive = true 
    };
    
    public async Task<ReportInfoDto> GetByIdAsync(long reportId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{reportId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ReportInfoDto>(json, JsonOptions);
    }

    public async Task<List<ReportInfoDto>> GetAllAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ReportInfoDto>>(json, JsonOptions);
    }

    public async Task<ReportInfoDto> CreateAsync(CreateReportDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}";
    
        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
        {
            Content = content
        };
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ReportInfoDto>(responseJson, JsonOptions);
    }

    public async Task DeleteAsync(long reportId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/{reportId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ReportAnswerInfoDto> GetAnswerByIdAsync(long reportAnswerId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/answer/{reportAnswerId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ReportAnswerInfoDto>(json, JsonOptions);
    }

    public async Task<List<ReportAnswerInfoDto>> GetAnswerAllAsync()
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/answer/";
    
        using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
    
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settingsService.Load().User.Token);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ReportAnswerInfoDto>>(json, JsonOptions);
    }

    public async Task<ReportAnswerInfoDto> CreateAnswerAsync(CreateReportAnswerDto dto)
    {
        var json = JsonSerializer.Serialize(dto);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/answer";
    
        using var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
        {
            Content = content
        };
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ReportAnswerInfoDto>(responseJson, JsonOptions);
    }

    public async Task DeleteAnswerAsync(long reportAnswerId)
    {
        var baseUrl = BaseUrl.TrimEnd('/');
        var path = RECOMMENDATION_API_PATH.TrimStart('/');
        var fullUrl = $"{baseUrl}/{path}/answer/{reportAnswerId}";
    
        using var request = new HttpRequestMessage(HttpMethod.Delete, fullUrl);
    
        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}