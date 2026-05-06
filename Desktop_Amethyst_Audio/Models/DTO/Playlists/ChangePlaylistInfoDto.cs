namespace Desktop_Amethyst_Audio.Models.DTO.Playlists;

public class ChangePlaylistInfoDto
{
    public bool? IsPublic { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? CoverFilePath { get; set; }
    
    public List<long> AddedTracksIdList { get; set; }
    
    public List<long> RemovedTracksIdList { get; set; }
}