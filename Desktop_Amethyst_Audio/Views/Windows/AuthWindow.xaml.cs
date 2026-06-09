using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation;
using Desktop_Amethyst_Audio.Views.Pages;

namespace Desktop_Amethyst_Audio.Views.Windows;

public partial class AuthWindow : Window
{
    private AuthPage AuthPage { get; } = new();
    private RegisterPage RegisterPage { get; } = new();
    
    public AuthWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(AuthPage);
        
        WeakReferenceMessenger.Default.Register<NavigateToAuthMessage>(this, (r, m) => MainFrame.Navigate(AuthPage));
        WeakReferenceMessenger.Default.Register<NavigateToForgetPasswordMessage>(this, (r, m) => MainFrame.Navigate(new ForgetPasswordPage()));
        WeakReferenceMessenger.Default.Register<NavigateToRegisterMessage>(this, (r, m) => MainFrame.Navigate(RegisterPage));
        WeakReferenceMessenger.Default.Register<NavigateToMainLayoutMessage>(this, (recipient, message) =>
        {
            LayoutWindow window = new LayoutWindow();
            window.Show();
            this.Close();
        });
    }
}