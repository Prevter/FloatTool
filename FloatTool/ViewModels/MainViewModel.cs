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
using System.Windows.Controls;

namespace FloatTool
{
    public enum SearchMode
    {
        Less, Equal, Greater
    }

    public sealed class Combination
    {
        public double Wear { get; set; }
        public string Wear32Bit { get; set; }
        public string Wear128Bit { get; set; }
        public InputSkin[] Inputs { get; set; }
        public string OutcomeName { get; set; }
        public Currency Currency { get; set; }
        public float Price { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
    }

    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private bool isSearching;

        private bool isStatTrak;
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
        private Visibility isErrorFloat = Visibility.Hidden;
        private readonly Viewbox errorMessage;
        private readonly Viewbox errorMessageFloat;
        private bool sort;
        private bool sortDescending;
        private float progressPercentage;
        private string currentSpeed = "0";

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
            "CZ75-Auto",
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
            "P250",
            "P90",
            "PP-Bizon",
            "R8 Revolver",
            "SCAR-20",
            "SG 553",
            "SSG 08",
            "Sawed-Off",
            "Tec-9",
            "UMP-45",
            "USP-S",
            "XM1014"
        };

        private List<string> skinList = new();
        private List<string> outcomeList = new();
        public Dictionary<Tuple<double, double>, List<Skin>> Outcomes = new();

        public List<Collection> SkinsDatabase;
        
        #region Properties

        public ObservableCollection<Combination> FoundCombinations { get; set; }

        public static string CurrentVersionSubtitle
        {
            get { return $"{AppHelpers.VersionCode} by Prevter"; }
        }

        public int ThreadCount
        {
            get { return AppHelpers.Settings.ThreadCount; }
            set { AppHelpers.Settings.ThreadCount = value; OnPropertyChanged(); }
        }

        public string CombinationsLabel
        {
            get { return $"{ParsedCombinations:n0}/{TotalCombinations}"; }
            set { OnPropertyChanged(); }
        }

        public string CurrentSpeedLabel
        {
            get { return currentSpeed; }
            set { currentSpeed = value; OnPropertyChanged(); }
        }

        public string SearchFilter
        {
            get { return searchFilter; }
            set
            {
                searchFilter = value;
                UpdateFloatError();
                OnPropertyChanged();
            }
        }

        public int SkinCount
        {
            get { return skinCount; }
            set
            {
                skinCount = value;
                OnPropertyChanged();
            }
        }

