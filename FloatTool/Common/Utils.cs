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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FloatTool
{
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
        public const string API_URL = "https://prevterapi.000webhostapp.com";
        private static readonly HttpClient Client = new();

        public static async Task<decimal> GetWearFromInspectURL(string inspect_url)
        {
            var result = await Client.GetAsync($"https://api.csgofloat.com/?url={inspect_url}");
            result.EnsureSuccessStatusCode();
            string response = await result.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(response);
            return Convert.ToDecimal(json["iteminfo"]["floatvalue"]);
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
    }

    public class UpdateResult
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

    public class CraftSearchSetup
    {
        public decimal SearchTarget { get; set; }
        public decimal TargetPrecision { get; set; }
        public string SearchFilter { get; set; }

        public Skin[] Outcomes { get; set; }
        public InputSkin[] SkinPool { get; set; }

        public SearchMode SearchMode { get; set; }

        public int ThreadID { get; set; }
        public int ThreadCount { get; set; }
    }

    public class FloatRange
    {
        readonly float min;
        readonly float max;

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public bool IsOverlapped(FloatRange other)
        {
            return Min <= other.Max && other.Min <= Max;
        }

        public float Min { get { return min; } }
        public float Max { get { return max; } }
    }
}
