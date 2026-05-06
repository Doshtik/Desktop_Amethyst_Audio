using System.ComponentModel.DataAnnotations;

namespace Desktop_Amethyst_Audio.Models.DTO.Users;

public class ChangeUserPasswordDto
{
    public string OldPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!; // Проверка на совпадение новых паролей пусть будет на клиенте
}