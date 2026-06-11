using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Users;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class UsersControl : UserControl
{
    public UserInfoDto User { get; set; }
    private IProfileApiClient _profileApiClient;
    
    public UsersControl()
    {
        InitializeComponent();
        _profileApiClient = new ProfileApiClient();
    }

    private async void UsersControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        NickNameTextBlock.Text = User.Nickname;
        try
        {
            BitmapImage avatarImage = await _profileApiClient.GetUserAvatarAsync(User.AvatarUrl);
            AvatarImage.Source = avatarImage;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }
}