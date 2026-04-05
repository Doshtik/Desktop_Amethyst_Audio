using System.Net.Http;
using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient = new HttpClient(); 
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
    
    public async Task<bool> TryAutoLoginAsync()
    {
        /*
         * Здесь мы должны взять данные из appsettings.json
         * Если User пустой - вернуть false
         * Если User заполнен:
             * Проверяем наличие пользователя в системе, отправив запрос в API
             * Если пользователь есть -
                * вернуть true
             * Если нет:
                * Удаляем сведения о пользователе из appsettings.json
                * возвращаем false
        */
        throw new NotImplementedException();
    }

    public async Task<UserInfoDto> LoginAsync(LoginDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<UserInfoDto> RegistrationAsync(CreateUserDto dto)
    {
        throw new NotImplementedException();
    }
}