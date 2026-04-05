namespace Desktop_Amethyst_Audio.Models.Services.Abstraction;

public interface ISettingsService
{
    AppSettings Load();
    void Save(AppSettings settings);
    void Clear();
}