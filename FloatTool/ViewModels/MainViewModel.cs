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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FloatTool
{
    public enum SearchMode
    {
        Less, Equal, Greater
    }

    public class Combination
    {
        public decimal Wear { get; set; }
        public string IEEE754 { get; set; }
        public InputSkin[] Inputs { get; set; }
        public string OutcomeName { get; set; }
        public Currency Currency { get; set; }
        public float Price { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private bool isSearching;

        private bool isStatTrack;
        private string weaponName;
        private string skinName;
        private string skinQuality;
        private string fullSkinName;
        private int outcomeIndex;
        private string floatRange;

        private SearchMode searchMode;
        private string searchFilter;
        private int skinCount;
        private int skinSkipCount;
        private Visibility isError = Visibility.Hidden;
        private bool sort;
        private bool sortDescending;
        private float progressPercentage;
        private string currentSpeed = "0";

        private string consoleOutput;

        private readonly List<string> qualityList = new()
        {
            "Factory New",
            "Minimal Wear",
            "Field-Tested",
            "Well-Worn",
            "Battle-Scarred"
        };

        private readonly List<string> weaponList = new()
        {
            "AK-47",
            "AUG",
            "AWP",
            "Desert Eagle",
            "Dual Berettas",
            "FAMAS",
            "Five-SeveN",
            "G3SG1",
            "Galil AR",
            "Glock-18",
            "M249",
            "M4A1-S",
            "M4A4",
            "MAC-10",
            "MAG-7",
            "MP5-SD",
            "MP7",
            "MP9",
            "Negev",
            "Nova",
            "P2000",
            "P90",
            "PP-Bizon",
            "R8 Revolver",
            "Sawed-Off",
            "SCAR-20",
            "SG 553",
            "UMP-45",
            "XM1014"
        };

        private List<string> skinList = new();
        private List<string> outcomeList = new();

        public Dictionary<Tuple<float, float>, List<Skin>> Outcomes = new();

        public List<SkinModel> SkinsDatabase;
        public Settings Settings { get; set; }

        #region Properties

        public ObservableCollection<Combination> FoundCombinations { get; set; }

        public static string CurrentVersionSubtitle
        {
            get { return $"{App.VersionCode} by Prevter"; }
        }

        public int ThreadCount
        {
            get { return Settings.ThreadCount; }
            set { Settings.ThreadCount = value; OnPropertyChanged(); }
        }

        public string CombinationsLabel
        {
            get { return $"{ParsedCombinations}/{TotalCombinations}"; }
            set { OnPropertyChanged(); }
        }
            
        public string CurrentSpeedLabel
        {
            get { return currentSpeed; }
            set { currentSpeed = value; OnPropertyChanged(); }
        }

        public string ConsoleOutput
        {
            get { return consoleOutput; }
            set { consoleOutput = value; OnPropertyChanged(); }
        }

        public string SearchFilter
        {
            get { return searchFilter; }
            set { 
                searchFilter = value;
                OnPropertyChanged(); 
            }
        }

        public int SkinCount
        {
            get { return skinCount; }
            set { 
                skinCount = value; 
                OnPropertyChanged(); 
            }
        }

        public int SkinSkipCount
        {
            get { return skinSkipCount; }
            set { 
                skinSkipCount = value; 
                OnPropertyChanged(); 
            }
        }

        public bool Sort
        {
            get { return sort; }
            set { sort = value; OnPropertyChanged(); }
        }

        public bool SortDescending
        {
            get { return sortDescending; }
            set { sortDescending = value; OnPropertyChanged(); }
        }

        public SearchMode SearchModeSelected
        {
            get { return searchMode; }
            set { searchMode = value; OnPropertyChanged(); }
        }

        public string FloatRange
        {
            get { return floatRange; }
            set { floatRange = value; OnPropertyChanged(); }
        }

        public int OutcomeIndex
        {
            get { return outcomeIndex; }
            set
            {
                outcomeIndex = value;
                UpdateFloatRange();
                OnPropertyChanged();
            }
        }

        public bool CanEditSettings
        {
            get { return !isSearching; }
            set { isSearching = !value; OnPropertyChanged(); }
        }

        public string WeaponName
        {
            get { return weaponName; }
            set
            {
                weaponName = value;
                UpdateFullSkinName();
                UpdateOutcomes();
                UpdateSkinList();
                OnPropertyChanged();
            }
        }

        public string SkinName
        {
            get { return skinName; }
            set
            {
                skinName = value;
                UpdateFullSkinName();
                UpdateOutcomes();
                OnPropertyChanged();
            }
        }

        public string SkinQuality
        {
            get { return skinQuality; }
            set
            {
                skinQuality = value;
                UpdateFullSkinName();
                UpdateFloatRange();
                OnPropertyChanged();
            }
        }

        public bool IsStatTrack
        {
            get { return isStatTrack; }
            set
            {
                isStatTrack = value;
                UpdateFullSkinName();
                OnPropertyChanged();
            }
        }

        public string FullSkinName
        {
            get { return fullSkinName; }
            set { fullSkinName = value; OnPropertyChanged(); }
        }

        public List<string> WeaponList { get { return weaponList; } }

        public List<string> SkinList
        {
            get { return skinList; }
            set { skinList = value; OnPropertyChanged(); }
        }

        public List<string> QualityList { get { return qualityList; } }

        public List<string> OutcomeList
        {
            get { return outcomeList; }
            set { outcomeList = value; OnPropertyChanged(); }
        }

        public Visibility IsError
        {
            get { return isError; }
            set { isError = value; OnPropertyChanged(); }
        }

        public float ProgressPercentage {
            get { return progressPercentage; }
            set { progressPercentage = value; OnPropertyChanged(); }
        }

        public long ParsedCombinations { get; internal set; }
        public long TotalCombinations { get; internal set; }

        #endregion

        private void UpdateFullSkinName()
        {
            FullSkinName = $"{(IsStatTrack ? "StatTrak™ " : "")}{WeaponName} | {SkinName} ({SkinQuality})";

            if (SkinsDatabase is null)
                return;

            foreach (var skin in SkinsDatabase)
            {
                if (skin.Name == $"{WeaponName} | {SkinName}")
                {
                    IsError = skin.IsQualityInRange(SkinQuality) ? Visibility.Hidden : Visibility.Visible;
                    break;
                }
            }
        }

        private float minCraftWear, maxCraftWear;

        private void UpdateFloatRange()
        {
            if (OutcomeIndex > Outcomes.Count - 1)
            {
                FloatRange = "No data";
                return;
            }

            var range = Skin.GetFloatRangeForQuality(SkinQuality);

            List<InputSkin> lowest = new();
            for (int i = 0; i < 10; i++)
                lowest.Add(new InputSkin(range.Min, 0, Currency.USD));

            List<InputSkin> highest = new();
            for (int i = 0; i < 10; i++)
                highest.Add(new InputSkin(range.Max, 0, Currency.USD));

            int index = 0;
            minCraftWear = 0;
            maxCraftWear = 0;
            foreach (var outcome in Outcomes.Values)
            {
                if (index++ == OutcomeIndex)
                {
                    var currSkin = outcome[0];
                    minCraftWear = Convert.ToSingle(
                        Calculations.Craft(
                            lowest.ToArray(),
                            currSkin.MinFloat,
                            currSkin.FloatRange
                        ),
                        CultureInfo.InvariantCulture
                    );
                    maxCraftWear = Convert.ToSingle(
                        Calculations.Craft(
                            highest.ToArray(),
                            currSkin.MinFloat,
                            currSkin.FloatRange
                        ),
                        CultureInfo.InvariantCulture
                    );
                    break;
                }
            }

            FloatRange = $"{minCraftWear.ToString("0.00", CultureInfo.InvariantCulture)} - {maxCraftWear.ToString("0.00", CultureInfo.InvariantCulture)}";
        }

        public void UpdateOutcomes()
        {
            if (SkinsDatabase is null)
                return;

            string skin = $"{WeaponName} | {SkinName}";
            var skinlist = new List<SkinModel>();

            for (int i = 0; i < SkinsDatabase.Count; i++)
            {
                if (SkinsDatabase[i].Name == skin)
                {
                    for (int j = 0; j < SkinsDatabase.Count; j++)
                    {
                        if (SkinsDatabase[i].Case == SkinsDatabase[j].Case &&
                            SkinsDatabase[j].Rarity == Skin.NextRarity(SkinsDatabase[i].Rarity))
                        {
                            skinlist.Add(SkinsDatabase[j]);
                        }
                    }
                }
            }

            Outcomes.Clear();

            for (int i = 0; i < skinlist.Count; i++)
            {
                var range = new Tuple<float, float>(skinlist[i].MinWear, skinlist[i].MaxWear);
                if (Outcomes.ContainsKey(range))
                    Outcomes[range].Add(new Skin(skinlist[i]));
                else
                    Outcomes[range] = new List<Skin> { new Skin(skinlist[i]) };
            }

            int totalSkins = 0;
            foreach (var skinRange in Outcomes.Values)
                totalSkins += skinRange.Count;

            var list = new List<string>();

            foreach (var skinRange in Outcomes.Values)
            {
                string tmp = (skinRange.Count > 1) ? $" + {(skinRange.Count - 1)}" : "";
                list.Add($"{((float)skinRange.Count) / totalSkins * 100}% ({skinRange[0].Name}{tmp})");
            }

            list.Add("* Search all *");
            OutcomeIndex = 0;
            OutcomeList = list;
        }

        public void UpdateSkinList()
        {
            if (SkinsDatabase is null)
                return;

            var list = new List<string>();

            for (int i = 0; i < SkinsDatabase.Count; ++i)
            {
                if (SkinsDatabase[i].Name.StartsWith(WeaponName) && !SkinsDatabase[i].IsHighest)
                    list.Add(SkinsDatabase[i].Name.Split(" | ")[1]);
            }

            list.Sort();
            SkinList = list;
        }

        public MainViewModel(string weapon, string skin, string quality, string filter, int count, int skip, Settings settings)
        {
            Settings = settings;
            WeaponName = weapon;
            SkinName = skin;
            SkinQuality = quality;

            SearchModeSelected = SearchMode.Equal;
            SearchFilter = filter;
            SkinCount = count;
            SkinSkipCount = skip;

            FoundCombinations = new();

            ConsoleOutput = "Welcome to FloatTool!\nTool for creating floats with CS:GO contracts.\n";

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FloatTool.Assets.SkinList.json")!;
            if (stream is null) throw new NullReferenceException("Could not find SkinList.json");

            using (StreamReader reader = new(stream))
            {
                var jsonFileContent = reader.ReadToEnd();
                SkinsDatabase = JsonConvert.DeserializeObject<List<SkinModel>>(jsonFileContent)!;
            }

            UpdateSkinList();
            UpdateOutcomes();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
