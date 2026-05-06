using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IAuthApiClient
{
    Task<UserInfoDto> RegisterAsync(CreateUserDto dto);
    Task<UserInfoDto> LoginUserAsync(LoginDto dto);
    Task<UserInfoDto> ExternalLoginAsync(ExternalLoginDto dto);
    Task RefreshTokenAsync();
    Task LogoutUserAsync();
}