namespace Backend_Amethyst_Audio.DTO;

public class UserInfoDto
{
    public long Id { get; set; }
    
    public string? Lastname { get; set; }
    
    public string? Firstname { get; set; }
    
    public string Nickname { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string? AvatarUrl { get; set; } // Ссылка на аватарку пользователя
    
    public string? HeaderUrl { get; set; } // Ссылка на задний фон пользователя
    
    public bool IsVerified { get; set; }
    public string? Token { get; set; }
}