using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IPlaylistApiClient
{
    Task<PlaylistInfoDto> GetPlaylistByIdAsync(long id);
    Task<List<PlaylistInfoDto>> GetPlaylistAllAsync();
    Task<List<PlaylistInfoDto>> GetListByUserIdAsync(long userId);
    Task<BitmapImage> GetPlaylistCoverAsync(string coverUrl);
    
    Task<PlaylistInfoDto> CreatePlaylistAsync(CreatePlaylistDto dto);
    Task<PlaylistInfoDto> UpdatePlaylistAsync(ChangePlaylistInfoDto dto);
    Task DeletePlaylistAsync(long id);
    
    Task<bool> IsPlaylistSavedAsync(long id);
    Task SavePlaylistAsync(long id);
    Task UnsavePlaylistAsync(long id);
}