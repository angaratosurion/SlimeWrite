using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.Reflection;

namespace SlimeWrite.Avalonia;

public partial class About : Window
{
    public string AppName { get; }
    public string Version { get; }
    public string Website { get; } = "https://github.com/angaratosurion/SlimeWrite";
    public string Copyright { get; }
    public string Description { get; }
    public About()
    {
        InitializeComponent();
        var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        AppName = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
            "SlimeWrite";
        Version = asm.GetName()?.Version?.ToString() ?? "1.0.0";
        Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>()?.
            Copyright;

        Description = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.
            Description;
        AppNameText.Text = AppName;
        VersionText.Text = $"Version {Version}";
        CopyRightText.Text = "© " + DateTime.Now.Year + " " + Copyright;
        DescriptionText.Text = Description;


        try
        {
            var uri = new Uri("/Assets/Images/logo.png",
                UriKind.Absolute);

        }

        catch { }
    }
    private void WebsiteHyperlink_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(Website) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            //MessageBox.Show(this, $"Couldn't open thee Web Site: {ex.Message}",
            //    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();


}