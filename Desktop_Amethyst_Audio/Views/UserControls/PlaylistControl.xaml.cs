using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class PlaylistControl : UserControl
{
    public PlaylistInfoDto Playlist { get; set; }
    public PlaylistControl()
    {
        InitializeComponent();
    }
}