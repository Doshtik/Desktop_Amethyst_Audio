using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Backend_Amethyst_Audio.DTO;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Pages;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Entities;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Microsoft.Win32;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class TrackFormModalWindow : Window
{
    private List<Pace> _paces = new List<Pace>();
    private List<Mood> _moods = new List<Mood>();
    private ObservableCollection<SelectableGenre> AvailableGenres { get; } = new();
    public HashSet<short> SelectedGenreIds { get; private set; } = new();
    
    private SearchApiClient _searchApiClient;
    private TrackApiClient _trackApiClient;
    private RecommendationApiClient _recommendationApiClient;
    
    private SettingsService _settingsService;

    private string _trackFilePath = string.Empty;
    private string _trackCoverPath = string.Empty;
    
    public TrackFormModalWindow()
    {
        InitializeComponent();
        _searchApiClient = new SearchApiClient();
        _trackApiClient = new TrackApiClient();
        _recommendationApiClient = new RecommendationApiClient();
        GenreListBox.ItemsSource = AvailableGenres;
    }

    private async void TrackFormModalWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            List<GenreInfoDto> genres = await _searchApiClient.GetGenresAsync();
            AvailableGenres.Clear();
            foreach (GenreInfoDto genre in genres)
            {
                SelectableGenre control = new(genre);
                AvailableGenres.Add(control);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("Произошла ошибка во время загрузки");
        }

        try
        {
            ResonanceConfigDto config = await _recommendationApiClient.GetRecommendationConfigAsync();
            _paces = config.AvailablePaces;
            _moods = config.AvailableMoods;
        }
        catch (Exception exception)
        {
            MessageBox.Show("Отдельная ошибка, потому что мне надо проверить конфиг");
        }
    }

    private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (IsFieldsCorrect(out string errorMessage))
        {
            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        short paceId = 0;
        short moodId = 0;
        SelectedGenreIds = new HashSet<short>(
            AvailableGenres.Where(g => g.IsSelected).Select(g => g.Id)
        );
        
        //TODO: Проверить подходит ли string для TrackFile и CoverFile
        CreateTrackDto trackDto = new()
        {
            Name = TrackNameTextBox.Text.Trim(),
            TrackFile = _trackFilePath,
            CoverFile = _trackCoverPath,
            AuthorsIdList = new List<long>()
            {
                _settingsService.Load().User.Id
            },
            PaceId = paceId,
            MoodId = moodId,
            GenresIdList = SelectedGenreIds.ToList(),
            IsExplicit = IsTrackExplicitToggleButton.IsChecked,
            IsTextless = IsTrackTextlessToggleButton.IsChecked
        };
        
        try
        {
            _trackApiClient.CreateAsync(trackDto);
        }
        catch (Exception exception)
        {
            MessageBox.Show("Не удалось создать трек");
        }
    }

    private bool IsFieldsCorrect(out string errorMessage)
    {
        errorMessage = string.Empty;
        //TODO: Сделать проверку полей
        return true;
    }

    private void TrackFileSelectorButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Выберите файл для загрузки",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Filter = "Изображения (*.jpg, *.png)|*.jpg;*.png",
            FilterIndex = 1,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() is true)
        {
            string filePath = openFileDialog.FileName;
            _trackFilePath = filePath;
        }
    }

    private void TrackCoverSelectorButton_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Выберите файл для загрузки",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            Filter = "MP3 файлы (*.mp3)|*.mp3",
            FilterIndex = 1,
            Multiselect = false
        };

        if (openFileDialog.ShowDialog() is true)
        {
            string filePath = openFileDialog.FileName;
            _trackCoverPath = filePath;
        }
    }
}