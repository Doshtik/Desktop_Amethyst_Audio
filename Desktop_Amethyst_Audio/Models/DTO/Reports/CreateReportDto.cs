namespace Desktop_Amethyst_Audio.Models.DTO.Reports;

public class CreateReportDto
{
    public long IdUser { get; set; }

    public short IdType { get; set; }

    public short IdReason { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}