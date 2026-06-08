using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.Messages.Navigation;
using Desktop_Amethyst_Audio.Messages.Navigation.MainLayout;
using Desktop_Amethyst_Audio.Messages.Navigation.System;
using Desktop_Amethyst_Audio.Models;
using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.Pages;
using Microsoft.VisualBasic.ApplicationServices;
using NAudio.Wave;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Desktop_Amethyst_Audio.Models.DTO.Albums;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.Enums;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class LayoutWindow : Window
{
    private readonly AudioService _audioService;
    private readonly ISettingsService _settingsService;
    private readonly IProfileApiClient _profileApiClient;
    private readonly ITrackApiClient _trackApiClient;
    
    public ObservableCollection<AlbumControl> Albums { get; } = new ();
    public ObservableCollection<PlaylistControl> Playlists { get; } = new ();
    
    private DispatcherTimer _progressTimer;
    
    private bool _isChangingTrack = false;
    private bool _isDraggingSlider = false;

    private bool _isRepeat;
    private Brush _defaultButtonBackground;
    private Brush _activeButtonBackground;
    
    private SearchPage SearchPage { get; }
    private ResonancePage ResonancePage { get; }
    private LibraryPage LibraryPage { get; }
    
    public LayoutWindow()
    {
        InitializeComponent();
        Loaded += Window_Loaded;

        _profileApiClient = new ProfileApiClient();
        _trackApiClient = new TrackApiClient();
        _settingsService = new SettingsService();
        _audioService = new AudioService();
        
        SearchPage = new SearchPage();
        ResonancePage = new ResonancePage(_audioService);
        LibraryPage = new LibraryPage();

        TrackPanel.Visibility = Visibility.Collapsed;

        _isRepeat = false;
        BrushConverter cc = new BrushConverter();
        _defaultButtonBackground = (Brush)cc.ConvertFrom("#00000000");
        _activeButtonBackground = (Brush)cc.ConvertFrom("#5A5959");

        TimeSlider.Minimum = 0;
        TimeSlider.Maximum = 0;
        TimeSlider.Value = 0;
        TimeSlider.PreviewMouseLeftButtonDown += TimeSlider_MouseDown;
        TimeSlider.PreviewMouseLeftButtonUp += TimeSlider_MouseUp;
        
        _progressTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(125) };
        _progressTimer.Tick += ProgressTimer_Tick;
        
        _audioService.PlaybackEnded += OnPlaybackEnded;

        WeakReferenceMessenger.Default.Register<NavigateToSearchMessage>(this, (r, m) 
            => ContentFrame.Navigate(SearchPage));
        WeakReferenceMessenger.Default.Register<NavigateToSearchResultMessage>(this, (r, m) 
            => ContentFrame.Navigate(new SearchResultPage("")));
        WeakReferenceMessenger.Default.Register<NavigateToResonanceMessage>(this, (r, m) 
            => ContentFrame.Navigate(ResonancePage));
        WeakReferenceMessenger.Default.Register<NavigateToLibraryMessage>(this, (r, m) 
            => ContentFrame.Navigate(LibraryPage));
        WeakReferenceMessenger.Default.Register<NavigateToProfileMessage>(this, (r, m) 
            => ContentFrame.Navigate(new ProfilePage(m.userId, m.isOwnProfile)));
        WeakReferenceMessenger.Default.Register<NavigateToAlbumMessage>(this, (r, m) 
            => ContentFrame.Navigate(new AlbumPage(m.album)));
        WeakReferenceMessenger.Default.Register<NavigateToPlaylistMessage>(this, (r, m) 
            => ContentFrame.Navigate(new PlaylistPage(m.playlist)));
        WeakReferenceMessenger.Default.Register<NavigateToQueueMessage>(this, (r, m) 
            => ContentFrame.Navigate(new QueuePage()));
        WeakReferenceMessenger.Default.Register<NavigateToAuthMessage>(this, (r, m) =>
        {
            AuthWindow window = new AuthWindow();
            window.Show();
            Close();
        });
        
        ContentFrame.Navigate(LibraryPage);
        
        WeakReferenceMessenger.Default.Register<TrackChangedMessage>(this, (r, m) 
            => ChangeTrack(m.Track));
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        AppSettings settings = _settingsService.Load();

        if (settings.User is null)
        {
            MessageBox.Show("Сессия не найдена. Выполняется возврат к авторизации...", "Вход", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            WeakReferenceMessenger.Default.Send(new NavigateToAuthMessage());
            return;
        }

        UserInfoDto user = settings.User;
        try
        {
            UserNicknameTextBlock.Text = user.Nickname;
            BitmapImage image = await _profileApiClient.GetUserAvatarAsync(user.AvatarUrl);
            UserAvatarImage.Source = image;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить данные пользователя: {ex.Message}");
            Console.WriteLine(ex.InnerException);
        }

        try
        {
            var albums = await _profileApiClient.GetUserSavedAlbumsAsync(user.Id) ?? new List<AlbumInfoDto>();
    
            Albums.Clear();
            foreach (var albumDto in albums)
            {
                AlbumControl control = new AlbumControl();
                control.Album = albumDto;
                Albums.Add(control);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LayoutWindow] Ошибка загрузки альбомов: {ex.Message}\n{ex.StackTrace}");
            MessageBox.Show($"КРИТИЧЕСКАЯ ОШИБКА загрузки альбомов:\n\n{ex.Message}\n\n{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        SavedAlbumsListBox.ItemsSource = Albums;
        
        try
        {
            var playlists = await _profileApiClient.GetUserSavedPlaylistsAsync(user.Id) ?? new List<PlaylistInfoDto>();
            
            Playlists.Clear();
            foreach (PlaylistInfoDto playlistDto in playlists)
            {
                PlaylistControl control = new PlaylistControl();
                control.Playlist = playlistDto;
                control.Width = 280;
                Playlists.Add(control);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LayoutWindow] Ошибка загрузки плейлистов: {ex.Message}\n{ex.StackTrace}");
        }
        SavedPlaylistsListBox.ItemsSource = Playlists;
    }
    
    private void OnPlaybackEnded()
    {
        Debug.WriteLine("[LayoutWindow] OnPlaybackEnded получен!");
        Application.Current.Dispatcher.Invoke(() =>
        {
            Debug.WriteLine($"[LayoutWindow] Tracks in queue: {PlaybackService.Queue.Count}");
            
            // Предотвращаем рекурсивный вызов, если очередь пуста
            if (PlaybackService.Queue.Count == 0) 
            {
                Debug.WriteLine("[LayoutWindow] Queue is empty, playback is stopped.");
                return; 
            }
            
            if (PlaybackService.CurrentRepeatMode is RepeatMode.One)
            {
                _audioService.Seek(0);
                _audioService.Play();
                return;
            }
            
            bool hasMoreTracks = PlaybackService.NextTrack();
            
            if (!hasMoreTracks && PlaybackService.CurrentRepeatMode is RepeatMode.None)
            {
                _audioService.Stop();
            }
        });
    }

    private void ProgressTimer_Tick(object? sender, EventArgs e)
    {
        // Если пользователь тащит ползунок - НИЧЕГО не делаем
        if (_isDraggingSlider) return;

        if (_audioService.State is not (PlaybackState.Playing or PlaybackState.Paused)) return;

        double duration = _audioService.Duration;
        if (duration > 0 && Math.Abs(TimeSlider.Maximum - duration) > 0.1)
            TimeSlider.Maximum = duration;

        double current = _audioService.CurrentTime;
        if (current >= 0 && current <= duration)
        {
            TimeSlider.Value = current;
            CurrentTrackTimeTextBlock.Text = TimeSpan.FromSeconds(current).ToString(@"mm\:ss");
            TotalTrackTimeTextBlock.Text = TimeSpan.FromSeconds(_audioService.Duration).ToString(@"mm\:ss");
        }
    }

    public void NavigateToSearch(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToSearchMessage());
    public void NavigateToResonance(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToResonanceMessage());
    public void NavigateToLibrary(object sender, RoutedEventArgs args)
        => WeakReferenceMessenger.Default.Send(new NavigateToLibraryMessage());


    private async void ChangeTrack(TrackInfoDto track)
    {
        if (_isChangingTrack) return;
        _isChangingTrack = true;

        try
        {
            // 1. Останавливаем старое
            _audioService.Stop();
            _progressTimer.Stop();

            // 2. Получаем поток (Убедись, что TrackApiClient НЕ делает 'using' на HttpResponseMessage)
            Stream response = await _trackApiClient.GetTrackFileAsync(track.TrackUrl);
            BitmapImage image = await _profileApiClient.GetUserAvatarAsync(track.CoverUrl);

            TrackImage.Source = image;
            TrackNameTextBlock.Text = track.Name;
            TrackAuthorsPanel.Children.Clear();
            foreach (UserInfoDto userDto in track.UserList)
            {
                TextBlock user = new TextBlock();
                Hyperlink link = new Hyperlink();
                link.Click += (sender, e)
                    => WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(userDto.Id, false));
                link.SetResourceReference(Hyperlink.ForegroundProperty, "ContentPrimaryBrush");
                Run runText = new Run(userDto.Nickname);
                link.Inlines.Add(runText);
                user.Inlines.Add(link);
                TrackAuthorsPanel.Children.Add(user);
                TextBlock space = new TextBlock();
                space.Margin = new Thickness(5, 0, 0, 0);
                TrackAuthorsPanel.Children.Add(space);
            }

            TrackPanel.Visibility = Visibility.Visible;
            VolumeSlider.Value = _audioService.Volume;
            TimeSlider.Value = 0;

            // 3. Запускаем воспроизведение (AudioService сам скачает, декодирует и сыграет)
            await _audioService.StartAsync(response);
            _audioService.SetVolume((float)(VolumeSlider.Value));
            _progressTimer.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка воспроизведения: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _isChangingTrack = false;
        }
    }

    private void RepeatButton_Click(object sender, RoutedEventArgs e)
    {
        PlaybackService.CurrentRepeatMode = PlaybackService.CurrentRepeatMode switch
        {
            RepeatMode.None => RepeatMode.All,
            RepeatMode.All => RepeatMode.One,
            RepeatMode.One => RepeatMode.None,
            _ => RepeatMode.None
        };
        
        //UpdateRepeatButtonUI();
    }

    private void ShuffleButton_Click(object sender, RoutedEventArgs e)
    {
        PlaybackService.ToggleShuffle();
        
        if (PlaybackService.IsShuffled) 
            ShuffleToggleButton.Background = _activeButtonBackground;
        else 
            ShuffleToggleButton.Background = _defaultButtonBackground;
    }

    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_audioService.State is PlaybackState.Paused)
        {
            _audioService.Play();
        }
        else
        {
            _audioService.Pause();
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (!PlaybackService.Queue.Any())
            return;
        
        PlaybackService.PreviousTrack();
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (!PlaybackService.Queue.Any())
            return;
        
        PlaybackService.NextTrack();
    }

    private void UpdateRepeatButtonUI()
    {
        // Здесь нужно обновить внешний вид кнопки в зависимости от режима.
        // Поскольку я не вижу ваш XAML, я использую ваши переменные _defaultButtonBackground и _activeButtonBackground.
        // Рекомендую также менять ToolTip или иконку, если она у вас есть.
    
        switch (PlaybackService.CurrentRepeatMode)
        {
            case RepeatMode.None:
                RepeatToggleButton.Background = _defaultButtonBackground;
                RepeatToggleButton.ToolTip = "Повтор выключен";
                break;
            
            case RepeatMode.All:
                RepeatToggleButton.Background = _activeButtonBackground;
                RepeatToggleButton.ToolTip = "Повторять все треки";
                break;
            
            case RepeatMode.One:
                RepeatToggleButton.Background = _activeButtonBackground; // Можно сделать третий, уникальный цвет
                RepeatToggleButton.ToolTip = "Повторять один трек";
                break;
        }
    }
    

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            UserInfoDto user = _settingsService.Load().User;
            WeakReferenceMessenger.Default.Send(new NavigateToProfileMessage(user.Id, true));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось перейти в профиль");
        }
    }

    private void VolumeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_audioService is null) return;
        _audioService.SetVolume((float)e.NewValue);
    }

    private void TimeSlider_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDraggingSlider = true;
        _progressTimer.Stop();
    }
    private void TimeSlider_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _isDraggingSlider = false;
        _progressTimer.Start();
        _audioService.Seek(TimeSlider.Value);
    }
    
    private void TimeSlider_PreviewMouseLeave(object sender, MouseEventArgs e)
    {
        if (_isDraggingSlider)
        {
            _isDraggingSlider = false;
            _audioService.Seek(TimeSlider.Value);
            _progressTimer.Start();
        }
    }

    private async void ChangeCollectionToPlaylistButton_OnChecked(object sender, RoutedEventArgs e)
    {
        if (SavedAlbumsListBox is null) return;
        SavedAlbumsListBox.Visibility = Visibility.Collapsed;
        SavedPlaylistsListBox.Visibility = Visibility.Visible;
    }

    private async void ChangeCollectionToAlbumButton_OnChecked(object sender, RoutedEventArgs e)
    {
        if (SavedPlaylistsListBox is null) return;
        
        SavedPlaylistsListBox.Visibility = Visibility.Collapsed;
        SavedAlbumsListBox.Visibility = Visibility.Visible;
    }

    private void SavedPlaylistsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        PlaylistControl? control = SavedPlaylistsListBox.SelectedItem as PlaylistControl;
        if (control is not null)
            WeakReferenceMessenger.Default.Send(new NavigateToPlaylistMessage(control.Playlist));
    }

    private void SavedAlbumsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        AlbumControl? control = SavedAlbumsListBox.SelectedItem as AlbumControl;
        if (control is not null)
            WeakReferenceMessenger.Default.Send(new NavigateToAlbumMessage(control.Album));
    }

    private void QueueButton_Click(object sender, RoutedEventArgs e)
        => WeakReferenceMessenger.Default.Send(new NavigateToQueueMessage());

    private void AddToLibrary_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void RemoveFromLibrary_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void AddToPlaylistButton_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}