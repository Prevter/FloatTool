using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace FloatToolGUI
{
    static class Utils
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

        static public string getNextRarity(string rarity)
        {
            if (rarity == "Consumer") return "Industrial";
            else if (rarity == "Industrial") return "Mil-Spec";
            else if (rarity == "Mil-Spec") return "Restricted";
            else if (rarity == "Restricted") return "Classified";
            else if (rarity == "Classified") return "Covert";
            return "Nothing";
        }

        public static string CheckUpdates()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                using (var response = client.GetAsync("https://api.github.com/repos/prevter/FloatTool-GUI/releases/latest").Result)
                {
                    var json = response.Content.ReadAsStringAsync().Result;

                    dynamic release = JsonConvert.DeserializeObject(json);
                    return release["tag_name"];
                }
            }
        }

        public static bool testOverlap(float x1, float x2, float y1, float y2)
        {
            return (x1 >= y1 && x1 <= y2) ||
                   (x2 >= y1 && x2 <= y2) ||
                   (y1 >= x1 && y1 <= x2) ||
                   (y2 >= x1 && y2 <= x2);
        }

        public static void CheckRegistry()
        {
            var registryData = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\FloatTool", true);
            if (registryData == null)
            {
                registryData = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\FloatTool");
                registryData.SetValue("darkMode", true);
                registryData.SetValue("sound", true);
                registryData.SetValue("updateCheck", true);
                registryData.SetValue("bufferSpeed", 250);
                registryData.SetValue("discordRPC", true);
                registryData.SetValue("currency", (int)Currency.USD);
                registryData.Close();
            }
            else
            {
                if(registryData.GetValue("darkMode") == null) registryData.SetValue("darkMode", true);
                if (registryData.GetValue("sound") == null) registryData.SetValue("sound", true);
                if (registryData.GetValue("updateCheck") == null) registryData.SetValue("updateCheck", true);
                if (registryData.GetValue("bufferSpeed") == null) registryData.SetValue("bufferSpeed", 250);
                if (registryData.GetValue("discordRPC") == null) registryData.SetValue("discordRPC", true);
                if (registryData.GetValue("currency") == null) registryData.SetValue("currency", (int)Currency.USD);
            }
        }

        static private PrivateFontCollection _privateFontCollection = new PrivateFontCollection();

        static public FontFamily GetFontFamilyByName(string name)
        {
            return _privateFontCollection.Families.FirstOrDefault(x => x.Name == name);
        }

        static public void AddFont(string fullFileName)
        {
            AddFont(File.ReadAllBytes(fullFileName));
        }

        static public void AddFont(byte[] fontBytes)
        {
            var handle = GCHandle.Alloc(fontBytes, GCHandleType.Pinned);
            IntPtr pointer = handle.AddrOfPinnedObject();
            try
            {
                _privateFontCollection.AddMemoryFont(pointer, fontBytes.Length);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
