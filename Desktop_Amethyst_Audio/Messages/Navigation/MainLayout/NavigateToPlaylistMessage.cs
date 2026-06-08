using Desktop_Amethyst_Audio.Models.DTO.Playlists;

namespace Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;

public record NavigateToPlaylistMessage(PlaylistInfoDto playlist);