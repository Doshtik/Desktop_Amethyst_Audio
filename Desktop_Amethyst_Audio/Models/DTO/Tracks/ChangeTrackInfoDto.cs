namespace Desktop_Amethyst_Audio.Models.DTO.Tracks;

public class ChangeTrackInfoDto
{
    public long Id { get; set; }
    
    public string? Name { get; set; }

    public IFormFile? CoverFile { get; set; }

    public IFormFile? TrackFile { get; set; }
    
    public short PaceId { get; set; }

    public short MoodId { get; set; }
    
    public string? GenresIdList { get; set; }

    public bool? IsTextless { get; set; }

    public bool? IsExplicit { get; set; }

    public string? Country { get; set; }
}