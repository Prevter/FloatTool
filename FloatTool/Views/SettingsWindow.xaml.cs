using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace FloatTool
{
    public partial class SettingsWindow : Window
    {
        public Settings Settings;

        public SettingsWindow(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
            DataContext = new SettingsViewModel(settings);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.GetPosition(this).Y < 40) DragMove();
        }

        private void WindowButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Save();
            Close();
        }

        private void OpenThemesFolder_Click(object sender, RoutedEventArgs e)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var subfolder = "floattool\\themes";
            var combined = Path.Combine(appdata, subfolder);
            Process.Start("explorer.exe", combined);
        }

        private void GetThemes_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://prevter.github.io/FloatTool-GUI/themes.html", UseShellExecute = true });
        }
    }
}
