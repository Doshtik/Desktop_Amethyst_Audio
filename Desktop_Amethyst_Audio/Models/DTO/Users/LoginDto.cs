namespace Backend_Amethyst_Audio.DTO;

public class LoginDto
{
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
}