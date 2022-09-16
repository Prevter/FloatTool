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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace FloatTool
{
    public sealed class SettingsViewModel : INotifyPropertyChanged
    {
        public Settings Settings { get; set; }

        public static List<string> ThemesList
        {
            get
            {
                var tmpList = new List<string>();
                foreach (var theme in AppHelpers.ThemesFound)
                    tmpList.Add(Path.GetFileNameWithoutExtension(theme));
                return tmpList;
            }
        }

        public int ThemeIndex
        {
            get { return AppHelpers.ThemesFound.IndexOf(Settings.ThemeURI); }
            set
            {
                Settings.ThemeURI = AppHelpers.ThemesFound[value];
                App.SelectTheme(Settings.ThemeURI);
                OnPropertyChanged();
            }
        }

        public bool EnableSound
        {
            get { return Settings.Sound; }
            set { Settings.Sound = value; OnPropertyChanged(); }
        }

        public bool CheckUpdates
        {
            get { return Settings.CheckForUpdates; }
            set { Settings.CheckForUpdates = value; OnPropertyChanged(); }
        }

        public bool DiscordRPC
        {
            get { return Settings.DiscordRPC; }
            set
            {
                Settings.DiscordRPC = value;
                OnPropertyChanged();

                // Re-enabling does not work. Probably bug in the library
                if (!value)
                    AppHelpers.DiscordClient.Deinitialize();
                else
                    AppHelpers.DiscordClient.Initialize();
            }
        }

        public int CurrentCurrencyIndex
        {
            get
            {
                return CurrencyHelper.GetIndexFromCurrency(Settings.Currency);
            }
            set
            {
                Settings.Currency = CurrencyHelper.GetCurrencyByIndex(value);
                OnPropertyChanged();
            }
        }

        public List<string> CurrencyNames { get; private set; } = new List<string>
        {
            "USD / United States Dollar",
            "GBP / United Kingdom Pound",
            "EUR / European Union Euro",
            "CHF / Swiss Francs",
            "RUB / Russian Rouble",
            "PLN / Polish Złoty",
            "BRL / Brazilian Reals",
            "JPY / Japanese Yen",
            "NOK / Norwegian Krone",
            "IDR / Indonesian Rupiah",
            "MYR / Malaysian Ringgit",
            "PHP / Philippine Peso",
            "SGD / Singapore Dollar",
            "THB / Thai Baht",
            "VND / Vietnamese Dong",
            "KRW / South Korean Won",
            "TRY / Turkish Lira",
            "UAH / Ukrainian Hryvnia",
            "MXN / Mexican Peso",
            "CAD / Canadian Dollar",
            "AUD / Australian Dollar",
            "NZD / New Zealand Dollar",
            "CNY / Chinese Yuan",
            "INR / Indian Rupee",
            "CLP / Chilean Peso",
            "PEN / Peruvian Nuevo Sol",
            "COP / Colombian Peso",
            "ZAR / South African Rand",
            "HKD / Hong Kong Dollar",
            "TWD / Taiwan Dollar",
            "SAR / Saudi Riyal",
            "AED / United Arab Emirates Dirham",
            "ARS / Argentine Peso",
            "ILS / Israeli New Shekel",
            "BYN / Belarusian Ruble",
            "KZT / Kazakhstani Tenge",
            "KWD / Kuwaiti Dinar",
            "QAR / Qatari Riyal",
            "CRC / Costa Rican Colón",
            "UYU / Uruguayan Peso",
            "RMB / Chinese Yuan",
            "NXP / NXP",
        };

        public int CurrentLanguage
        {
            get
            {
                return LanguageCodes.IndexOf(Settings.LanguageCode);
            }
            set
            {
                Settings.LanguageCode = LanguageCodes[value];
                App.SelectCulture(Settings.LanguageCode);
                OnPropertyChanged();
            }
        }

        public static List<string> Languages { get; private set; }
        public readonly static List<string> LanguageCodes = new()
        {
            "cs",
            "da",
            "de",
            "en",
            "es",
            "fi",
            "fr",
            "ga",
            "he",
            "hr",
            "it",
            "ja",
            "ka",
            "lt",
            "lv",
            "pl",
            "pt",
            "uk",
            "tr",
            "ru",
            "zh",
        };

        public SettingsViewModel(Settings settings)
        {
            Settings = settings;
            Languages = new List<string>();

            foreach (var lang in LanguageCodes)
            {
                var locale = new System.Globalization.CultureInfo(lang);
                Languages.Add($"{locale.NativeName.FirstCharToUpper()} ({locale.EnglishName})");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