        public int SkinSkipCount
        {
            get { return skinSkipCount; }
            set
            {
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

        public bool IsStatTrak
        {
            get { return isStatTrak; }
            set
            {
                isStatTrak = value;
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

        public Visibility IsErrorFloat
        {
            get { return isErrorFloat; }
            set { isErrorFloat = value; OnPropertyChanged(); }
        }

        public float ProgressPercentage
        {
            get { return progressPercentage; }
            set { progressPercentage = value; OnPropertyChanged(); }
        }

        public long ParsedCombinations { get; internal set; }
        public long TotalCombinations { get; internal set; }

        private RelayCommand copyCommand;
        public RelayCommand CopyCommand
        {
            get
            {
                return copyCommand ??= new RelayCommand(field =>
                {
                    var textbox = field as TextBox;
                    Clipboard.SetText(textbox.Text);
                    textbox?.Focus();
                    textbox?.SelectAll();
                });
            }
        }

        #endregion

        private void UpdateFullSkinName()
        {
            FullSkinName = $"{(IsStatTrak ? "StatTrak™ " : "")}{WeaponName} | {SkinName} ({SkinQuality})";

            if (SkinsDatabase is null)
                return;

            foreach (var collection in SkinsDatabase)
            {
                foreach (var skin in collection.Skins)
                {
                    if (skin.Name == $"{WeaponName} | {SkinName}")
                    {
                        if (!skin.IsQualityInRange(SkinQuality))
                        {
                            errorMessage.SetResourceReference(Viewbox.ToolTipProperty, "m_SkinNotFound");
                            IsError = Visibility.Visible;
                        }
                        else if (IsStatTrak && !collection.CanBeStattrak)
                        {
                            errorMessage.SetResourceReference(Viewbox.ToolTipProperty, "m_CantBeStattrak");
                            IsError = Visibility.Visible;
                        }
                        else
                        {
                            IsError = Visibility.Hidden;
                        }
                    }
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

            UpdateFloatError();
            FloatRange = $"{minCraftWear.ToString("0.00", CultureInfo.InvariantCulture)} - {maxCraftWear.ToString("0.00", CultureInfo.InvariantCulture)}";
        }

        public void UpdateFloatError()
        {
            try
            {
                decimal searchFilterDecimal = decimal.Parse(searchFilter, CultureInfo.InvariantCulture);
                if (searchFilterDecimal < (decimal)minCraftWear || searchFilterDecimal > (decimal)maxCraftWear)
                {
                    IsErrorFloat = Visibility.Visible;
                    errorMessageFloat.SetResourceReference(Viewbox.ToolTipProperty, "m_OutOfBounds");
                }
                else
                {
                    IsErrorFloat = Visibility.Hidden;
                }
            }
            catch (FormatException)
            {
                IsErrorFloat = Visibility.Visible;
                errorMessageFloat.SetResourceReference(Viewbox.ToolTipProperty, "m_CantParse");
            }
        }


        public Collection FindSkinCollection(string skin)
        {
            if (SkinsDatabase is null)
                return null;

            foreach (var collection in SkinsDatabase)
            {
                foreach (var skinObj in collection.Skins)
                {
                    if (skinObj.Name == skin)
                    {
                        return collection;
                    }

                }
            }

            return null;
        }

        public void UpdateOutcomes()
        {
            if (SkinsDatabase is null)
                return;

            string skin = $"{WeaponName} | {SkinName}";
            var skinlist = new List<SkinModel>();

            var collection = FindSkinCollection(skin);
            if (collection is null)
                return;

            foreach (var skinObj in collection.Skins)
            {
                if (skinObj.Name == skin)
                {
                    foreach (var otherSkinModel in collection.Skins)
                    {
                        if (Skin.NextRarity(skinObj.Rarity) == otherSkinModel.Rarity)
                        {
                            skinlist.Add(otherSkinModel);
                        }
                    }
                    break;
                }
            }
            Outcomes.Clear();

            for (int i = 0; i < skinlist.Count; i++)
            {
                var range = new Tuple<double, double>(skinlist[i].MinWear, skinlist[i].MaxWear);
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

            foreach (var collection in SkinsDatabase)
            {
                foreach (var skin in collection.Skins)
                {
                    if (skin.Name.Contains(WeaponName) && collection.HighestRarity != skin.Rarity)
                    {
                        list.Add(skin.Name.Split(" | ")[1]);
                    }
                }
            }

            list.Sort();
            SkinList = list;
        }

        public MainViewModel(string weapon, string skin, string quality, string filter, int count, int skip, Viewbox errorTooltip, Viewbox errorTooltipFloat)
        {
            errorMessage = errorTooltip;
            errorMessageFloat = errorTooltipFloat;
            WeaponName = weapon;
            SkinName = skin;
            SkinQuality = quality;

            SearchModeSelected = SearchMode.Equal;
            SearchFilter = filter;
            SkinCount = count;
            SkinSkipCount = skip;

            FoundCombinations = new();

            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FloatTool.Assets.SkinList.json")!;
            if (stream is null)
                throw new NullReferenceException("Could not find SkinList.json");

            using (StreamReader reader = new(stream))
            {
                var jsonFileContent = reader.ReadToEnd();
                SkinsDatabase = JsonConvert.DeserializeObject<List<Collection>>(jsonFileContent)!;
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
