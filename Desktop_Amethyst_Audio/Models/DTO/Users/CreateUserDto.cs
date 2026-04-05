namespace Backend_Amethyst_Audio.DTO;

public class CreateUserDto
{
    public string Nickname { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!; // Передаем чистый пароль, который потом хешируем
}