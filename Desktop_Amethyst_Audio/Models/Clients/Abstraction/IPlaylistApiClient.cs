using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IPlaylistApiClient
{
    Task<PlaylistInfoDto> GetPlaylistByIdAsync();
    Task<List<PlaylistInfoDto>> GetPlaylistAllAsync();
    Task<PlaylistInfoDto> GetListPlaylistByUserIdAsync();
    Task<BitmapImage> GetPlaylistCoverAsync();
    
    Task<PlaylistInfoDto> CreatePlaylistAsync();
    Task<PlaylistInfoDto> UpdatePlaylistAsync();
    Task DeletePlaylistAsync();
    
    Task SavePlaylistAsync();
    Task UnsavePlaylistAsync();
}