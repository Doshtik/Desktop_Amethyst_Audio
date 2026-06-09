using Desktop_Amethyst_Audio.Models.DTO.Albums;

namespace Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;

public record NavigateToAlbumMessage(AlbumInfoDto album, bool isOwnAlbum);