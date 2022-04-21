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

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
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
        public static CancellationTokenSource TokenSource = new CancellationTokenSource();
        public CancellationToken CancellationToken;

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

            Logger.Log.Info("Main window started");
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
                    Logger.Log.Info("Closing");
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

        private void SetStatus(string stringCode)
        {
            if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
                StatusBar.SetResourceReference(TextBlock.TextProperty, stringCode);
            else
                Dispatcher.Invoke(new Action(() => { SetStatus(stringCode); }));
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
                            gotResult = resultFloat < options.SearchTarget;
                            break;
                        case SearchMode.Greater:
                            gotResult = resultFloat > options.SearchTarget;
                            break;
                    }

                    if (gotResult)
                    {
                        if (options.SearchMode != SearchMode.Equal ||
                            Math.Round(resultFloat, 14, MidpointRounding.AwayFromZero)
                            .ToString(CultureInfo.InvariantCulture)
                            .StartsWith(options.SearchFilter, StringComparison.Ordinal))
                        {
                            float price = 0;
                            float ieeesum = 0;
                            foreach (var skin in resultList)
                            {
                                price += skin.Price;
                                ieeesum += (float)skin.WearValue;
                            }
                            ieeesum /= 10;
                            float ieee = ((float)options.Outcomes[i].MaxFloat - (float)options.Outcomes[i].MinFloat) * ieeesum + (float)options.Outcomes[i].MinFloat;
                            
                            Dispatcher.Invoke(new Action(() =>
                            {
                                ViewModel.FoundCombinations.Add(new Combination
                                {
                                    Wear = resultFloat,
                                    OutcomeName = options.Outcomes[i].Name,
                                    Inputs = resultList,
                                    Currency = resultList[0].SkinCurrency,
                                    Price = price,
                                    IEEE754 = ((double)ieee).ToString("0.000000000000000", CultureInfo.InvariantCulture)
                                });
                            }), DispatcherPriority.ContextIdle);
                        }
                    }
                }

                Interlocked.Increment(ref PassedCombinations);

                if (CancellationToken.IsCancellationRequested)
                    break;
            }
        }

        private void StartSearchButton_Click(object sender, RoutedEventArgs e)
        {
            bool stopAfterHit = (sender as Button).Name == "FindOneButton";

            if (ViewModel.CanEditSettings)
            {
                ViewModel.FoundCombinations.Clear();
                StartButton.SetResourceReference(ContentProperty, "m_Stop");

                TokenSource.Dispose();
                TokenSource = new CancellationTokenSource();
                CancellationToken = TokenSource.Token;
                
                if (Settings.DiscordRPC)
                {
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
                }
                
                new Thread(() =>
                {
                    Logger.Log.Info("Starting up search");
                    SetStatus("m_SearchingSkin");
                    PassedCombinations = 0;
                    ViewModel.CanEditSettings = false;
                    ViewModel.ProgressPercentage = 0;
                    ViewModel.TotalCombinations = Calculations.GetCombinationsCount(ViewModel.SkinCount);

                    int index = 0;
                    Skin[] outcomes = Array.Empty<Skin>();
                    bool found = false;
                    foreach (var outcome in ViewModel.Outcomes.Values)
                    {
                        if (index++ == ViewModel.OutcomeIndex)
                        {
                            outcomes = new Skin[] { outcome[0] };
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        List<Skin> everything = new();
                        foreach (var outcome in ViewModel.Outcomes.Values)
                            everything.Add(outcome[0]);
                        outcomes = everything.ToArray();
                    }

                    ConcurrentBag<InputSkin> inputSkinBag = new();
                    List<Task> downloaderTasks = new();

                    string url = $"https://steamcommunity.com/market/listings/730/{ ViewModel.FullSkinName }/render/?count={ ViewModel.SkinCount }&start={ ViewModel.SkinSkipCount }&currency={ (int)Settings.Currency }";
                    try
                    {
                        using var client = new HttpClient();
                        HttpResponseMessage response = client.GetAsync(url).Result;
                        response.EnsureSuccessStatusCode();
                        string responseBody = response.Content.ReadAsStringAsync().Result;
                        dynamic r = JsonConvert.DeserializeObject(responseBody);
                        
                        if (r["success"] == false)
                        {
                            Logger.Log.Error($"Steam haven't returned a success code\n{r.ToString()}");
                            throw new ValueUnavailableException("Steam server returned error.");
                        }
                        
                        SetStatus("m_GettingFloats");

                        Dictionary<Task<decimal>, float> floatTasks = new();
                        foreach (var skin in r["listinginfo"])
                        {
                            string lid = r["listinginfo"][skin.Name]["listingid"].ToString();
                            string aid = r["listinginfo"][skin.Name]["asset"]["id"].ToString();
                            string link = r["listinginfo"][skin.Name]["asset"]["market_actions"][0]["link"].ToString();
                            link = link.Replace("%assetid%", aid).Replace("%listingid%", lid);
                            try
                            {
                                floatTasks.Add(
                                    Utils.GetWearFromInspectURL(link),
                                    (float.Parse(r["listinginfo"][skin.Name]["converted_price"].ToString()) +
                                    float.Parse(r["listinginfo"][skin.Name]["converted_fee"].ToString())) / 100
                                );
                            }
                            catch (Exception ex)
                            {
                                Logger.Log.Error($"Error getting float from link {link}", ex);
                            }
                        }

                        foreach (var task in floatTasks.Keys)
                        {
                            try
                            {
                                inputSkinBag.Add(new InputSkin(
                                    task.Result,
                                    floatTasks[task],
                                    Settings.Currency
                                ));

                                ViewModel.ProgressPercentage = (float)inputSkinBag.Count * 100 / floatTasks.Count;
                            }
                            catch (Exception ex)
                            {
                                Logger.Log.Error($"Error getting float from task {task.Id}", ex);
                            }
                        }
                    }
                    catch (ValueUnavailableException)
                    {
                        SetStatus("m_ErrorCouldntGetFloats");
                        Dispatcher.Invoke(
                            new Action(() =>
                            {
                                StartButton.SetResourceReference(ContentProperty, "m_Start");
                                ViewModel.CanEditSettings = true;
                            })
                        );
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("Error getting floats from marketplace", ex);
                        SetStatus("m_ErrorCouldntGetFloats");
                    }

                    if (inputSkinBag.Count < 10)
                    {
                        Logger.Log.Error("Couldn't get more than 10 floats from marketplace");
                        SetStatus("m_ErrorLessThan10");
                        Dispatcher.Invoke(
                            new Action(() =>
                            {
                                StartButton.SetResourceReference(ContentProperty, "m_Start");
                                ViewModel.CanEditSettings = true;
                            })
                        );
                        return;
                    }

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

                    SetStatus("m_StartingUpSearch");

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
                        Logger.Log.Error("Error starting up thread pool", ex);
                    }

                    SetStatus("m_Searching");
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

                        if (stopAfterHit && ViewModel.FoundCombinations.Count >= 1)
                        {
                            TokenSource.Cancel();
                            break;
                        }

                        Thread.Sleep(100);
                    }

                    Logger.Log.Info("Finished searching");
                    Dispatcher.Invoke(
                        new Action(() =>
                        {
                            StartButton.SetResourceReference(ContentProperty, "m_Start");
                            ViewModel.CanEditSettings = true;
                            SetStatus("m_FinishedSearching");
                        })
                    );

                }).Start();
            }
            else
            {
                Logger.Log.Info("Canceling task");
                TokenSource.Cancel();
            }
        }
    }
}
