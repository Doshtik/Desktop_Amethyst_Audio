namespace Desktop_Amethyst_Audio.Models.DTO.Reports.ReportAnswer;

public class CreateReportAnswerDto
{
    public int IdEmployee { get; set; }

    public long IdReport { get; set; }

    public string Message { get; set; } = null!;
}