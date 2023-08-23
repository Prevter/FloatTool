/*
- Copyright(C) 2022-2023 Prevter
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

using DiscordRPC;
using FloatTool.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace FloatTool
{
	public partial class App : Application
    {
        public static ResourceDictionary ThemeDictionary { get; set; }

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
            ThemeDictionary ??= (ResourceDictionary)LoadComponent(new Uri("/Theme/Schemes/Dark.xaml", UriKind.Relative));

            if (themeURI.StartsWith("/Theme/Schemes"))
                ThemeDictionary.Source = new Uri(themeURI, UriKind.Relative);
            else if (File.Exists(themeURI))
                ThemeDictionary.Source = new Uri(themeURI);

            Current.Resources.MergedDictionaries.Add(ThemeDictionary);
        }

        public App()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            AppHelpers.VersionCode = $"v.{version.Major}.{version.Minor}.{version.Build}";

            Logger.Initialize();
            Logger.Log.Info($"FloatTool {AppHelpers.VersionCode}");
            Logger.Log.Info($"OS: {Environment.OSVersion}");
            Logger.Log.Info($"Memory: {Environment.WorkingSet / 1024 / 1024} MB");
            Logger.Log.Info($"Culture: {Thread.CurrentThread.CurrentCulture}");

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                Logger.Log.Error("Unhandled exception", (Exception)e.ExceptionObject);

            DispatcherUnhandledException += (s, e) =>
            {
                Logger.Log.Error("Dispatcher Unhandled Exception", e.Exception);
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                Logger.Log.Error("Unobserved Unhandled Exception", e.Exception);
                e.SetObserved();
            };

            Utils.GetApiUrl();

			//Get path for %AppData%
			var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var subfolder = "floattool";
            var combined = Path.Combine(appdata, subfolder);

            AppHelpers.AppDirectory = combined;

            //Check if folder exists
            if (!Directory.Exists(combined))
                Directory.CreateDirectory(combined);

            //Check themes folder
            var themesFolder = Path.Combine(combined, "themes");
            if (!Directory.Exists(themesFolder))
                Directory.CreateDirectory(themesFolder);
            AppHelpers.ThemesFound = new List<string>
            {
                "/Theme/Schemes/Dark.xaml", "/Theme/Schemes/Light.xaml"
            };

            DirectoryInfo d = new(themesFolder);
            FileInfo[] Files = d.GetFiles("*.xaml");

            foreach (FileInfo file in Files)
                AppHelpers.ThemesFound.Add(file.FullName);
            AppHelpers.Watcher = new FileSystemWatcher
            {
                Path = themesFolder,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            AppHelpers.Watcher.Changed += new FileSystemEventHandler(OnChanged);
            AppHelpers.Watcher.Created += new FileSystemEventHandler(OnChanged);
            AppHelpers.Watcher.Deleted += new FileSystemEventHandler(OnChanged);
            AppHelpers.Watcher.Renamed += new RenamedEventHandler(OnRenamed);
            AppHelpers.Watcher.EnableRaisingEvents = true;
            AppHelpers.DiscordClient = new DiscordRpcClient("734042978246721537");
            AppHelpers.DiscordClient.Initialize();

            AppHelpers.Settings = new Settings();
            AppHelpers.Settings.Load();
        }

        public static void CleanOldFiles()
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            foreach (var oldFile in Directory.GetFiles(folderPath, "*.old", SearchOption.TopDirectoryOnly))
                File.Delete(oldFile);
        }

        void AppLoaded(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0 && e.Args.Contains("--clean-update"))
            {
                CleanOldFiles();
            }
        }

        const int ERROR_SHARING_VIOLATION = 32;
        const int ERROR_LOCK_VIOLATION = 33;
        public static bool IsFileLocked(string file)
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
                    stream?.Close();
                }
            }
            return false;
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    AppHelpers.ThemesFound.Remove(e.FullPath);
                    break;
                case WatcherChangeTypes.Created:
                    AppHelpers.ThemesFound.Add(e.FullPath);
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
            AppHelpers.ThemesFound[AppHelpers.ThemesFound.IndexOf(e.OldFullPath)] = e.FullPath;
        }
    }

    public sealed class EnumToBooleanConverter : IValueConverter
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

    public sealed class DoublePrecisionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            //Logger.Debug(AppHelpers.Settings.ToString());
            return AppHelpers.Settings.ExtensionType switch
            {
                ExtensionType.SteamInventoryHelper => string.Format(CultureInfo.InvariantCulture, "{0:R}", value),
                _ => string.Format(CultureInfo.InvariantCulture, "{0:0.00000000000000}", value),
            };
        }

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }


}
