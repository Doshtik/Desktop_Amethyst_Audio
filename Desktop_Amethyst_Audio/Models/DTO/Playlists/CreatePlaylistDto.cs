using System.ComponentModel.DataAnnotations;

namespace Desktop_Amethyst_Audio.Models.DTO.Playlists;

public class CreatePlaylistDto
{
    [Required] public long UserId { get; set; }

    [Required] public bool IsPublic { get; set; }

    [Required] public string Name { get; set; }

    public string? Description { get; set; }

    public string? CoverFilePath { get; set; }
    
    [Required] public List<long> TracksIdList { get; set; }
}