namespace Desktop_Amethyst_Audio.Models.DTO.Pages;

public class PageResonanceDto
{
    public short PaceId { get; set; }
    
    public short MoodId { get; set; }

    public bool IsFromLibrary { get; set; } = false;
    
    public bool IsTextless { get; set; }
    
    public string? Country { get; set; }
}