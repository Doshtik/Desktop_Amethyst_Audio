namespace Desktop_Amethyst_Audio.Models.DTO.Tracks;

public class TrackInfoDto
{
    public long Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public List<UserInfoDto> UserList { get; set; }

    public string CoverUrl { get; set; } = null!;

    public string TrackUrl { get; set; } = null!;

    public bool? IsExplicit { get; set; }
    
    public int DurationSec { get; set; }
}