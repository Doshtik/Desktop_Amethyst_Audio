using System.Net.Http;
using System.Windows;
using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class AuthService : IAuthService
{
    //private readonly IAuthApiClient _authApiClient = new AuthApiClient();
    private readonly ISettingsService _settingsService = new SettingsService();
    
    public async Task<bool> TryAutoLoginAsync()
    {
        AppSettings settings = _settingsService.Load();
        
        if (settings.User is null)
            return false;
        
        try
        {
            UserInfoDto? userFromApi = null; // await _authApiClient.GetUserByIdAsync(settings.User.Id);

            if (userFromApi is not null)
            {
                settings.User = userFromApi;
                _settingsService.Save(settings);
                return true;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
        
        settings.User = null; 
        _settingsService.Save(settings);
        return false;
    }

    public async Task<UserInfoDto> LoginAsync(LoginDto dto)
    {
        //UserInfoDto dto = await _authApiClient.LoginAsync(dto);
        throw new NotImplementedException();
    }

    public async Task<UserInfoDto> RegistrationAsync(CreateUserDto dto)
    {
        throw new NotImplementedException();
    }
}