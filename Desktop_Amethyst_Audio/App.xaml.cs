using System.Configuration;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Windows;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.Windows;

namespace Desktop_Amethyst_Audio;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IAuthService _authService;
    protected async override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _authService = new AuthService();
        
        // Логика проверки (например, через сервис или настройки)
        bool isAuthenticated = await _authService.TryAutoLoginAsync();
        
        if (isAuthenticated)
        {
            // Показываем основное окно
            new LayoutWindow().Show();
        }
        else
        {
            // Показываем окно входа
            var loginWindow = new AuthWindow();
        
            // Главное окно ПОСЛЕ успешного входа:
            if (loginWindow.ShowDialog() == true) 
            {
                new LayoutWindow().Show();
            }
            else
            {
                // Если пользователь закрыл окно входа, закрываем приложение
                Shutdown();
            }
        }
    }
}