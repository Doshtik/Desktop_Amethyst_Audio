using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace Desktop_Amethyst_Audio.Views.Windows;

public partial class AuthByProvidersFormWindow : Window
{
    public string StartUrl { get; set; }
    
    public AuthByProvidersFormWindow()
    {
        InitializeComponent();
    }

    private void ProviderWebView_OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        
    }
}