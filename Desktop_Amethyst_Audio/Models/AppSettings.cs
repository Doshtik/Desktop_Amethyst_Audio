using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models.Enums;

namespace Desktop_Amethyst_Audio.Models;

public class AppSettings
{
    // Настройки пользователя
    public UserInfoDto? User { get; set; }
    
    // Настройки интерфейса
    public string Language { get; set; } = "ru-RU";
    public AppTheme Theme { get; set; } = AppTheme.Dark;
    
    public AppSettings() { }
}