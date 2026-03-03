using System.Configuration;
using System.Data;
using System.Windows;
using Desktop_Amethyst_Audio.Views.Windows;

namespace Desktop_Amethyst_Audio;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Твоя логика проверки (например, через сервис или настройки)
        bool isAuthenticated = true; //CheckUserAuth(); 

        if (isAuthenticated)
        {
            // Показываем основное окно
            new LayoutWindow().Show();
        }
        else
        {
            // Показываем окно входа
            var loginWindow = new AuthWindow();
        
            // Если нужно открыть главное окно ПОСЛЕ успешного входа:
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