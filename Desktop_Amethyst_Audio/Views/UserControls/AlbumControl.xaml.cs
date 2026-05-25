using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.DTO.Albums;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class AlbumControl : UserControl
{
    public AlbumInfoDto Album { get; set; }
    public AlbumControl()
    {
        InitializeComponent();
    }
}