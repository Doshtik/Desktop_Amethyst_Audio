using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace Desktop_Amethyst_Audio.Views.ModalWindows;

public partial class AuthByProvidersModalWindow : Window
{
    public AuthByProvidersModalWindow()
    {
        InitializeComponent();
    }

    private void ProviderWebView_OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        throw new NotImplementedException();
    }
}