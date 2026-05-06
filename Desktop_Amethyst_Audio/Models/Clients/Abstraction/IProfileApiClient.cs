using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IProfileApiClient
{
    Task<UserInfoDto> GetUserByIdAsync(long id);
    Task<List<UserInfoDto>> GetUserAllAsync();
    
    Task<UserInfoDto> UpdateUserAsync(ChangeUserInfoDto dto);
    Task DeleteUserAsync(long id);

    Task<List<UserHistoryDto>> GetUserHistoryAsync();
    Task AddToHistoryAsync(long trackId);
    Task UpdateListeningTimeAsync(long trackId, int seconds);
    
    Task<List<TrackInfoDto>> GetUserLibraryAsync(long id);
    Task AddUserLibraryAsync(long id);
    Task RemoveUserLibraryAsync(long id);
    
    Task<List<PlaylistInfoDto>> GetUserSavedPlaylistsAsync(long id);
    Task<List<AlbumInfoDto>> GetUserSavedAlbumsAsync(long id);
    
    Task<BitmapImage> GetUserAvatarAsync(string avatarUrl);
    Task<BitmapImage> GetUserHeaderAsync(string headerUrl);
    
    Task FollowUserAsync(long targetId);
    Task UnfollowUserAsync(long subscriberId);
}