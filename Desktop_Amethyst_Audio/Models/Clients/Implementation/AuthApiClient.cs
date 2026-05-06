using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.DTO.Albums;

namespace Desktop_Amethyst_Audio.Models.Clients.Implementation;

public class AuthApiClient : IAlbumApiClient
{
    public Task<AlbumInfoDto> GetByIdAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<AlbumInfoDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<AlbumInfoDto>> GetListByUserIdAsync(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<BitmapImage> GetAlbumCoverAsync(string coverUrl)
    {
        throw new NotImplementedException();
    }

    public Task<AlbumInfoDto> CreateAlbumAsync(CreateAlbumDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<AlbumInfoDto> UpdateAlbumAsync(ChangeAlbumInfoDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAlbumAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task SaveAlbumAsync(long id)
    {
        throw new NotImplementedException();
    }

    public Task UnsaveAlbumAsync(long id)
    {
        throw new NotImplementedException();
    }
}