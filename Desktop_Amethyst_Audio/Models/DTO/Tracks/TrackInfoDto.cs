using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Models.DTO.Tracks;

public class TrackInfoDto
{
    public long Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string PaceName { get; set; }
    
    public string MoodName { get; set; }

    
    public List<UserInfoDto> UserList { get; set; }
    
    public List<GenreInfoDto> GenreList { get; set; }

    public string CoverUrl { get; set; } = null!;

    public string TrackUrl { get; set; } = null!;

    public bool? IsExplicit { get; set; }
    
    public bool? IsTextless { get; set; }
    
    public int DurationSec { get; set; }
}