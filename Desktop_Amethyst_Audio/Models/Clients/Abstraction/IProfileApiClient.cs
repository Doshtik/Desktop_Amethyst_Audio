using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IProfileApiClient
{
    Task<UserInfoDto> GetUserByIdAsync(long userId);
    Task<List<UserInfoDto>> GetUserAllAsync();
    
    Task<UserInfoDto> UpdateUserAsync(ChangeUserInfoDto dto);
    Task DeleteUserAsync();
    
    Task<List<TrackInfoDto>> GetUserLibraryAsync(long userId);
    Task<List<PlaylistInfoDto>> GetUserSavedPlaylistsAsync(long userId);
    Task<List<AlbumInfoDto>> GetUserSavedAlbumsAsync(long userId);
    
    Task<BitmapImage> GetUserAvatarAsync(string avatarUrl);
    Task<BitmapImage> GetUserHeaderAsync(string headerUrl);
    
    Task FollowUserAsync(FollowUserDto dto);
    Task UnfollowUserAsync(FollowUserDto dto);
}