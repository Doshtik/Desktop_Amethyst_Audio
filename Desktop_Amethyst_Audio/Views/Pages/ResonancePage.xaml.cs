using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.CustomElements.WaveVisualizer;
using Desktop_Amethyst_Audio.Views.ModalWindows;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class ResonancePage : Page
{
    public ResonancePage(AudioService audioService)
    {
        InitializeComponent();
        Sphere.Audio = audioService;
        Sphere.Mode = VisMode.Equalizer;
    }

    private void ResonanceSettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
        var window = new ResonanceSettingsModalWindow() { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }
}