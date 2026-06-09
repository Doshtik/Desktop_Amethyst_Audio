using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Messages.Data;

public record SavedTracksTransferMessage(List<TrackInfoDto> savedTracks);