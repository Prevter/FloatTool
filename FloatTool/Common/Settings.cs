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

using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace FloatTool
{
    public enum Currency
    {
        USD = 1, GBP = 2,
        EUR = 3, CHF = 4,
        RUB = 5, PLN = 6,
        BRL = 7, JPY = 8,
        NOK = 9, IDR = 10,
        MYR = 11, PHP = 12,
        SGD = 13, THB = 14,
        VND = 15, KRW = 16,
        TRY = 17, UAH = 18,
        MXN = 19, CAD = 20,
        AUD = 21, NZD = 22,
        CNY = 23, INR = 24,
        CLP = 25, PEN = 26,
        COP = 27, ZAR = 28,
        HKD = 29, TWD = 30,
        SAR = 31, AED = 32,
        ARS = 34, ILS = 35,
        BYN = 36, KZT = 37,
        KWD = 38, QAR = 39,
        CRC = 40, UYU = 41,
        RMB = 9000, NXP = 9001
    }
    
    // These are currently the same, but that can change later.
    public enum FloatAPI
    {
        CSGOFloat,
        SteamInventoryHelper
    }

    public enum ExtensionType
    {
        CSGOFloat,
        SteamInventoryHelper
    }

    public static class CurrencyHelper
    {
        //Method to get the currency name from the enum
        public static Currency GetCurrencyByIndex(int index)
        {
            return (Currency)(Enum.GetValues(typeof(Currency))).GetValue(index);
        }

        //Method to get index of a currency
        public static int GetIndexFromCurrency(Currency currency)
        {
            return Array.IndexOf(Enum.GetValues(typeof(Currency)), currency);
        }
    }

    public sealed class Settings
    {
        public string LanguageCode { get; set; } = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        public Currency Currency { get; set; } = Currency.USD;
        public string ThemeURI { get; set; } = "/Theme/Schemes/Dark.xaml";
        public bool Sound { get; set; } = true;
        public bool CheckForUpdates { get; set; } = true;
        public bool DiscordRPC { get; set; } = true;
        public int ThreadCount { get; set; } = Environment.ProcessorCount;
        public FloatAPI FloatAPI { get; set; } = FloatAPI.CSGOFloat;
        public ExtensionType ExtensionType { get; set; } = ExtensionType.CSGOFloat;

        public void Load()
        {
            try
            {
                string settingsPath = Path.Join(AppHelpers.AppDirectory, "settings.json");
                if (File.Exists(settingsPath))
                {
                    var settings = File.ReadAllText(settingsPath);
                    var tmpSettings = JsonConvert.DeserializeObject<Settings>(settings);
                    LanguageCode = tmpSettings.LanguageCode;
                    Currency = tmpSettings.Currency;
                    ThemeURI = tmpSettings.ThemeURI;
                    Sound = tmpSettings.Sound;
                    CheckForUpdates = tmpSettings.CheckForUpdates;
                    DiscordRPC = tmpSettings.DiscordRPC;
                    ThreadCount = tmpSettings.ThreadCount;
                    FloatAPI = tmpSettings.FloatAPI;
                    ExtensionType = tmpSettings.ExtensionType;
                }
                else
                {
                    LoadOld();
                    Save();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error loading settings", ex);
            }
        }

        public void LoadOld()
        {
            // Load settings from registry
            const string userRoot = "HKEY_CURRENT_USER";
            const string subkey = "Software\\FloatTool";
            const string keyName = userRoot + "\\" + subkey;

            try
            {
                using var hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                var key = hkcu.OpenSubKey(@"SOFTWARE\FloatTool");

                if (key == null) return;

                LanguageCode = (string)Registry.GetValue(keyName, "languageCode", LanguageCode);
                Currency = (Currency)Registry.GetValue(keyName, "currency", Currency);
                ThemeURI = (string)Registry.GetValue(keyName, "themeURI", ThemeURI);
                ThreadCount = (int)Registry.GetValue(keyName, "lastThreads", ThreadCount);
                Sound = (string)Registry.GetValue(keyName, "sound", Sound ? "True" : "False") == "True";
                CheckForUpdates = (string)Registry.GetValue(keyName, "updateCheck", CheckForUpdates ? "True" : "False") == "True";
                DiscordRPC = (string)Registry.GetValue(keyName, "discordRPC", DiscordRPC ? "True" : "False") == "True";

                key.Close();
                Logger.Log.Info("Loaded settings from registry");

                MigrateFromOldVersion();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error loading settings from registry", ex);
            }
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(AppHelpers.AppDirectory))
                    Directory.CreateDirectory(AppHelpers.AppDirectory);

                var settings = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(Path.Join(AppHelpers.AppDirectory, "settings.json"), settings);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error saving settings", ex);
            }
        }

        public void MigrateFromOldVersion()
        {
            // This method cleans up old settings and data
            try
            {
                RegistryKey regkeySoftware = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                regkeySoftware.DeleteSubKeyTree("FloatTool");
                regkeySoftware.Close();
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Error saving settings", ex);
            }

            List<string> oldFiles = new()
            {
                "debug.log",
                "FloatCore.dll",
                "FloatTool.exe.config",
                "FloatTool.pdb",
                "itemData.json",
                "theme.json",
                "Updater.exe"
            };

            foreach (var file in oldFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }

            App.CleanOldFiles();
            Save();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
