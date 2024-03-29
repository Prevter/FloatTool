﻿/*
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

using FloatTool.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FloatTool
{
	public sealed partial class UpdateWindow : Window
    {
        public UpdateResult UpdateResult;

        public UpdateWindow(UpdateResult updateResult)
        {
            InitializeComponent();
            DataContext = updateResult;
            UpdateResult = updateResult;
            Markdown.Xaml.Markdown engine = new();
            var document = engine.Transform(updateResult.Body);
            document.FontFamily = new System.Windows.Media.FontFamily("Segoe UI");
            document.FontSize = 14;
            ChangelogTextBox.Document = document;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.GetPosition(this).Y < 40) DragMove();
        }

        private void LaterButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DoNotAskButtonClick(object sender, RoutedEventArgs e)
        {
            AppHelpers.Settings.CheckForUpdates = false;
            AppHelpers.Settings.Save();
            Close();
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            UpdateButton.IsEnabled = false;
            DownloadProgress.Visibility = Visibility.Visible;
            string archiveUrl = UpdateResult.Assets[0].BrowserDownloadUrl;
            Task.Run(async () =>
            {
                // Download the archive
                using HttpClient client = new();
                var progress = new Progress<float>(value =>
                {
                    DownloadProgress.Dispatcher.Invoke(() => DownloadProgress.Value = value * 100);
                });

                using (var file = new FileStream("update.zip", FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await client.DownloadAsync(archiveUrl, file, progress);
                }

                // Rename all locked files to .old
                string folderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                foreach (string file in Directory.GetFiles(folderPath, "*", SearchOption.TopDirectoryOnly))
                {
                    Logger.Log.Debug($"Checking {file}");
                    if (App.IsFileLocked(file) || file.EndsWith(".json"))
                    {
                        File.Move(file, file + ".old");
                    }
                }

                // Extract the archive
                try
                {
                    ZipFile.ExtractToDirectory("./update.zip", ".");
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Error while extracting the update archive", ex);
                }

                File.Delete("update.zip");
                ProcessStartInfo startInfo = new()
                {
                    FileName = "FloatTool.exe",
                    Arguments = "--clean-update"
                };
                Process.Start(startInfo);
                Environment.Exit(0);
            });
        }
    }
}
