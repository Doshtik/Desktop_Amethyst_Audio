using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Views.ModalWindows;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class ResonancePage : Page
{
    public ResonancePage()
    {
        InitializeComponent();
    }

    private void ResonanceSettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
        ResonanceSettingsModalWindow window = new ResonanceSettingsModalWindow();
        window.ShowDialog();
    }
}