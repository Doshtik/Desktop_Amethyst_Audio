using System.Globalization;
using System.Windows.Data;

namespace Desktop_Amethyst_Audio.Views.CustomElements.WaveVisualizer;

public class SliderFillConverter : IMultiValueConverter
{
    public static readonly SliderFillConverter Instance = new();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3) return 0.0;
        double value = (double)values[0];
        double max = (double)values[1];
        double width = (double)values[2];
        if (max == 0) return 0.0;
        return width * value / max;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
