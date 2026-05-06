using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Models.DTO;

public class SearchInfoDto
{
    public List<UserInfoDto> Users { get; set; }
    public List<TrackInfoDto> Tracks { get; set; }
    public List<AlbumInfoDto> Albums { get; set; }
    public List<PlaylistInfoDto> Playlists { get; set; }
}