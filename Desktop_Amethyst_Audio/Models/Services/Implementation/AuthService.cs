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
        AppSettings settings = _settingsService.Load();
        
        if (settings.User is null)
            return false;
        
        try
        {
            UserInfoDto? userFromApi = await _profileApiClient.GetUserByIdAsync(settings.User.Id);

            if (userFromApi is not null)
            {
                settings.User = userFromApi;
                _settingsService.Save(settings);
                return true;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        settings.User = null; 
        _settingsService.Save(settings);
        return false;
    }
}