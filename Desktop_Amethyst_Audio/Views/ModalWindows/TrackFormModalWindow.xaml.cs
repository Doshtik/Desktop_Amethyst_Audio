using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Pages;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Entities;
using Desktop_Amethyst_Audio.Models.Enums;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;
using Microsoft.Win32;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class TrackFormModalWindow : Window
{
    public TrackInfoDto? Track { get; set; }
    public FormMode Mode { get; set; } = FormMode.Add;
    
    private List<Pace> _paces = new List<Pace>();
    private List<Mood> _moods = new List<Mood>();
    
    private SearchApiClient _searchApiClient;
    private TrackApiClient _trackApiClient;
    private RecommendationApiClient _recommendationApiClient;
    
    private SettingsService _settingsService;

    private string? _trackFilePath = string.Empty;
    private string? _trackCoverPath = string.Empty;
    
    public TrackFormModalWindow()
    {
        InitializeComponent();
        _searchApiClient = new SearchApiClient();
        _trackApiClient = new TrackApiClient();
        _recommendationApiClient = new RecommendationApiClient();
        _settingsService = new SettingsService();
    }

    private async void TrackFormModalWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            List<GenreInfoDto> genres = await _searchApiClient.GetGenresAsync();
            GenreListBox.Items.Clear();
            foreach (GenreInfoDto genre in genres)
            {
                GenreControl control = new GenreControl();
                control.Genre = genre;
                GenreListBox.Items.Add(control);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("Произошла ошибка во время загрузки");
            Close();
        }

        try
        {
            ResonanceConfigDto config = await _recommendationApiClient.GetRecommendationConfigAsync();
            _paces = config.AvailablePaces;
            PacesComboBox.ItemsSource = _paces;
            PacesComboBox.DisplayMemberPath = "PaceName";
            PacesComboBox.SelectedValuePath = "Id";
            _moods = config.AvailableMoods;
            MoodsComboBox.ItemsSource = _moods;
            MoodsComboBox.DisplayMemberPath = "MoodName";
            MoodsComboBox.SelectedValuePath = "Id";
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.InnerException);
        }

        if (Track is not null)
        {
            Mode = FormMode.Edit;
            BitmapImage coverImage = await _trackApiClient.GetTrackCoverAsync(Track.CoverUrl);
            CoverImage.Source = coverImage;
            NameTextBox.Text = Track.Name;
            PacesComboBox.SelectedValue = _paces
                .Where(x => x.PaceName == Track.PaceName)
                .Select(x => x.Id)
                .First();
            MoodsComboBox.SelectedValue = _moods
                .Where(x => x.MoodName == Track.MoodName)
                .Select(x => x.Id)
                .First();
            GenreListBox.Items.Clear();
            foreach (GenreInfoDto genre in Track.GenreList)
            {
                GenreControl control = new();
                control.Genre = genre;
                GenreListBox.Items.Add(control);
            }
            IsExplicitToggleButton.IsChecked = Track.IsExplicit;
            IsTextlessToggleButton.IsChecked = Track.IsTextless;
        }
    }

    private async void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (IsFieldsCorrect(out string errorMessage))
        {
            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        List<short> selectedGenreIds = GenreListBox.SelectedItems
            .Cast<GenreInfoDto>()
            .Select(x => x.Id)
            .ToList();

        if (Mode is FormMode.Add)
        {
            CreateTrackDto trackDto = new CreateTrackDto()
            {
                Name = NameTextBox.Text.Trim(),
                CoverFilePath = _trackCoverPath,
                TrackFilePath = _trackFilePath,
                AuthorsIdList = new List<long> { _settingsService.Load().User.Id },
                PaceId = (short)PacesComboBox.SelectedValue,
                MoodId = (short)MoodsComboBox.SelectedValue,
                GenresIdList = selectedGenreIds,
                IsExplicit = IsExplicitToggleButton.IsChecked,
                IsTextless = IsTextlessToggleButton.IsChecked
            };
    
            try
            {
                var result = await _trackApiClient.CreateAsync(trackDto);
                MessageBox.Show("Трек успешно создан");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Не удалось создать трек: {exception.Message}");
                Debug.WriteLine(exception);
            }
        }
        else
        {
            ChangeTrackInfoDto updateDto = new ChangeTrackInfoDto
            {
                Id = Track.Id,
                Name = NameTextBox.Text.Trim(),
                PaceId = (short)PacesComboBox.SelectedValue,
                MoodId = (short)MoodsComboBox.SelectedValue,
                GenresIdList = selectedGenreIds,
                IsExplicit = IsExplicitToggleButton.IsChecked,
                IsTextless = IsTextlessToggleButton.IsChecked,
    
                TrackFilePath = _trackFilePath, 
                CoverFilePath = _trackCoverPath 
            };
            
            try
            {
                var result = await _trackApiClient.UpdateAsync(Track.Id, updateDto);
                MessageBox.Show("Трек успешно обновлен");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Не удалось обновлен трек: {exception.Message}");
                Debug.WriteLine(exception);
            }
        }
    }

    private bool IsFieldsCorrect(out string errorMessage)
    {
        errorMessage = string.Empty;
    
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            errorMessage += "Введите название трека\n";
        }
    
        if (Mode == FormMode.Add)
        {
            if (string.IsNullOrEmpty(_trackFilePath) || !File.Exists(_trackFilePath))
            {
                errorMessage += "Выберите аудиофайл\n";
            }

            if (string.IsNullOrEmpty(_trackCoverPath) || !File.Exists(_trackCoverPath))
            {
                errorMessage += "Выберите обложку\n";
            }
        }
    
        if (PacesComboBox.SelectedValue == null)
        {
            errorMessage += "Выберите темп\n";
        }
    
        if (MoodsComboBox.SelectedValue == null)
        {
            errorMessage += "Выберите настроение\n";
        }
    
        if (GenreListBox.SelectedItems.Count == 0)
        {
            errorMessage += "Выберите хотя бы один жанр\n";
        }

        if (errorMessage == string.Empty)
            return false;
    
        return true;
    }

    private void TrackCoverSelectorButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Выберите обложку",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Filter = "Изображения (*.jpg, *.png)|*.jpg;*.png",
            FilterIndex = 1,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() is true)
        {
            _trackCoverPath = openFileDialog.FileName;
        
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; 
            bitmap.UriSource = new Uri(_trackCoverPath, UriKind.Absolute);
            bitmap.EndInit();
        
            CoverImage.Source = bitmap;
        }
    }

    private void TrackFileSelectorButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Выберите аудиофайл",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            Filter = "MP3 файлы (*.mp3)|*.mp3|WAV файлы (*.wav)|*.wav|Все аудиофайлы|*.*",
            FilterIndex = 1,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() is true)
        {
            _trackFilePath = openFileDialog.FileName;
            ChooseTrackFileButton.Content = Path.GetFileName(_trackFilePath);
        }
    }
}