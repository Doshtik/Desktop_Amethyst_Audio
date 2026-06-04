using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Views.CustomElements.WaveVisualizer;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class ResonanceSettingsModalWindow : Window
{
    private readonly WaveRenderer? _viz;
    private bool _ready;

    public ResonanceSettingsModalWindow(WaveRenderer viz)
    {
        InitializeComponent();
        _viz = viz;

        StyleBox.SelectedIndex = _viz.Style switch
        {
            VisStyle.Sphere => 1,
            VisStyle.Bars => 2,
            _ => 0
        };
        ColorBox.SelectedIndex = (int)_viz.Color;
        SensSlider.Value = _viz.Sensitivity;
        BassSlider.Value = _viz.BassBoost;
        SpeedSlider.Value = _viz.AnimSpeed;
        GlowSlider.Value = _viz.Glow;
        SizeSlider.Value = _viz.SphereScale;
        RaysCheck.IsChecked = _viz.ShowRays;
        ParticlesCheck.IsChecked = _viz.ShowParticles;
        OrbitsCheck.IsChecked = _viz.ShowOrbits;

        SensValue.Text = _viz.Sensitivity.ToString("0.0");
        BassValue.Text = _viz.BassBoost.ToString("0.0");
        SpeedValue.Text = _viz.AnimSpeed.ToString("0.0");
        GlowValue.Text = _viz.Glow.ToString("0.0");
        SizeValue.Text = _viz.SphereScale.ToString("0.0");
        _ready = true;
    }

    private void StyleBox_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_ready || _viz == null) return;
        _viz.Style = StyleBox.SelectedIndex switch
        {
            1 => VisStyle.Sphere,
            2 => VisStyle.Bars,
            _ => VisStyle.Ring
        };
    }

    private void ColorBox_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_ready || _viz == null) return;
        _viz.Color = (VisColor)ColorBox.SelectedIndex;
    }

    private void SensSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_viz != null) _viz.Sensitivity = e.NewValue;
        if (SensValue != null) SensValue.Text = e.NewValue.ToString("0.0");
    }

    private void BassSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_viz != null) _viz.BassBoost = e.NewValue;
        if (BassValue != null) BassValue.Text = e.NewValue.ToString("0.0");
    }

    private void SpeedSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_viz != null) _viz.AnimSpeed = e.NewValue;
        if (SpeedValue != null) SpeedValue.Text = e.NewValue.ToString("0.0");
    }

    private void GlowSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_viz != null) _viz.Glow = e.NewValue;
        if (GlowValue != null) GlowValue.Text = e.NewValue.ToString("0.0");
    }

    private void SizeSlider_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_viz != null) _viz.SphereScale = e.NewValue;
        if (SizeValue != null) SizeValue.Text = e.NewValue.ToString("0.0");
    }

    private void Element_Changed(object sender, RoutedEventArgs e)
    {
        if (!_ready || _viz == null) return;
        _viz.ShowRays = RaysCheck.IsChecked == true;
        _viz.ShowParticles = ParticlesCheck.IsChecked == true;
        _viz.ShowOrbits = OrbitsCheck.IsChecked == true;
    }

    private void Reset_Click(object sender, RoutedEventArgs e)
    {
        if (_viz == null) return;
        StyleBox.SelectedIndex = 0;
        ColorBox.SelectedIndex = 0;
        SensSlider.Value = 1.3;
        BassSlider.Value = 1.2;
        SpeedSlider.Value = 1.0;
        GlowSlider.Value = 1.0;
        SizeSlider.Value = 1.0;
        RaysCheck.IsChecked = true;
        ParticlesCheck.IsChecked = true;
        OrbitsCheck.IsChecked = true;
    }

    private void Done_Click(object sender, RoutedEventArgs e) => Close();
}
