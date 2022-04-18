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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using DiscordRPC;

namespace FloatTool
{
    public partial class App : Application
    {
        public static ResourceDictionary ThemeDictionary { get; set; }
        public static List<string> ThemesFound { get; set; }
        public static FileSystemWatcher Watcher;
        public static DiscordRpcClient DiscordClient;
        public static string VersionCode;

        public static void SelectCulture(string culture)
        {
            if (string.IsNullOrEmpty(culture))
                return;

            //Copy all MergedDictionarys into a auxiliar list.
            var dictionaryList = Current.Resources.MergedDictionaries.ToList();

            //Search for the specified culture.     
            string requestedCulture = $"Languages/Lang.{culture}.xaml";
            var resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                //If not found, select our default language.             
                requestedCulture = "Languages/Lang.xaml";
                resourceDictionary = dictionaryList.
                    FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            //If we have the requested resource, remove it from the list and place at the end.     
            //Then this language will be our string table to use.      
            if (resourceDictionary != null)
            {
                Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //Inform the threads of the new culture.     
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        public static void SelectTheme(string themeURI)
        {
            if (string.IsNullOrEmpty(themeURI))
                return;

            // Preload
            if (ThemeDictionary is null)
                ThemeDictionary = (ResourceDictionary)LoadComponent(new Uri("/Theme/Schemes/Dark.xaml", UriKind.Relative));

            if (themeURI.StartsWith("/Theme/Schemes"))
                ThemeDictionary.Source = new Uri(themeURI, UriKind.Relative);
            else if (File.Exists(themeURI))
                ThemeDictionary.Source = new Uri(themeURI);

            Current.Resources.MergedDictionaries.Add(ThemeDictionary);
        }

        public App()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionCode = $"v.{version.Major}.{version.MajorRevision}.{version.Minor}";
            Logger.Initialize();
            Logger.Log.Info($"FloatTool {VersionCode}");
            Logger.Log.Info($"OS: {Environment.OSVersion}");
            Logger.Log.Info($"Memory: {Environment.WorkingSet / 1024 / 1024} MB");
            Logger.Log.Info($"Culture: {Thread.CurrentThread.CurrentCulture}");
            
            //Get path for %AppData%
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var subfolder = "floattool";
            var combined = Path.Combine(appdata, subfolder);

            //Check if folder exists
            if (!Directory.Exists(combined))
                Directory.CreateDirectory(combined);

            //Check themes folder
            var themesFolder = Path.Combine(combined, "themes");
            if (!Directory.Exists(themesFolder))
                Directory.CreateDirectory(themesFolder);

            ThemesFound = new List<string>
            {
                "/Theme/Schemes/Dark.xaml", "/Theme/Schemes/Light.xaml"
            };

            DirectoryInfo d = new(themesFolder);
            FileInfo[] Files = d.GetFiles("*.xaml");

            foreach (FileInfo file in Files)
                ThemesFound.Add(file.FullName);

            Watcher = new FileSystemWatcher
            {
                Path = themesFolder,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            // Add event handlers.
            Watcher.Changed += new FileSystemEventHandler(OnChanged);
            Watcher.Created += new FileSystemEventHandler(OnChanged);
            Watcher.Deleted += new FileSystemEventHandler(OnChanged);
            Watcher.Renamed += new RenamedEventHandler(OnRenamed);

            Watcher.EnableRaisingEvents = true;
            Trace.WriteLine("Started FileWatcher");

            DiscordClient = new DiscordRpcClient("734042978246721537");
            DiscordClient.Initialize();
        }

        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;
        private static bool IsFileLocked(string file)
        {
            if (File.Exists(file) == true)
            {
                FileStream stream = null;
                try
                {
                    stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                }
                catch (Exception ex2)
                {
                    int errorCode = Marshal.GetHRForException(ex2) & ((1 << 16) - 1);
                    if ((ex2 is IOException) && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
                    {
                        return true;
                    }
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
            }
            return false;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Trace.WriteLine($"File: {e.FullPath} {e.ChangeType}");
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    ThemesFound.Remove(e.FullPath);
                    break;
                case WatcherChangeTypes.Created:
                    ThemesFound.Add(e.FullPath);
                    break;
                case WatcherChangeTypes.Changed:
                    if (ThemeDictionary.Source.IsAbsoluteUri && e.FullPath == ThemeDictionary.Source.LocalPath)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            int retries = 0;
                            while (IsFileLocked(e.FullPath) && retries++ < 10)
                                Thread.Sleep(500);

                            if (retries != 10)
                                SelectTheme(e.FullPath);
                        }), DispatcherPriority.ContextIdle);
                    }
                    break;
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            Trace.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
            ThemesFound[ThemesFound.IndexOf(e.OldFullPath)] = e.FullPath;
        }
    }

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            return value.Equals(param);
        }

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            return (bool)value ? param : Binding.DoNothing;
        }
    }
}
