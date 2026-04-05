using System.IO;
using System.Text.Json;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class SettingsService : ISettingsService
{
    private readonly string _folderPath;
    private readonly string _filePath;
    
    public SettingsService()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _folderPath = Path.Combine(appDataPath, "Amethyst Audio");
        _filePath = Path.Combine(_folderPath, "appsettings.json");

        if (!Directory.Exists(_folderPath))
            Directory.CreateDirectory(_folderPath);
    }
    
    public AppSettings Load()
    {
        if (!File.Exists(_filePath))
            return new AppSettings();
        
        string existingJson = File.ReadAllText(_filePath);
        AppSettings currentSettings = JsonSerializer.Deserialize<AppSettings>(existingJson);
        return currentSettings;
    }

    public void Save(AppSettings settings)
    {
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public void Clear()
    {
        AppSettings settings = Load();
        settings.User = null;
        Save(settings);
    }
}