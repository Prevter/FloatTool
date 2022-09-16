/*
- Copyright(C) 2022 Prevter
-
- This program is free software: you can redistribute it and/or modify
- it under the terms of the GNU General Public License as published by
- the Free Software Foundation, either version 3 of the License, or
- (at your option) any later version.
-
- This program is distributed in the hope that it will be useful,
- but WITHOUT ANY WARRANTY; without even the implied warranty of
- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
- GNU General Public License for more details.
-
- You should have received a copy of the GNU General Public License
- along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace FloatTool
{
    public sealed partial class SettingsWindow : Window
    {
        public Settings Settings;

        public SettingsWindow(Settings settings)
        {
            Settings = settings;
            InitializeComponent();
            DataContext = new SettingsViewModel(settings);
            Logger.Log.Info("Opened settings window");
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.GetPosition(this).Y < 40) DragMove();
        }

        private void WindowButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Save();
            Logger.Log.Info($"Saved settings: {Settings}");
            Logger.Log.Info($"Closed settings window");
            Close();
        }

        private void OpenThemesFolder_Click(object sender, RoutedEventArgs e)
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var subfolder = "floattool\\themes";
            var combined = Path.Combine(appdata, subfolder);
            Logger.Log.Info("Opened themes folder: " + combined);
            Process.Start("explorer.exe", combined);
        }

        private void GetThemes_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://git.prevter.ml/floattool/themes", UseShellExecute = true });
        }
    }
}
