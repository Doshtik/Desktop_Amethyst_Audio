using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IAuthApiClient
{
    Task<UserInfoDto> RegisterAsync(CreateUserDto dto);
    Task<UserInfoDto> LoginUserAsync(LoginDto dto);
    Task<UserInfoDto> ExternalLoginByGoogleAsync(ExternalLoginDto dto);
    Task<UserInfoDto> ExternalLoginByYandexAsync(ExternalLoginDto dto);
    Task RefreshTokenAsync();
    Task LogoutUserAsync();
}