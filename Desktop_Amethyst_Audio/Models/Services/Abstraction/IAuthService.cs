using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Models.Services.Abstraction;

public interface IAuthService
{
    Task<bool> TryAutoLoginAsync();
}