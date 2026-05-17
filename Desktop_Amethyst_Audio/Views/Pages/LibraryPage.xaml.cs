using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Action;
using Desktop_Amethyst_Audio.ViewModels.PageViewModels;
using Desktop_Amethyst_Audio.Views.UserControls;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class LibraryPage : Page
{
    public LibraryPage()
    {
        InitializeComponent();
    }

    private void TrackListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TrackControl? control = TrackListBox.SelectedItem as TrackControl;
        WeakReferenceMessenger.Default.Send(new TrackChangedMessage(control.Track));
    }

    private void LibraryPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var tmp = DataContext as LibraryPageViewModel;
        tmp.PageWidth = this.ActualWidth;
    }
}