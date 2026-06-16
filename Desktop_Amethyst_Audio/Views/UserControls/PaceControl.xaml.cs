using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.Entities;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class PaceControl : UserControl
{
    public Pace Pace { get; set; }
    public PaceControl()
    {
        InitializeComponent();
    }

    private void PaceControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        PaceNameTextBlock.Text = Pace.PaceName;
    }
}