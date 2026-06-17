using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.DTO;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class GenreControl : UserControl
{
    public GenreInfoDto Genre { get; set; }
    public GenreControl()
    {
        InitializeComponent();
    }

    private void GenreControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        GenreNameTextBlock.Text = Genre.GenreName;
    }
}