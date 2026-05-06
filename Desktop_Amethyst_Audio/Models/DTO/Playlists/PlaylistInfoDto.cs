using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Models.DTO.Playlists;

public class PlaylistInfoDto
{
    public long Id { get; set; }
    
    public long OwnerId { get; set; }
    
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? CoverUrl { get; set; }
    
    public List<TrackInfoDto> TrackList { get; set; } = null!;
}