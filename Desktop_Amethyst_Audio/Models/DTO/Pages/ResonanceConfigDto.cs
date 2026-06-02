using Desktop_Amethyst_Audio.Models.Entities;

namespace Desktop_Amethyst_Audio.Models.DTO.Pages;

public class ResonanceConfigDto
{
    public List<Pace> AvailablePaces { get; set; }
    
    public List<Mood> AvailableMoods { get; set; }
}