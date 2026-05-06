namespace Desktop_Amethyst_Audio.Models.DTO.Reports.ReportAnswer;

public class ReportAnswerInfoDto
{
    public long Id { get; set; }
    
    public int IdEmployee { get; set; }

    public long IdReport { get; set; }

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}