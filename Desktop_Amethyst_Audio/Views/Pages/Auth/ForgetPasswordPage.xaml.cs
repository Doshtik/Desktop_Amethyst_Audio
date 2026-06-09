using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation;

namespace Desktop_Amethyst_Audio.Views.Pages;

public partial class ForgetPasswordPage : Page
{
    public ForgetPasswordPage()
    {
        InitializeComponent();
    }

    private void BackToLoginButton_OnClick(object sender, RoutedEventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new NavigateToAuthMessage());
    }
}