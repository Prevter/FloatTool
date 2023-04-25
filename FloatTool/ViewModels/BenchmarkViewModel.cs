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
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace FloatTool.ViewModels
{
	public sealed class BenchmarkViewModel : INotifyPropertyChanged
	{
		public sealed class BenchmarkResult
		{
			public string CpuName { get; set; }
			public string ThreadCount { get; set; }
			public string MultithreadedScore { get; set; }
			public string SinglethreadedScore { get; set; }
			public LinearGradientBrush FillBrush { get; set; }
			public GridLength FillSize { get; set; }
			public GridLength EmptySize { get; set; }
		}
		public long TotalCombinations { get; set; }

		private int progressPercentage;
		private bool buttonsEnabled = true;
		private bool canPublish = false;
		private bool showOnlyCurrent = false;
		private int multithreadedSpeed = 0;
		private int threadCount = Environment.ProcessorCount;
		public int ThreadCountTested = 0;
		private int singlethreadedSpeed = 0;
		private bool isUpdatingEnabled;

		private static readonly LinearGradientBrush AMDBrush = Application.Current.Resources["AmdBenchmarkFill"] as LinearGradientBrush;
		private static readonly LinearGradientBrush IntelBrush = Application.Current.Resources["IntelBenchmarkFill"] as LinearGradientBrush;
		private static readonly LinearGradientBrush CurrentBrush = Application.Current.Resources["CurrentBenchmarkFill"] as LinearGradientBrush;

		public int ProgressPercentage
		{
			get { return progressPercentage; }
			set
			{
				progressPercentage = value;
				OnPropertyChanged();
			}
		}

		public bool ButtonsEnabled
		{
			get { return buttonsEnabled; }
			set
			{
				buttonsEnabled = value;
				OnPropertyChanged();
			}
		}
		public bool CanPublish
		{
			get { return canPublish; }
			set
			{
				canPublish = value;
				OnPropertyChanged();
			}
		}

		public int ThreadCount
		{
			get
			{
				return threadCount;
			}
			set
			{
				threadCount = value;
				OnPropertyChanged();
			}
		}
		public bool ShowOnlyCurrent
		{
			get
			{
				return showOnlyCurrent;
			}
			set
			{
				showOnlyCurrent = value;
				PollBenchmarkResults();
				OnPropertyChanged();
			}
		}

		public int MultithreadedSpeed
		{
			get { return multithreadedSpeed; }
			set
			{
				multithreadedSpeed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MultithreadedSpeedText)));
			}
		}
		public string MultithreadedSpeedText { get { return $"{multithreadedSpeed:n0}"; } }

		public int SinglethreadedSpeed
		{
			get { return singlethreadedSpeed; }
			set
			{
				singlethreadedSpeed = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SinglethreadedSpeedText)));
			}
		}
		public string SinglethreadedSpeedText { get { return $"{singlethreadedSpeed:n0}"; } }

		public ObservableCollection<BenchmarkResult> BenchmarkResults { get; set; }
		public string CurrentCpuName { get; set; }
		public static string CurrentCpuThreads { get { return $"{Environment.ProcessorCount}"; } }

		public bool IsUpdatingEnabled
		{
			get { return isUpdatingEnabled; }
			set { isUpdatingEnabled = value; OnPropertyChanged(); }
		}

		public async void PollBenchmarkResults()
		{
			var url = Utils.API_URL + "/load";
			if (ShowOnlyCurrent) url += $"?version={AppHelpers.VersionCode}";
			Logger.Log.Info($"Getting benchmark results from {url}");
			IsUpdatingEnabled = false;
			try
			{
				BenchmarkResults.Clear();
				using var client = new HttpClient();

				HttpResponseMessage response = await client.GetAsync(url);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();
				dynamic result = JsonConvert.DeserializeObject(responseBody);

				if (result["status"] == 200 && result["count"] > 0)
				{
					float maxspeed = result.items[0].multithread;

					foreach (var benchmark in result.items)
					{
						float percentage = (float)benchmark.multithread / maxspeed;
						float reverse = 1 - percentage;

						string cpuName = Utils.ShortCpuName((string)benchmark.name);
						var currentFill = AMDBrush;
						if (cpuName == CurrentCpuName)
						{
							currentFill = CurrentBrush;
							MultithreadedSpeed = Math.Max((int)benchmark.multithread, MultithreadedSpeed);
							SinglethreadedSpeed = Math.Max((int)benchmark.singlethread, SinglethreadedSpeed);
						}
						else if (cpuName.StartsWith("Intel"))
							currentFill = IntelBrush;

						BenchmarkResults.Add(new BenchmarkResult
						{
							CpuName = cpuName,
							ThreadCount = Utils.EscapeLocalization($"{benchmark.threads} {((int)benchmark.threads == 1 ? "%m_Thread%" : "%m_Threads%")} [{benchmark.version}]"),
							MultithreadedScore = $"{(int)benchmark.multithread:n0}",
							SinglethreadedScore = $"{(int)benchmark.singlethread:n0}",
							FillSize = new GridLength(percentage, GridUnitType.Star),
							EmptySize = new GridLength(reverse, GridUnitType.Star),
							FillBrush = currentFill
						});
					}
				}

				Logger.Log.Info($"Benchmark results loaded: {BenchmarkResults.Count} items");

				if (BenchmarkResults.Count == 0)
					throw new Exception("0 results");
			}
			catch (Exception ex)
			{
				BenchmarkResults.Add(new BenchmarkResult
				{
					CpuName = "Error loading benchmark table: " + ex.Message,
					FillSize = new GridLength(0, GridUnitType.Star),
					EmptySize = new GridLength(1, GridUnitType.Star),
				});
				Logger.Log.Error("Error getting benchmark results", ex);
			}
			IsUpdatingEnabled = true;
		}

		public async void PushBenchmarkResults()
		{
			Logger.Log.Info("Sending benchmark result");
			try
			{
				BenchmarkResults.Clear();
				using var client = new HttpClient();
				var version = Assembly.GetExecutingAssembly().GetName().Version;
				client.DefaultRequestHeaders.Add("User-Agent", $"FloatTool/{AppHelpers.VersionCode}");
				string paramedURL = $"/submit?cpu={CurrentCpuName}&threads={ThreadCountTested}&multicore={MultithreadedSpeed}&singlecore={SinglethreadedSpeed}";
				HttpResponseMessage response = await client.GetAsync(Utils.API_URL + paramedURL);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();

				Logger.Log.Info("Sended benchmark result");
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Error sending benchmark result", ex);
			}
			CanPublish = false;
			PollBenchmarkResults();
		}

		public BenchmarkViewModel()
		{
			ThreadCount = AppHelpers.Settings.ThreadCount;
			string path = @"HKEY_LOCAL_MACHINE\HARDWARE\DESCRIPTION\System\CentralProcessor\0";
			string cpu = (string)Registry.GetValue(path, "ProcessorNameString", "Unknown");
			CurrentCpuName = Utils.ShortCpuName(cpu);

			Logger.Log.Info($"CPU: {CurrentCpuName}");
			Logger.Log.Info($"Threads: {CurrentCpuThreads}");

			BenchmarkResults = new ObservableCollection<BenchmarkResult>();
			PollBenchmarkResults();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
