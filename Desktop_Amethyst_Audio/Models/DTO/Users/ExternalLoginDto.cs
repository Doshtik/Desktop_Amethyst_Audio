namespace Desktop_Amethyst_Audio.Models.DTO.Users;

public class ExternalLoginDto
{
    public string Provider { get; set; } = null!;
    public string Token { get; set; } = null!;
}