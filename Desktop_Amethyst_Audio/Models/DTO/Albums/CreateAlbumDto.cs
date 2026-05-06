using System.ComponentModel.DataAnnotations;

namespace Desktop_Amethyst_Audio.Models.DTO.Albums;

public class CreateAlbumDto
{
    public string Name { get; set; }

    public string AlbumCoverFilePath { get; set; }
    
    public List<long> AuthorsIdList { get; set; }
    
    public List<long> TracksIdList { get; set; }
}