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
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class ResonanceConfigModalWindow : Window
{
    private readonly IRecommendationApiClient _recommendationApiClient;
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

            PacesListBox.Items.Clear();
            foreach (Pace pace in configDto.AvailablePaces)
            {
                PaceControl control = new PaceControl();
                control.Pace = pace;
                PacesListBox.Items.Add(control);
            }

            MoodsListBox.Items.Clear();
            foreach (Mood mood in configDto.AvailableMoods)
            {
                MoodControl control = new MoodControl();
                control.Mood = mood;
                MoodsListBox.Items.Add(control);
            }
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
            PaceId = (PacesListBox.SelectedItem as PaceControl).Pace.Id,
            MoodId = (MoodsListBox.SelectedItem as MoodControl).Mood.Id,
            IsTextless = (bool)IsTextlessToggleButton.IsChecked,
            Country = CountryCodeComboBox.SelectedItem is not null ? CountryCodeComboBox.SelectedValue.ToString() : null
        };
        
        try
        {
            List<TrackInfoDto> result = await _recommendationApiClient.GetPersonalizedRecommendationsAsync(dto);
            if (result.Count == 0)
            {
                MessageBox.Show("Сожалеем, но мы не смогли найти подходящий трек под ваши предпочтения.\n" +
                    "Похоже, они весьма специфичны ;)", "Внимание", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                Close();
                return;
            }
            PlaybackService.SetQueue(result);
            Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message + ", "+ exception.Source + ", " + exception.InnerException);
            Debug.WriteLine(exception);
        }
    }
}