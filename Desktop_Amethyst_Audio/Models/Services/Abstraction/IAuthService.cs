using Backend_Amethyst_Audio.DTO;

namespace Desktop_Amethyst_Audio.Models.Services.Abstraction;

public interface IAuthService
{
    Task<bool> TryAutoLoginAsync();
    Task<UserInfoDto> LoginAsync(LoginDto dto);
    Task<UserInfoDto> RegistrationAsync(CreateUserDto dto);
    //ResetPassword
}