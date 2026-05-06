using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface ITrackApiClient
{
    Task<TrackInfoDto> GetByIdAsync(long id);
    Task<List<TrackInfoDto>> GetAllAsync();
    Task<List<TrackInfoDto>> GetListByUserIdAsync(long userId);
    
    Task<TrackInfoDto> CreateAsync(CreateTrackDto dto);
    Task<TrackInfoDto> UpdateAsync(long trackId, ChangeTrackInfoDto dto);
    Task DeleteAsync(long trackId);
    
    Task<byte[]> GetTrackFileAsync(string trackFileUrl);
    Task<BitmapImage> GetTrackCoverAsync(string trackCoverUrl);
}