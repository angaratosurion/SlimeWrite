using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SlimeWrite
{
    public partial class About  : Window
    {
        public string AppName { get; }
        public string Version { get; }
        public string Website { get; } = "https://github.com/angaratosurion/SlimeWrite";
        public string Copyright { get; }
        public string Description { get; }



        public About ()
        {
            InitializeComponent();


            var asm = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            AppName = asm.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ??
                "SlimeWritep";
            Version = asm.GetName()?.Version?.ToString() ?? "1.0.0";
            Copyright = asm.GetCustomAttribute<AssemblyCopyrightAttribute>().
                Copyright;

            Description = asm.GetCustomAttribute<AssemblyDescriptionAttribute>().
                Description;
            AppNameText.Text = AppName;
            VersionText.Text = $"Version {Version}";
            CopyRightText.Text = "© " + DateTime.Now.Year+" "+ Copyright;
            DescriptionText.Text = Description;


            try
            {
                var uri = new Uri("pack://application:,,,/Assets/Images/logo.png", 
                    UriKind.Absolute);
                LogoImage.Source = new BitmapImage(uri);
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
                MessageBox.Show(this, $"Couldn't open thee Web Site: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //private void CopyInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine(AppName);
        //    sb.AppendLine($"Version: {Version}");
        //    sb.AppendLine($"Web Site: {Website}");
        //    sb.AppendLine("© "+DateTime.Now.Year);


        //    try
        //    {
        //        Clipboard.SetText(sb.ToString());
        //        MessageBox.Show(this, "Οι πληροφορίες αντιγράφηκαν στο πρόχειρο.", "Αντιγραφή", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
        //    catch
        //    {
        //        MessageBox.Show(this, "Απέτυχε η αντιγραφή στο πρόχειρο.", "Σφάλμα", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}


        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
