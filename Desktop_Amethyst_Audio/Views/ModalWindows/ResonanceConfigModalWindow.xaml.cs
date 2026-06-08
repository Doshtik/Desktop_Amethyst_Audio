using System.Diagnostics;
using System.Windows;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Pages;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
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
    }

    private async void ResonanceConfigModalWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ResonanceConfigDto configDto = await _recommendationApiClient.GetRecommendationConfigAsync();
            _paces = configDto.AvailablePaces.ToDictionary(x => x.Id, x => x.PaceName);
            _moods = configDto.AvailableMoods.ToDictionary(x => x.Id, x => x.MoodName);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }

        PacesListBox.ItemsSource = _paces;
        PacesListBox.DisplayMemberPath = "Value";
        PacesListBox.SelectedValuePath = "Key";
    }

    private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        PageResonanceDto dto = new PageResonanceDto()
        {
            PaceId = (short)PacesListBox.SelectedValue,
            MoodId = (short)MoodsListBox.SelectedValue,
            IsTextless = (bool)IsTextlessToggleButton.IsChecked,
            Country = CountryCodeComboBox.SelectedValue.ToString()
        };
        
        try
        {
            List<TrackInfoDto> result = await _recommendationApiClient.GetPersonalizedRecommendationsAsync(dto);
            PlaybackService.SetQueue(result);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }
}