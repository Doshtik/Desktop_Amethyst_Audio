namespace Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;

public record NavigateToProfileMessage(long userId, bool isOwnProfile);