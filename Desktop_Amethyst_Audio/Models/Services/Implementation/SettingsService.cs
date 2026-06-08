using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using System.IO;
using System.Text.Json;
using static System.Windows.Forms.Design.AxImporter;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class SettingsService : ISettingsService
{
    private readonly string _folderPath;
    private readonly string _filePath;
    
    public SettingsService()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _folderPath = Path.Combine(appDataPath, "AmethystAudio");
        _filePath = Path.Combine(_folderPath, "appsettings.json");

        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);

            var defaultSettings = new AppSettings();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(defaultSettings, options);

            File.WriteAllText(_filePath, jsonString);
        }
    }
    
    public AppSettings Load()
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        };

        if (!File.Exists(_filePath))
            return new AppSettings();
    
        string existingJson = File.ReadAllText(_filePath);
    
        if (string.IsNullOrWhiteSpace(existingJson))
            return new AppSettings();
    
        try
        {
            var currentSettings = JsonSerializer.Deserialize<AppSettings>(existingJson, jsonOptions);
            return currentSettings ?? new AppSettings();
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SettingsService] Parse exception: {ex.Message}");
        
            var backupPath = _filePath + $".broken.{DateTime.Now:yyyyMMddHHmmss}.bak";
            File.Copy(_filePath, backupPath, overwrite: true);
        
            return new AppSettings();
        }
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