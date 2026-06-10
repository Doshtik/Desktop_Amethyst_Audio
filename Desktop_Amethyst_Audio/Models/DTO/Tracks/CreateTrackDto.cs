namespace Desktop_Amethyst_Audio.Models.DTO.Tracks;

public class CreateTrackDto
{
    public string Name { get; set; } = null!;

    public string CoverFilePath { get; set; } = null!;

    public string TrackFilePath { get; set; } = null!;
    
    public List<long> AuthorsIdList { get; set; }
    
    public short PaceId { get; set; }

    public short MoodId { get; set; }
    
    public List<short> GenresIdList { get; set; }

    public bool? IsTextless { get; set; }

    public bool? IsExplicit { get; set; }

    public string? Country { get; set; }
}