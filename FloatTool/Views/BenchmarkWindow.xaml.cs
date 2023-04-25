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

using FloatTool.Common;
using FloatTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FloatTool
{
	public sealed partial class BenchmarkWindow : Window
    {
        public BenchmarkViewModel Context;
        private static long PassedCombinations;

        public BenchmarkWindow()
        {
            Logger.Log.Info("Opened benchmark window");
            Context = new BenchmarkViewModel();
            DataContext = Context;

            if (AppHelpers.Settings.DiscordRPC)
            {
                AppHelpers.DiscordClient.SetPresence(new DiscordRPC.RichPresence()
                {
                    Details = Application.Current.Resources["m_Benchmarking"] as string,
                    State = $"CPU: {Context.CurrentCpuName}",
                    Assets = new DiscordRPC.Assets()
                    {
                        LargeImageKey = "icon_new",
                        LargeImageText = $"FloatTool {AppHelpers.VersionCode}",
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
            int size = options.SkinPool.Length - 10;
            int[] numbers = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            InputSkin[] resultList = new InputSkin[10];
            bool running = true;

            running = Calculations.NextCombination(numbers, size, options.ThreadID);

            while (running)
            {
                for (int i = 0; i < 10; ++i)
                    resultList[i] = options.SkinPool[numbers[i]];

                // Check if the combination is valid

                for (int i = 0; i < options.Outcomes.Length; ++i)
                {
                    double resultFloat = Calculations.Craft(
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
                        if (Math.Round(resultFloat, 14, MidpointRounding.AwayFromZero)
                            .ToString(CultureInfo.InvariantCulture)
                            .StartsWith(options.SearchFilter, StringComparison.Ordinal)
                            || options.SearchMode != SearchMode.Equal)
                        {
                            _ = (InputSkin[])resultList.Clone();
                        }
                    }
                }

                Interlocked.Increment(ref PassedCombinations);

                // Get next combination
                running = Calculations.NextCombination(numbers, size, options.ThreadCount);
            }

        }

        private void StartBenchmark_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log.Info("Started benchmarking");

            new Thread(() =>
            {
                Skin[] outcomes = new Skin[] {
                    new Skin("AK-47 | Safari Mesh", 0.06, 0.8, Quality.Industrial)
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
                    0.157685652375221, 0.217334255576134, 0.217334255576134,
					0.157685652375221, 0.217334255576134, 0.217334255576134,
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
                Context.ThreadCountTested = Context.ThreadCount;

                string searchFilter = "0.250000000";

                double searched = double.Parse(searchFilter, CultureInfo.InvariantCulture);
                double precission = Math.Pow(0.1, searchFilter.Length - 2);

                // Create thread pool
                List<Task> threadPool = new();
                int threads = Context.ThreadCount;

                long startTime = Stopwatch.GetTimestamp();
				ParallelLoopResult? parallel = null;

				try
				{
                    if (AppHelpers.Settings.UseParallel)
                    {
                        Task.Run(() =>
                        {
                            parallel = Parallel.For(0, threads, i =>
                            {
                                FloatCraftWorkerThread(new CraftSearchSetup
                                {
                                    SkinPool = inputSkins,
                                    Outcomes = outcomes,
                                    SearchTarget = searched,
                                    TargetPrecision = precission,
                                    SearchFilter = searchFilter,
                                    SearchMode = SearchMode.Equal,
                                    ThreadID = i,
                                    ThreadCount = threads,
                                });
                            });
                        });
                    }
                    else
                    {
						for (int i = 0; i < threads; i++)
						{
							int startIndex = i;
							Task newThread = Task.Factory.StartNew(() => FloatCraftWorkerThread(
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
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Error starting up thread pool", ex);
                }

                while (true)
                {
                    bool isAnyRunning;
					if (AppHelpers.Settings.UseParallel)
                    {
						isAnyRunning = true;
						if (parallel.HasValue)
						{
							isAnyRunning = !parallel.Value.IsCompleted;
						}
					}
                    else
                    {
						isAnyRunning = false;
						foreach (Task t in CollectionsMarshal.AsSpan(threadPool))
						{
							if (t.Status != TaskStatus.RanToCompletion)
							{
								isAnyRunning = true;
								break;
							}
						}
					}

                    Context.ProgressPercentage = (int)(PassedCombinations * 100 / Context.TotalCombinations);

                    if (!isAnyRunning)
                        break;

                    Thread.Sleep(5);
                }

                long endTime = Stopwatch.GetTimestamp();
                double millis = Utils.GetTimePassed(startTime, endTime).TotalMilliseconds;
                double speed = PassedCombinations * 1000 / millis;

                Context.MultithreadedSpeed = (int)speed;
                Context.SinglethreadedSpeed = (int)(speed / threads);
                Context.CanPublish = true;
                Context.ButtonsEnabled = true;

                Logger.Log.Info($"Benchmarking finished. Speed = {speed}; Threads = {threads}");
            }).Start();
        }
    }
}
