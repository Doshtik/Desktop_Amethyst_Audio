using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Desktop_Amethyst_Audio.Messages.Navigation;

namespace Desktop_Amethyst_Audio.Views.Windows;

public partial class AuthWindow : Window
{
    public AuthWindow()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<NavigateToMainLayoutMessage>(this, (recipient, message) =>
        {
            LayoutWindow window = new LayoutWindow();
            window.Show();
            this.Close();
        });
    }

    private void ForgetPasswordTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        throw new NotImplementedException();
    }
}