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

using FloatTool.Common;
using FloatTool.ViewModels;
using FloatTool.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace FloatTool
{
	public sealed partial class MainWindow : Window
	{
		public MainViewModel ViewModel;
		private readonly RPCSettingsPersist RPCSettings = new();

		private static long PassedCombinations;
		private static List<Task> ThreadPool;
		private static CancellationTokenSource TokenSource = new();
		public CancellationToken CancellationToken;
		private static SoundPlayer CombinationFoundSound;

		public void UpdateRichPresence(bool clear = false)
		{
			if (clear)
			{
				RPCSettings.Details = "%m_SettingUpSearch%";
				RPCSettings.State = "";
				RPCSettings.ShowTime = false;
			}

			if (AppHelpers.Settings.DiscordRPC)
			{
				AppHelpers.DiscordClient.SetPresence(RPCSettings.GetPresense());
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FloatTool.Assets.Found.wav"))
			{
				CombinationFoundSound = new SoundPlayer(stream);
				CombinationFoundSound.Load();
			}

			App.SelectCulture(AppHelpers.Settings.LanguageCode);
			App.SelectTheme(AppHelpers.Settings.ThemeURI);

			ViewModel = new MainViewModel("Nova", "Predator", "Field-Tested", "0.250000000", 100, 20, ErrorTooltip, ErrorTooltipFloat);

			MaxHeight = SystemParameters.WorkArea.Height + 12;
			MaxWidth = SystemParameters.WorkArea.Width + 12;

			UpdateRichPresence(true);
			DataContext = ViewModel;

			Logger.Info("Main window started");

			if (AppHelpers.Settings.CheckForUpdates)
			{
				Task.Factory.StartNew(() =>
				{
					var update = Utils.CheckForUpdates().Result;
					if (update != null && update.TagName != AppHelpers.VersionCode)
					{
						Dispatcher.Invoke(new Action(() =>
						{
							Logger.Info("New version available");
							var updateWindow = new UpdateWindow(update)
							{
								Owner = this
							};
							updateWindow.ShowDialog();
						}));
					}
				});
			}
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			if (e.GetPosition(this).Y < 40) DragMove();
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.F1:
					Process.Start(new ProcessStartInfo { FileName = $"{Utils.HOME_URL}/table", UseShellExecute = true });
					break;
				case Key.F2:
					Process.Start(new ProcessStartInfo { FileName = $"{Utils.HOME_URL}/tools", UseShellExecute = true });
					break;
				case Key.F3:
					string skin = $"{ViewModel.WeaponName} | {ViewModel.SkinName}";
					var collection = ViewModel.FindSkinCollection(skin);
					Process.Start(new ProcessStartInfo { FileName = collection.Link, UseShellExecute = true });
					break;
				case Key.F4:
					var link = $"https://steamcommunity.com/market/listings/730/{ViewModel.FullSkinName}";
					Process.Start(new ProcessStartInfo { FileName = link, UseShellExecute = true });
					break;
				case Key.F5:
					ViewModel.FoundCombinations.Sort((a, b) => a.Price.CompareTo(b.Price));
					break;
				case Key.F12:
					break;
			}
		}

		private void WindowButton_Click(object sender, RoutedEventArgs e)
		{
			switch (((Button)sender).Name)
			{
				case "CloseButton":
					Logger.Info("Closing");
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
					new BenchmarkWindow().ShowDialog();
					break;
				case "SettingsButton":
					new SettingsWindow().ShowDialog();
					break;
			}

			// This will return rich presense to last state and update the language
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
							InputSkin[] result = (InputSkin[])resultList.Clone();
							float price = 0;
							float ieeesum = 0;
							foreach (var skin in result)
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
									Inputs = result,
									Currency = result[0].SkinCurrency,
									Price = price,
									Wear32Bit = ((double)ieee).ToString("0.000000000000000", CultureInfo.InvariantCulture),
								});
								if (AppHelpers.Settings.Sound)
									CombinationFoundSound.Play();
							}));
						}
					}
				}

				Interlocked.Increment(ref PassedCombinations);

				if (CancellationToken.IsCancellationRequested)
					break;

				// Get next combination

				running = Calculations.NextCombination(numbers, size, options.ThreadCount);
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

				if (AppHelpers.Settings.DiscordRPC)
				{
					RPCSettings.Details = $"%m_Searching% {ViewModel.FullSkinName}";
					RPCSettings.State = $"%m_DesiredFloat% {ViewModel.SearchFilter}";
					RPCSettings.Timestamp = DiscordRPC.Timestamps.Now;
					RPCSettings.ShowTime = true;

					UpdateRichPresence();
				}

				new Thread(() =>
				{
					Logger.Info("Starting up search");
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

					string url = $"https://steamcommunity.com/market/listings/730/{ViewModel.FullSkinName}/render/?count={ViewModel.SkinCount}&start={ViewModel.SkinSkipCount}&currency={(int)AppHelpers.Settings.Currency}";
					try
					{
						using var client = new HttpClient();
						HttpResponseMessage response = client.GetAsync(url).Result;
						string responseBody = response.Content.ReadAsStringAsync().Result;
						if (!response.IsSuccessStatusCode)
						{
							Logger.Error($"Steam haven't returned a success code: {(int)response.StatusCode} {response.ReasonPhrase}\n{responseBody}");
							throw new ValueUnavailableException("Steam server returned error.");
						}
						
						dynamic r = JsonConvert.DeserializeObject(responseBody);

						if (r["success"] == false)
						{
							Logger.Error($"Steam haven't returned a success code\n{responseBody}");
							throw new ValueUnavailableException("Steam server returned error.");
						}

						SetStatus("m_GettingFloats");

						Dictionary<Task<double>, (string, float)> floatTasks = new();
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
									(lid, (float.Parse(r["listinginfo"][skin.Name]["converted_price"].ToString()) +
									float.Parse(r["listinginfo"][skin.Name]["converted_fee"].ToString())) / 100)
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
									floatTasks[task].Item2,
									AppHelpers.Settings.Currency,
									floatTasks[task].Item1
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
						UpdateRichPresence(true);
						Dispatcher.Invoke(new Action(() =>
						{
							StartButton.SetResourceReference(ContentProperty, "m_Start");
							ViewModel.CanEditSettings = true;
						}));
						return;
					}
					catch (Exception ex)
					{
						Logger.Log.Error("Error getting floats from marketplace", ex);
						SetStatus("m_ErrorCouldntGetFloats");
						UpdateRichPresence(true);
						Dispatcher.Invoke(
							new Action(() =>
							{
								StartButton.SetResourceReference(ContentProperty, "m_Start");
								ViewModel.CanEditSettings = true;
							})
						);
						return;
					}

					if (inputSkinBag.Count < 10)
					{
						Logger.Error("Couldn't get more than 10 floats from marketplace");
						SetStatus("m_ErrorLessThan10");
						UpdateRichPresence(true);
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

					// sort skins by price in ascending order
					inputSkinList.Sort((a, b) => a.Price.CompareTo(b.Price));

					if (ViewModel.Sort)
					{
						if (ViewModel.SortDescending)
							inputSkinList.Sort((a, b) => b.CompareTo(a));
						else
							inputSkinList.Sort((a, b) => a.CompareTo(b));
					}
					InputSkin[] inputSkins = inputSkinList.ToArray();
					ViewModel.TotalCombinations = Calculations.GetCombinationsCount(inputSkinBag.Count);

					// Check max and min possible floats if searching for one skin
					if (outcomes.Length == 1)
					{
						// Get 10 lowest and 10 highest floats
						List<InputSkin> sorted = inputSkinList.ToList();
						sorted.Sort((a, b) => a.CompareTo(b));
						InputSkin[] best = sorted.Take(10).ToArray();
						InputSkin[] worst = sorted.TakeLast(10).ToArray();

						// Get min and max floats by running the craft function
						double minFloat = Calculations.Craft(best, outcomes[0].MinFloat, outcomes[0].FloatRange);
						double maxFloat = Calculations.Craft(worst, outcomes[0].MinFloat, outcomes[0].FloatRange);

						Dispatcher.Invoke(new Action(() =>
						{
							// Set them to datacontext
							ViewModel.CurrentMinWear = minFloat;
							ViewModel.CurrentMaxWear = maxFloat;

							// And update the label
							ViewModel.UpdateFloatRange();
						}));
					}

					SetStatus("m_StartingUpSearch");

					string searchFilter = ViewModel.SearchFilter;

					double searched = double.Parse(searchFilter, CultureInfo.InvariantCulture);
					double precission = Math.Pow(0.1, searchFilter.Length - 2);

					// Create thread pool
					ThreadPool = new();
					int threads = ViewModel.ThreadCount;

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
										SearchMode = ViewModel.SearchModeSelected,
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
					}
					catch (Exception ex)
					{
						Logger.Log.Error("Error starting up thread pool", ex);
					}

					SetStatus("m_Searching");
					var startTime = Stopwatch.GetTimestamp();
					long LastCombinationCount = 0;

					while (true)
					{
						long newTime = Stopwatch.GetTimestamp();
						double timeSinceLast = Utils.GetTimePassed(startTime, newTime).TotalMilliseconds;
						startTime = newTime;

						bool isAnyRunning;
						if (AppHelpers.Settings.UseParallel)
						{
							isAnyRunning = true;
							if (parallel.HasValue)
								isAnyRunning = !parallel.Value.IsCompleted;
						}
						else
						{
							isAnyRunning = false;
							foreach (Task t in CollectionsMarshal.AsSpan(ThreadPool))
							{
								if (t.Status != TaskStatus.RanToCompletion)
								{
									isAnyRunning = true;
									break;
								}
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
						ViewModel.CombinationsLabel = string.Empty;

						if (!isAnyRunning)
							break;

						if (stopAfterHit && ViewModel.FoundCombinations.Count >= 1)
						{
							TokenSource.Cancel();
							break;
						}

						Thread.Sleep(100);
					}

					UpdateRichPresence(true);
					Logger.Info("Finished searching");
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
				Logger.Info("Canceling task");
				TokenSource.Cancel();
				UpdateRichPresence(true);
			}
		}
	}
}
