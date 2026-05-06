using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.DTO.Albums;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IAlbumApiClient
{
    Task<AlbumInfoDto> GetByIdAsync(long id);
    Task<List<AlbumInfoDto>> GetAllAsync();
    Task<List<AlbumInfoDto>> GetListByUserIdAsync(long userId);
    Task<BitmapImage> GetAlbumCoverAsync(string coverUrl);
    
    Task<AlbumInfoDto> CreateAlbumAsync(CreateAlbumDto dto);
    Task<AlbumInfoDto> UpdateAlbumAsync(ChangeAlbumInfoDto dto);
    Task DeleteAlbumAsync(long id);
    
    Task SaveAlbumAsync(long id);
    Task UnsaveAlbumAsync(long id);
}