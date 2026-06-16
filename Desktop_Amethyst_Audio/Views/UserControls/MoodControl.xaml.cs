using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.Entities;

namespace Desktop_Amethyst_Audio.Views.UserControls;

public partial class MoodControl : UserControl
{
    public Mood Mood { get; set; }
    public MoodControl()
    {
        InitializeComponent();
    }

    private void MoodControl_OnLoaded(object sender, RoutedEventArgs e)
    {
        MoodNameTextBlock.Text = Mood.MoodName;
    }
}