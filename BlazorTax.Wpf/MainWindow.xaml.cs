using System.IO;
using System.Windows;
using Microsoft.AspNetCore.Components.WebView;

namespace BlazorTax.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnBlazorWebViewInitialized(object sender, BlazorWebViewInitializedEventArgs e)
    {
        File.WriteAllText(
            Path.Combine(Path.GetTempPath(), "sostax_webview.log"),
            $"BlazorWebView initialized at {DateTime.Now}\nWebView2 version: {e.WebView.CoreWebView2.Environment.BrowserVersionString}");

        e.WebView.CoreWebView2.WebMessageReceived += (_, args) =>
            File.AppendAllText(Path.Combine(Path.GetTempPath(), "sostax_webview.log"),
                "\nMsg: " + args.WebMessageAsJson);

        e.WebView.CoreWebView2.NavigationCompleted += (_, args) =>
            File.AppendAllText(Path.Combine(Path.GetTempPath(), "sostax_webview.log"),
                $"\nNav completed: success={args.IsSuccess}, url={e.WebView.CoreWebView2.Source}");
    }
}
