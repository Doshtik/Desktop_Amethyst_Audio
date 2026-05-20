using System.Net.Http;
using System.Windows;
using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IProfileApiClient _profileApiClient = new ProfileApiClient();
    private readonly ISettingsService _settingsService = new SettingsService();
    
    public async Task<bool> TryAutoLoginAsync()
    {
        var settings = _settingsService.Load();
    
        if (settings.User is null || string.IsNullOrWhiteSpace(settings.User.Token))
            return false;
    
        try
        {
            var userFromApi = await _profileApiClient.GetUserByIdAsync(settings.User.Id);

            if (userFromApi is not null)
            {
                var token = settings.User.Token;
                settings.User = userFromApi;
                settings.User.Token = token;
                _settingsService.Save(settings);
                return true;
            }
        
            return false;
        }
        catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized 
                                                  || httpEx.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            Console.WriteLine($"[AutoLogin] Token expired or invalid: {httpEx.Message}");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("[AutoLogin] Request timeout, keeping user data");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AutoLogin] Temporary error, keeping user data: {ex.Message}");
            return false;
        }
    
        settings.User = null;
        _settingsService.Save(settings);
        return false;
    }
}