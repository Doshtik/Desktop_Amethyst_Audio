using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Pages;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Entities;
using Desktop_Amethyst_Audio.Models.Enums;
using Desktop_Amethyst_Audio.Models.Services.Implementation;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class ResonanceConfigModalWindow : Window
{
    private readonly IRecommendationApiClient _recommendationApiClient;
    private Dictionary<short, string> _paces = new ();
    private Dictionary<short, string> _moods = new ();
    public ResonanceConfigModalWindow()
    {
        InitializeComponent();
        _recommendationApiClient = new RecommendationApiClient();
        IsTextlessToggleButton.IsChecked = false;
    }

    private async void ResonanceConfigModalWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        CountryCodeComboBox.ItemsSource = Enum.GetValues(typeof(CountryCode));
        try
        {
            ResonanceConfigDto configDto = await _recommendationApiClient.GetRecommendationConfigAsync();
            _paces = configDto.AvailablePaces.ToDictionary(x => x.Id, x => x.PaceName);
            _moods = configDto.AvailableMoods.ToDictionary(x => x.Id, x => x.MoodName);
            
            PacesListBox.ItemsSource = configDto.AvailablePaces;
            MoodsListBox.ItemsSource = configDto.AvailableMoods;
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

    private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageResonanceDto dto = new PageResonanceDto()
        {
            PaceId = (short)PacesListBox.SelectedValue,
            MoodId = (short)MoodsListBox.SelectedValue,
            IsTextless = (bool)IsTextlessToggleButton.IsChecked,
            Country = CountryCodeComboBox.SelectedItem is not null ? CountryCodeComboBox.SelectedValue.ToString() : null
        };
        
        try
        {
            List<TrackInfoDto> result = await _recommendationApiClient.GetPersonalizedRecommendationsAsync(dto);
            PlaybackService.SetQueue(result);
            Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
            Debug.WriteLine(exception);
        }
    }
}