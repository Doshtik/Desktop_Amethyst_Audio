using Desktop_Amethyst_Audio.Models.DTO.Reports;
using Desktop_Amethyst_Audio.Models.DTO.Reports.ReportAnswer;

namespace Desktop_Amethyst_Audio.Models.Clients.Abstraction;

public interface IReportApiClient
{
    Task<ReportInfoDto> GetByIdAsync(long reportId);
    Task<List<ReportInfoDto>> GetAllAsync();
    Task<ReportInfoDto> CreateAsync(CreateReportDto dto);
    Task DeleteAsync(long reportId);
    Task<ReportAnswerInfoDto> GetAnswerByIdAsync(long reportAnswerId);
    Task<List<ReportAnswerInfoDto>> GetAnswerAllAsync();
    Task<ReportAnswerInfoDto> CreateAnswerAsync(CreateReportAnswerDto dto);
    Task DeleteAnswerAsync(long reportAnswerId);
}