using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace FloatToolGUI
{
    public class PaletteColors
    {
        public Color Primary1 { get; set; }
        public Color Primary2 { get; set; }
        public Color Primary3 { get; set; }
        public Color Primary4 { get; set; }
        public Color Primary5 { get; set; }
        public Color Primary6 { get; set; }
        public Color Secondary1 { get; set; }
        public Color Secondary2 { get; set; }
        public Color Secondary3 { get; set; }
        public Color OverBackColor1 { get; set; }
        public Color OverBackColor2 { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        public bool IsDarkButtons { get; set; }

        public bool IsThumbnail { get; set; }
        public Image Thumbnail { get; set; }
    }

    static class Utils
    {
        #region Color palette

        //Dark Mode
        static Color darkPrimary1 = Color.FromArgb(44, 44, 44);
        static Color darkPrimary2 = Color.FromArgb(31, 31, 31);
        static Color darkPrimary3 = Color.FromArgb(37, 37, 37);
        static Color darkPrimary4 = Color.FromArgb(32, 32, 32);
        static Color darkPrimary5 = Color.FromArgb(56, 56, 56);
        static Color darkPrimary6 = Color.FromArgb(56, 56, 56);

        static Color darkSecondary1 = Color.White;
        static Color darkSecondary2 = Color.FromArgb(150, 150, 150);
        static Color darkSecondary3 = Color.Green;

        static Color darkOverBackColor1 = Color.Black;
        static Color darkOverBackColor2 = SystemColors.WindowFrame;

        //Light Mode
        static Color lightPrimary1 = Color.FromArgb(222, 222, 222);
        static Color lightPrimary2 = Color.FromArgb(255, 255, 255);
        static Color lightPrimary3 = Color.FromArgb(249, 249, 249);
        static Color lightPrimary4 = Color.FromArgb(255, 255, 255);
        static Color lightPrimary5 = Color.FromArgb(249, 249, 249);
        static Color lightPrimary6 = Color.FromArgb(200, 200, 200);

        static Color lightSecondary1 = Color.Black;
        static Color lightSecondary2 = Color.FromArgb(10, 10, 10);
        static Color lightSecondary3 = Color.FromArgb(119, 194, 119);

        static Color lightOverBackColor1 = Color.FromArgb(230, 230, 230);
        static Color lightOverBackColor2 = Color.FromArgb(240, 240, 240);

        #endregion

        public static PaletteColors CustomPalette { get; } = new PaletteColors();

        static public void UpdateCustomPalette()
        {
            try
            {
                using (StreamReader r = new StreamReader("theme.json"))
                {
                    string json = r.ReadToEnd();
                    dynamic items = JsonConvert.DeserializeObject(json);
                    var colorData = items["colors"];
                    CustomPalette.Primary1 = Color.FromArgb((int)colorData["Primary1"][0], (int)colorData["Primary1"][1], (int)colorData["Primary1"][2]);
                    CustomPalette.Primary2 = Color.FromArgb((int)colorData["Primary2"][0], (int)colorData["Primary2"][1], (int)colorData["Primary2"][2]);
                    CustomPalette.Primary3 = Color.FromArgb((int)colorData["Primary3"][0], (int)colorData["Primary3"][1], (int)colorData["Primary3"][2]);
                    CustomPalette.Primary4 = Color.FromArgb((int)colorData["Primary4"][0], (int)colorData["Primary4"][1], (int)colorData["Primary4"][2]);
                    CustomPalette.Primary5 = Color.FromArgb((int)colorData["Primary5"][0], (int)colorData["Primary5"][1], (int)colorData["Primary5"][2]);
                    CustomPalette.Primary6 = Color.FromArgb((int)colorData["Primary6"][0], (int)colorData["Primary6"][1], (int)colorData["Primary6"][2]);
                    CustomPalette.Secondary1 = Color.FromArgb((int)colorData["Secondary1"][0], (int)colorData["Secondary1"][1], (int)colorData["Secondary1"][2]);
                    CustomPalette.Secondary2 = Color.FromArgb((int)colorData["Secondary2"][0], (int)colorData["Secondary2"][1], (int)colorData["Secondary2"][2]);
                    CustomPalette.Secondary3 = Color.FromArgb((int)colorData["Secondary3"][0], (int)colorData["Secondary3"][1], (int)colorData["Secondary3"][2]);
                    CustomPalette.OverBackColor1 = Color.FromArgb((int)colorData["OverBackColor1"][0], (int)colorData["OverBackColor1"][1], (int)colorData["OverBackColor1"][2]);
                    CustomPalette.OverBackColor2 = Color.FromArgb((int)colorData["OverBackColor2"][0], (int)colorData["OverBackColor2"][1], (int)colorData["OverBackColor2"][2]);

                    CustomPalette.Name = items["name"].ToString();
                    CustomPalette.Description = items["description"].ToString();
                    CustomPalette.Author = items["author"].ToString();

                    CustomPalette.IsDarkButtons = items["darkButtons"];

                    CustomPalette.IsThumbnail = items["thumbnail"];
                    if (CustomPalette.IsThumbnail)
                    {
                        byte[] bytes = Convert.FromBase64String(items["thumbnailBase64"].ToString());

                        using (MemoryStream ms = new MemoryStream(bytes))
                            CustomPalette.Thumbnail = Image.FromStream(ms);
                    }
                    else
                    {
                        CustomPalette.Thumbnail = Properties.Resources.DarkThemePreview;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"[{DateTime.Now}]: {{EXCEPTION}} {ex.Message}{Environment.NewLine}{ex.StackTrace}");
                Logger.SaveCrashReport();
            }
        }

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

        public enum Pallete
        {
            Dark,
            Light,
            Custom
        }

        public enum PalleteColor
        {
            Primary1,
            Primary2,
            Primary3,
            Primary4,
            Primary5,
            Primary6,
            Secondary1,
            Secondary2,
            Secondary3,
            OverBackColor1,
            OverBackColor2
        }

        public static Color GetPalleteColor(Pallete pallete, PalleteColor color)
        {
            if (pallete.CompareTo(Pallete.Dark) == 0)
            {
                switch (color)
                {
                    case PalleteColor.Primary1:
                        return darkPrimary1;
                    case PalleteColor.Primary2:
                        return darkPrimary2;
                    case PalleteColor.Primary3:
                        return darkPrimary3;
                    case PalleteColor.Primary4:
                        return darkPrimary4;
                    case PalleteColor.Primary5:
                        return darkPrimary5;
                    case PalleteColor.Primary6:
                        return darkPrimary6;
                    case PalleteColor.Secondary1:
                        return darkSecondary1;
                    case PalleteColor.Secondary2:
                        return darkSecondary2;
                    case PalleteColor.Secondary3:
                        return darkSecondary3;
                    case PalleteColor.OverBackColor1:
                        return darkOverBackColor1;
                    case PalleteColor.OverBackColor2:
                        return darkOverBackColor2;
                }
            }
            else if (pallete.CompareTo(Pallete.Light) == 0)
            {
                switch (color)
                {
                    case PalleteColor.Primary1:
                        return lightPrimary1;
                    case PalleteColor.Primary2:
                        return lightPrimary2;
                    case PalleteColor.Primary3:
                        return lightPrimary3;
                    case PalleteColor.Primary4:
                        return lightPrimary4;
                    case PalleteColor.Primary5:
                        return lightPrimary5;
                    case PalleteColor.Primary6:
                        return lightPrimary6;
                    case PalleteColor.Secondary1:
                        return lightSecondary1;
                    case PalleteColor.Secondary2:
                        return lightSecondary2;
                    case PalleteColor.Secondary3:
                        return lightSecondary3;
                    case PalleteColor.OverBackColor1:
                        return lightOverBackColor1;
                    case PalleteColor.OverBackColor2:
                        return lightOverBackColor2;
                }
            }
            else
            {
                switch (color)
                {
                    case PalleteColor.Primary1:
                        return CustomPalette.Primary1;
                    case PalleteColor.Primary2:
                        return CustomPalette.Primary2;
                    case PalleteColor.Primary3:
                        return CustomPalette.Primary3;
                    case PalleteColor.Primary4:
                        return CustomPalette.Primary4;
                    case PalleteColor.Primary5:
                        return CustomPalette.Primary5;
                    case PalleteColor.Primary6:
                        return CustomPalette.Primary6;
                    case PalleteColor.Secondary1:
                        return CustomPalette.Secondary1;
                    case PalleteColor.Secondary2:
                        return CustomPalette.Secondary2;
                    case PalleteColor.Secondary3:
                        return CustomPalette.Secondary3;
                    case PalleteColor.OverBackColor1:
                        return CustomPalette.OverBackColor1;
                    case PalleteColor.OverBackColor2:
                        return CustomPalette.OverBackColor2;
                }
            }
            return Color.Black;
        }

        static public string getNextRarity(string rarity)
        {
            if (string.Compare(rarity, "Consumer") == 0) return "Industrial";
            else if (string.Compare(rarity, "Industrial") == 0) return "Mil-Spec";
            else if (string.Compare(rarity, "Mil-Spec") == 0) return "Restricted";
            else if (string.Compare(rarity, "Restricted") == 0) return "Classified";
            else if (string.Compare(rarity, "Classified") == 0) return "Covert";
            return "Nothing";
        }

        public static string CheckUpdates()
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string response = client.DownloadString("https://api.github.com/repos/prevter/FloatTool-GUI/releases/latest");
                dynamic release = JsonConvert.DeserializeObject(response);
                return release["tag_name"]+"|"+release["assets"][0]["browser_download_url"];
            }
        }

        public static decimal GetWearFromInspectURL(string url)
        {
            url = "https://api.csgofloat.com/?url=" + url;

            using (WebClient wcf = new WebClient())
            {
                wcf.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                string jsonf = wcf.DownloadString(url);
                dynamic rf = JsonConvert.DeserializeObject(jsonf);
                return Convert.ToDecimal(rf["iteminfo"]["floatvalue"]);
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
                registryData.SetValue("sound", true);
                registryData.SetValue("updateCheck", true);
                registryData.SetValue("bufferSpeed", 250);
                registryData.SetValue("discordRPC", true);
                registryData.SetValue("currency", (int)Currency.USD);
                registryData.SetValue("lastThreads", Environment.ProcessorCount);
                registryData.SetValue("theme", (int)Pallete.Dark);
                registryData.Close();
            }
            else
            {
                if (registryData.GetValue("sound") == null) registryData.SetValue("sound", true);
                if (registryData.GetValue("updateCheck") == null) registryData.SetValue("updateCheck", true);
                if (registryData.GetValue("bufferSpeed") == null) registryData.SetValue("bufferSpeed", 250);
                if (registryData.GetValue("discordRPC") == null) registryData.SetValue("discordRPC", true);
                if (registryData.GetValue("currency") == null) registryData.SetValue("currency", (int)Currency.USD);
                if (registryData.GetValue("lastThreads") == null) registryData.SetValue("lastThreads", Environment.ProcessorCount);
                if (registryData.GetValue("theme") == null) registryData.SetValue("theme", (int)Pallete.Dark); 
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
