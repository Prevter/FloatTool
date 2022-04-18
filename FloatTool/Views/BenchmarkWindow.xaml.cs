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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FloatTool
{
    public partial class BenchmarkWindow : Window
    {
        public BenchmarkViewModel Context;
        public Settings Settings;
        public static long PassedCombinations;

        public BenchmarkWindow(Settings settings)
        {
            Logger.Log.Info("Opened benchmark window");
            Context = new BenchmarkViewModel(settings);
            DataContext = Context;

            if (settings.DiscordRPC)
            {
                App.DiscordClient.SetPresence(new DiscordRPC.RichPresence()
                {
                    Details = Application.Current.Resources["m_Benchmarking"] as string,
                    State = $"CPU: {Context.CurrentCpuName}",
                    Assets = new DiscordRPC.Assets()
                    {
                        LargeImageKey = "icon_new",
                        LargeImageText = $"FloatTool {App.VersionCode}",
                    },
                });
            }

            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.GetPosition(this).Y < 40) DragMove();
        }

        private void WindowButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Context.PollBenchmarkResults();
        }

        private void PublishResult_Click(object sender, RoutedEventArgs e)
        {
            Context.PushBenchmarkResults();
        }

        private static void FloatCraftWorkerThread(CraftSearchSetup options)
        {
            foreach (InputSkin[] resultList in Calculations.Combinations(options.SkinPool, options.ThreadID, options.ThreadCount))
            {
                for (int i = 0; i < options.Outcomes.Length; ++i)
                {
                    decimal resultFloat = Math.Round(Calculations.Craft(
                        resultList, options.Outcomes[i].MinFloat, options.Outcomes[i].FloatRange)
                        , 14);
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
                        if (options.SearchMode == SearchMode.Equal) { }

                    }
                }

                Interlocked.Increment(ref PassedCombinations);
            }
        }

        private void StartBenchmark_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log.Info("Started benchmarking");

            new Thread(() =>
            {
                Skin[] outcomes = new Skin[] {
                    new Skin("AK-47 | Safari Mesh", 0.06f, 0.8f, Skin.Quality.Industrial)
                };

                double[] pool = {
                    0.246938750147820, 0.196652039885521, 0.154839321970940,
                    0.333326697349548, 0.163415759801865, 0.291821509599686,
                    0.374309629201889, 0.378754675388336, 0.231419935822487,
                    0.311867892742157, 0.374067693948746, 0.377068012952805,
                    0.244467452168465, 0.355135351419449, 0.352264583110809,
                    0.227853879332542, 0.340960860252380, 0.375657349824905,
                    0.157685652375221, 0.217334255576134, 0.217334255576134,
                    0.157685652375221, 0.217334255576134, 0.217334255576134,
                    0.157685652375221, 0.217334255576134, 0.217334255576134,
                    0.157685652375221, 0.217334255576134, 0.217334255576134
                };

                List<InputSkin> inputSkinsList = new();
                foreach (double f in pool)
                    inputSkinsList.Add(new InputSkin(f, 0.03f, Currency.USD));

                InputSkin[] inputSkins = inputSkinsList.ToArray();

                Context.ButtonsEnabled = false;
                PassedCombinations = 0;
                Context.TotalCombinations = Calculations.GetCombinationsCount(pool.Length);
                Context.MultithreadedSpeed = 0;
                Context.SinglethreadedSpeed = 0;
                Context.ProgressPercentage = 0;

                string searchFilter = "0.25000000";

                decimal searched = decimal.Parse(searchFilter, CultureInfo.InvariantCulture);
                decimal precission = (decimal)Math.Pow(0.1, searchFilter.Length - 2);

                // Create thread pool
                List<Task> threadPool = new();
                int threads = Context.ThreadCount;

                Stopwatch timer = Stopwatch.StartNew();

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
                                SearchMode = SearchMode.Equal,
                                ThreadID = startIndex,
                                ThreadCount = threads,
                            }
                        ));
                        threadPool.Add(newThread);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Error starting up thread pool", ex);
                }

                while (true)
                {
                    bool isAnyRunning = false;
                    foreach (Task t in threadPool)
                    {
                        if (t.Status != TaskStatus.RanToCompletion)
                        {
                            isAnyRunning = true;
                            break;
                        }
                    }

                    Context.ProgressPercentage = (int)(PassedCombinations * 100 / Context.TotalCombinations);

                    if (!isAnyRunning)
                        break;

                    Thread.Sleep(5);
                }

                timer.Stop();

                float speed = PassedCombinations * 1000 / timer.ElapsedMilliseconds;

                Context.MultithreadedSpeed = (int)speed;
                Context.SinglethreadedSpeed = (int)(speed / threads);
                Context.CanPublish = true;
                Context.ButtonsEnabled = true;

                Logger.Log.Info($"Benchmarking finished. Speed = {speed}; Threads = {threads}");
            }).Start();
        }
    }
}
