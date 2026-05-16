using Desktop_Amethyst_Audio.Models.DTO.Tracks;

namespace Desktop_Amethyst_Audio.Messages.Action;

public record TrackChangedMessage(TrackInfoDto Track);