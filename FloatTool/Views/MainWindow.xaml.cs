﻿using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace FloatTool
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel;
        public Settings Settings;
        public static long PassedCombinations;
        public static List<Task> ThreadPool;
        
        public void UpdateRichPresence()
        {
            if (Settings.DiscordRPC)
            {
                App.DiscordClient.SetPresence(new DiscordRPC.RichPresence()
                {
                    Details = Application.Current.Resources["m_SettingUpSearch"] as string,
                    Assets = new DiscordRPC.Assets()
                    {
                        LargeImageKey = "icon_new",
                        LargeImageText = $"FloatTool {App.VersionCode}",
                    }
                });
            }
        }

        public MainWindow()
        {
            Settings = new Settings();
            Settings.TryLoad();
            App.SelectCulture(Settings.LanguageCode);
            App.SelectTheme(Settings.ThemeURI);

            ViewModel = new MainViewModel("Nova", "Predator", "Field-Tested", "0.25000000032783", 100, 20, Settings);

            MaxHeight = SystemParameters.WorkArea.Height + 12;
            MaxWidth = SystemParameters.WorkArea.Width + 12;

            UpdateRichPresence();
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.GetPosition(this).Y < 40) DragMove();
        }

        private void WindowButton_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "CloseButton":
                    Environment.Exit(Environment.ExitCode);
                    break;
                case "MaximizeButton":
                    WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
                    break;
                case "MinimizeButton":
                    WindowState = WindowState.Minimized;
                    break;
                case "DiscordButton":
                    Process.Start(new ProcessStartInfo { FileName = "https://discord.gg/RM9VrzMfhP", UseShellExecute = true });
                    break;
                case "BenchmarkButton":
                    new BenchmarkWindow(Settings).ShowDialog();
                    break;
                case "SettingsButton":
                    new SettingsWindow(Settings).ShowDialog();
                    break;
            }

            // Update RPC after closing any window
            UpdateRichPresence();
        }

        private void ConsoleBoxUpdated(object sender, DataTransferEventArgs e)
        {
            ((TextBox)sender).ScrollToEnd();
        }

        private void FloatCraftWorkerThread(CraftSearchSetup options)
        {
            foreach (InputSkin[] resultList in Calculations.Combinations(options.SkinPool, options.ThreadID, options.ThreadCount))
            {
                for (int i = 0; i < options.Outcomes.Length; ++i)
                {
                    decimal resultFloat = Calculations.Craft(
                        resultList, options.Outcomes[i].MinFloat, options.Outcomes[i].FloatRange
                    );
                    bool gotResult = false;

                    switch (options.SearchMode)
                    {
                        case SearchMode.Equal:
                            gotResult = Math.Abs(resultFloat - options.SearchTarget) < options.TargetPrecision;
                            break;
                        case SearchMode.Less:
                            decimal neededLess = decimal.Parse(options.SearchFilter, CultureInfo.InvariantCulture);
                            gotResult = resultFloat < neededLess;
                            break;
                        case SearchMode.Greater:
                            decimal neededGreater = decimal.Parse(options.SearchFilter, CultureInfo.InvariantCulture);
                            gotResult = resultFloat > neededGreater;
                            break;
                    }

                    if (gotResult)
                    {
                        if (options.SearchMode == SearchMode.Equal) {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                ViewModel.FoundCombinations.Add(new Combination
                                {
                                    Wear = resultFloat,
                                    OutcomeName = ViewModel.OutcomeList[i],
                                    Inputs = resultList,
                                });
                            }), DispatcherPriority.ContextIdle);
                        }
                    }
                }

                Interlocked.Increment(ref PassedCombinations);
            }
        }

        private void StartSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FoundCombinations.Clear();
            StartButton.SetResourceReference(ContentProperty, "m_Stop");

            App.DiscordClient.SetPresence(new DiscordRPC.RichPresence()
            {
                Details = Application.Current.Resources["m_Searching"] as string,
                State = $"{Application.Current.Resources["m_DesiredFloat"] as string} {ViewModel.SearchFilter}",
                Assets = new DiscordRPC.Assets()
                {
                    LargeImageKey = "icon_new",
                    LargeImageText = $"FloatTool {App.VersionCode}",
                },
                Timestamps = DiscordRPC.Timestamps.Now,
            });

            new Thread(() =>
            {
                PassedCombinations = 0;
                ViewModel.CanEditSettings = false;
                ViewModel.ProgressPercentage = 0;
                ViewModel.TotalCombinations = Calculations.GetCombinationsCount(ViewModel.SkinCount);

                int index = 0;
                Skin[] outcomes = Array.Empty<Skin>();

                foreach (var outcome in ViewModel.Outcomes.Values)
                {
                    if (index++ == ViewModel.OutcomeIndex)
                    {
                        outcomes = outcome.ToArray();
                        break;
                    }
                }

                // TODO: Have to debug wrong setup settings
                outcomes = new Skin[] {
                    new Skin("AK-47 | Safari Mesh", 0.06f, 0.8f, Skin.Quality.Industrial)
                };

                ConcurrentBag<InputSkin> inputSkinBag = new();
                List<Task> downloaderTasks = new();

                string url = $"https://steamcommunity.com/market/listings/730/{ ViewModel.FullSkinName }/render/?query=&language=english&count={ ViewModel.SkinCount }&start={ ViewModel.SkinSkipCount }&currency={ (int)Settings.Currency }";
                try
                {
                    using var client = new HttpClient();
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    dynamic r = JsonConvert.DeserializeObject(responseBody);
                    
                    ViewModel.ConsoleOutput += "Getting floats from marketplace\n";

                    Dictionary<Task<decimal>, float> floatTasks = new();
                    foreach (var skin in r["listinginfo"])
                    {
                        string lid = r["listinginfo"][skin.Name]["listingid"].ToString();
                        string aid = r["listinginfo"][skin.Name]["asset"]["id"].ToString();
                        string link = r["listinginfo"][skin.Name]["asset"]["market_actions"][0]["link"].ToString();
                        link = link.Replace("%assetid%", aid).Replace("%listingid%", lid);
                        floatTasks.Add(
                            Utils.GetWearFromInspectURL(link), 
                            (float.Parse(r["listinginfo"][skin.Name]["converted_price"].ToString()) +
                            float.Parse(r["listinginfo"][skin.Name]["converted_fee"].ToString())) / 100
                        );
                    }
                    
                    foreach (var task in floatTasks.Keys)
                    {
                        inputSkinBag.Add(new InputSkin(
                            task.Result,
                            floatTasks[task],
                            Settings.Currency
                        ));

                        ViewModel.ProgressPercentage = (float)inputSkinBag.Count * 100 / floatTasks.Count;
                    }
                }
                catch (Exception ex) { Trace.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}"); }

                List<InputSkin> inputSkinList = inputSkinBag.ToList();
                if (ViewModel.Sort)
                {
                    if (ViewModel.SortDescending)
                        inputSkinList.Sort((a, b) => b.CompareTo(a));
                    else
                        inputSkinList.Sort((a, b) => a.CompareTo(b));
                }
                InputSkin[] inputSkins = inputSkinList.ToArray();
                ViewModel.TotalCombinations = Calculations.GetCombinationsCount(inputSkinBag.Count);

                ViewModel.ConsoleOutput += $"Starting up...\n";

                string searchFilter = ViewModel.SearchFilter;

                decimal searched = decimal.Parse(searchFilter, CultureInfo.InvariantCulture);
                decimal precission = (decimal)Math.Pow(0.1, searchFilter.Length - 2);

                // Create thread pool
                ThreadPool = new();
                int threads = ViewModel.ThreadCount;

                try
                {
                    for (int i = 0; i < threads; i++)
                    {
                        var startIndex = i;
                        var newThread = Task.Factory.StartNew(() => FloatCraftWorkerThread(
                            new CraftSearchSetup
                            {
                                SkinPool = inputSkins,
                                Outcomes = outcomes,
                                SearchTarget = searched,
                                TargetPrecision = precission,
                                SearchFilter = searchFilter,
                                SearchMode = ViewModel.SearchModeSelected,
                                ThreadID = startIndex,
                                ThreadCount = threads,
                            }
                        ));
                        ThreadPool.Add(newThread);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }

                ViewModel.ConsoleOutput += $"Started threads\n";
                Stopwatch timer = Stopwatch.StartNew();
                long LastCombinationCount = 0;

                while (true)
                {
                    long timeSinceLast = timer.ElapsedMilliseconds;
                    timer.Restart();

                    bool isAnyRunning = false;
                    foreach (Task t in ThreadPool)
                    {
                        if (t.Status != TaskStatus.RanToCompletion)
                        {
                            isAnyRunning = true;
                            break;
                        }
                    }

                    if (timeSinceLast > 0)
                        ViewModel.CurrentSpeedLabel = 
                        (
                            (PassedCombinations - LastCombinationCount) 
                            * 1000 
                            / timeSinceLast
                        ).ToString("n0");
                    
                    LastCombinationCount = PassedCombinations;
                    ViewModel.ProgressPercentage = (float)PassedCombinations * 100 / ViewModel.TotalCombinations;
                    ViewModel.ParsedCombinations = PassedCombinations;
                    ViewModel.CombinationsLabel = String.Empty;

                    if (!isAnyRunning)
                        break;

                    Thread.Sleep(100);                    
                }

                ViewModel.CanEditSettings = true;

            }).Start();
        }
    }
}