using System.ComponentModel.DataAnnotations;

namespace Desktop_Amethyst_Audio.Models.DTO.Playlists;

public class CreatePlaylistDto
{
    [Required]
    public long UserId { get; set; }

    [Required]
    public bool IsPublic { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public IFormFile? CoverFile { get; set; }
    
    [Required]
    public string TracksIdList { get; set; }
}