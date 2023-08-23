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

using FloatTool.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FloatTool.Common
{
	// Taken from here:
	// https://stackoverflow.com/a/46497896/16349466
	public static class StreamExtensions
	{
		public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (!source.CanRead)
				throw new ArgumentException("Has to be readable", nameof(source));
			if (destination == null)
				throw new ArgumentNullException(nameof(destination));
			if (!destination.CanWrite)
				throw new ArgumentException("Has to be writable", nameof(destination));
			if (bufferSize < 0)
				throw new ArgumentOutOfRangeException(nameof(bufferSize));

			var buffer = new byte[bufferSize];
			long totalBytesRead = 0;
			int bytesRead;
			while ((bytesRead = await source.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
			{
				await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
				totalBytesRead += bytesRead;
				progress?.Report(totalBytesRead);
			}
		}
	}

	public static class HttpClientExtensions
	{
		public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
		{
			// Get the http headers first to examine the content length
			using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
			var contentLength = response.Content.Headers.ContentLength;

			using var download = await response.Content.ReadAsStreamAsync(cancellationToken);

			// Ignore progress reporting when no progress reporter was 
			// passed or when the content length is unknown
			if (progress == null || !contentLength.HasValue)
			{
				await download.CopyToAsync(destination, cancellationToken);
				return;
			}

			// Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
			var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
			// Use extension method to report progress while downloading
			await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
			progress.Report(1);
		}
	}

	public static class StringExtensions
	{
		public static string FirstCharToUpper(this string input) =>
			input switch
			{
				null => throw new ArgumentNullException(nameof(input)),
				"" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
				_ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
			};
	}

	public static class Utils
	{
		public static string API_URL = "";
		public static string HOME_URL = "";
		private static readonly HttpClient Client = new();

		/// <summary>
		/// Fetches home URL to allow for dynamic updates
		/// </summary>
		public static void GetApiUrl()
		{
			string global_settings_url = "https://raw.githubusercontent.com/Prevter/Prevter/main/globals.json";
			string global_settings = Client.GetStringAsync(global_settings_url).Result;
			dynamic json = JsonConvert.DeserializeObject(global_settings);
			HOME_URL = json["home_url"] + "floattool";
			API_URL = json["home_url"] + "api/floattool";
		}

		public static async Task<double> GetWearFromInspectURL(string inspect_url)
		{
			string url = AppHelpers.Settings.FloatAPI switch
			{
				FloatAPI.SteamInventoryHelper => $"https://floats.steaminventoryhelper.com/?url={inspect_url}",
				_ => $"https://api.csfloat.com/?url={inspect_url}&minimal=true",
			};

			Logger.Info($"Getting float from: {url}");

			var request = new HttpRequestMessage(HttpMethod.Get, url);

			if (AppHelpers.Settings.FloatAPI == FloatAPI.CSFloat) {
				request.Version = HttpVersion.Version30;
				request.VersionPolicy = HttpVersionPolicy.RequestVersionExact;
				
				request.Headers.Add("authority", "api.csgofloat.com");
				request.Headers.Add("accept", "*/*");
				request.Headers.Add("accept-language", "uk");
				request.Headers.Add("cache-control", "no-cache");
				request.Headers.Add("origin", "chrome-extension://jjicbefpemnphinccgikpdaagjebbnhg");
				request.Headers.Add("pragma", "no-cache");
				request.Headers.Add("sec-fetch-dest", "empty");
				request.Headers.Add("sec-fetch-mode", "cors");
				request.Headers.Add("sec-fetch-site", "cross-site");
				request.Headers.Add("sec-gpc", "1");
				request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/112.0.0.0 Safari/537.36");
			}

			var result = await Client.SendAsync(request);
			string response = await result.Content.ReadAsStringAsync();

			try
			{
				result.EnsureSuccessStatusCode();
				dynamic json = JsonConvert.DeserializeObject(response);
				return Convert.ToDouble(json["iteminfo"]["floatvalue"]);
			}
			catch (Exception ex)
			{
				Logger.Log.Warn(response, ex);
				throw new Exception("Failed to fetch data from API");
			}
		}

		public static async Task<UpdateResult> CheckForUpdates()
		{
			try
			{
				HttpClient client = new();
				client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
				var result = await client.GetAsync("https://api.github.com/repos/prevter/floattool/releases/latest");
				result.EnsureSuccessStatusCode();
				string response = await result.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<UpdateResult>(response);
			}
			catch (Exception ex)
			{
				Logger.Log.Error("Failed to get latest version", ex);
				return null;
			}
		}

		public static string ShortCpuName(string cpu)
		{
			cpu = cpu.Replace("(R)", "");
			cpu = cpu.Replace("(TM)", "");
			cpu = cpu.Replace("(tm)", "");
			cpu = cpu.Replace(" with Radeon Graphics", "");
			cpu = cpu.Replace(" with Radeon Vega Mobile Gfx", "");
			cpu = cpu.Replace(" CPU", "");
			cpu = cpu.Replace(" Processor", "");

			Regex regex = new(@"( \S{1,}-Core)");
			MatchCollection matches = regex.Matches(cpu);
			if (matches.Count > 0)
				foreach (Match match in matches.Cast<Match>())
					cpu = cpu.Replace(match.Value, "");

			var index = cpu.IndexOf('@');
			if (index != -1)
				cpu = cpu[..index];

			index = cpu.IndexOf('(');
			if (index != -1)
				cpu = cpu[..index];

			return cpu.Trim();
		}

		public static string EscapeLocalization(string input)
		{
			string regex = @"%(m_[^%]{1,})%";
			var matches = Regex.Matches(input, regex);

			foreach (Match m in matches.Cast<Match>())
			{
				string key = m.Groups[1].Value;
				string localization = Application.Current.Resources[key] as string;
				input = input.Replace(m.Value, localization);
			}

			return input;
		}

		public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
		{
			var sortableList = new List<T>(collection);
			sortableList.Sort(comparison);

			for (int i = 0; i < sortableList.Count; i++)
			{
				collection.Move(collection.IndexOf(sortableList[i]), i);
			}
		}

		private const long TicksPerMillisecond = 10000;
		private const long TicksPerSecond = TicksPerMillisecond * 1000;
		private static readonly double s_tickFrequency = (double)TicksPerSecond / Stopwatch.Frequency;

		public static TimeSpan GetTimePassed(long starttime)
		{
			long endtime = Stopwatch.GetTimestamp();
			return GetTimePassed(starttime, endtime);
		}

		public static TimeSpan GetTimePassed(long starttime, long endtime)
		{
			return new TimeSpan((long)((endtime - starttime) * s_tickFrequency));
		}

	}

	public sealed class UpdateResult
	{
		public class Asset
		{
			[JsonRequired]
			[JsonProperty("browser_download_url")]
			public string BrowserDownloadUrl { get; set; }
		}

		[JsonRequired]
		[JsonProperty("tag_name")]
		public string TagName { get; set; }
		[JsonRequired]
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonRequired]
		[JsonProperty("assets")]
		public List<Asset> Assets { get; set; }
		[JsonRequired]
		[JsonProperty("body")]
		public string Body { get; set; }
	}

	public struct CraftSearchSetup
	{
		public double SearchTarget;
		public double TargetPrecision;
		public string SearchFilter;

		public Skin[] Outcomes;
		public InputSkin[] SkinPool;

		public SearchMode SearchMode;

		public int ThreadID;
		public int ThreadCount;
	}

	public readonly struct FloatRange
	{
		readonly double min;
		readonly double max;

		public FloatRange(double min, double max)
		{
			this.min = min;
			this.max = max;
		}

		public bool IsOverlapped(FloatRange other)
		{
			return Min <= other.Max && other.Min <= Max;
		}

		public double Min { get { return min; } }
		public double Max { get { return max; } }
	}

	/// <summary>
	/// Used to store current Discord Presense and update language if needed
	/// </summary>
	public sealed class RPCSettingsPersist
	{
		public string Details { get; set; }
		public string State { get; set; }

		public DiscordRPC.Timestamps Timestamp { get; set; }
		public bool ShowTime { get; set; }

		public DiscordRPC.RichPresence GetPresense()
		{
			string details = Utils.EscapeLocalization(Details);
			string state = Utils.EscapeLocalization(State);

			var rpc = new DiscordRPC.RichPresence()
			{
				Details = details,
				State = state,
				Assets = new DiscordRPC.Assets()
				{
					LargeImageKey = "icon_new",
					LargeImageText = $"FloatTool {AppHelpers.VersionCode}",
				},
			};

			if (ShowTime)
				rpc.Timestamps = Timestamp;

			return rpc;
		}
	}

}
