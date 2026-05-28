using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class QueuePage : Page
{
    
    public QueuePage()
    {
        InitializeComponent();
    }

    private void QueuePage_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadQueueListBox();
    }

    private void LoadQueueListBox()
    {
        foreach (TrackInfoDto trackDto in PlaybackService.Queue)
        {
            QueueTrackControl control = new QueueTrackControl();
            control.Track = trackDto;
            control.Width = TrackQueueListBox.ActualWidth - 20;
            TrackQueueListBox.Items.Add(control);
        }
    }

    private void ClearQueueButton_OnClick(object sender, RoutedEventArgs e)
    {
        PlaybackService.ClearQueue();
    }

    private void TrackQueueListBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void TrackQueueListBox_OnMouseMove(object sender, MouseEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void TrackQueueListBox_OnDrop(object sender, DragEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    private void TrackClickButton_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}