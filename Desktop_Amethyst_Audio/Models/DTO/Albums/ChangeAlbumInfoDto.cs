namespace Desktop_Amethyst_Audio.Models.DTO.Albums;

public class ChangeAlbumInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? AlbumCoverFilePath { get; set; }
    public List<long>? AddedTrackIdList { get; set; }
    public List<long>? RemovedTrackIdList { get; set; }
}