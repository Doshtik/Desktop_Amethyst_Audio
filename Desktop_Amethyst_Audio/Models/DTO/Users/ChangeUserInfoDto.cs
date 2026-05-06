namespace Desktop_Amethyst_Audio.Models.DTO.Users;

public class ChangeUserInfoDto
{
    public string? Lastname { get; set; }

    public string? Firstname { get; set; }

    public string Nickname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Gender { get; set; }

    public string? Country { get; set; }

    public IFormFile? AvatarFile { get; set; }

    public IFormFile? HeaderFile { get; set; }
}