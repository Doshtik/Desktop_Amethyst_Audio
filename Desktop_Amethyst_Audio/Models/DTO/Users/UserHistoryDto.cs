using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Models.DTO.Users;

public class UserHistoryDto
{
    public UserInfoDto User { get; set; }
    
    public TrackInfoDto Track { get; set; }
    
    public int TotalListeningSec { get; set; }
}