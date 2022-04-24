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
using System.Globalization;
using System.Windows;

namespace FloatTool
{
    public class Collection
    {
        public string Name { get; set; }
        public bool CanBeStattrak { get; set; }
        public string LowestRarity { get; set; }
        public string HighestRarity { get; set; }
        public List<SkinModel> Skins { get; set; }
    }

    public class SkinModel
    {
        public string Name { get; set; }
        public string Rarity { get; set; }
        public float MinWear { get; set; }
        public float MaxWear { get; set; }

        public bool IsQualityInRange(string quality)
        {
            var range = Skin.GetFloatRangeForQuality(quality);
            return new FloatRange(MinWear, MaxWear).IsOverlapped(range);
        }
    }

    public class Skin
    {
        public enum Quality
        {
            Consumer,
            Industrial,
            MilSpec,
            Restricted,
            Classified,
            Covert
        }

        public string Name { get; set; }
        public decimal MinFloat { get; set; }
        public decimal MaxFloat { get; set; }
        public decimal FloatRange { get; set; }
        public Quality Rarity { get; set; }

        public Skin(string name, float minWear, float maxWear, Quality rarity)
        {
            Name = name;
            MinFloat = (decimal)minWear;
            MaxFloat = (decimal)maxWear;
            FloatRange = (MaxFloat - MinFloat) / 10;
            Rarity = rarity;
        }

        public Skin(SkinModel model) : this(model.Name, model.MinWear, model.MaxWear, FromString(model.Rarity)) { }

        public static string NextRarity(string rarity)
        {
            return rarity switch
            {
                "Consumer" => "Industrial",
                "Industrial" => "Mil-Spec",
                "Mil-Spec" => "Restricted",
                "Restricted" => "Classified",
                "Classified" => "Covert",
                _ => "Nothing",
            };
        }

        public static Quality FromString(string value)
        {
            return value switch
            {
                "Consumer" => Quality.Consumer,
                "Industrial" => Quality.Industrial,
                "Mil-Spec" => Quality.MilSpec,
                "Restricted" => Quality.Restricted,
                "Classified" => Quality.Classified,
                _ => Quality.Covert,
            };
        }

        public static FloatRange GetFloatRangeForQuality(string quality)
        {
            float lowestWear;
            float highestWear;

            switch (quality)
            {
                case "Factory New":
                    lowestWear = 0f;
                    highestWear = 0.07f;
                    break;
                case "Minimal Wear":
                    lowestWear = 0.07f;
                    highestWear = 0.15f;
                    break;
                case "Field-Tested":
                    lowestWear = 0.15f;
                    highestWear = 0.38f;
                    break;
                case "Well-Worn":
                    lowestWear = 0.38f;
                    highestWear = 0.45f;
                    break;
                case "Battle-Scarred":
                    lowestWear = 0.45f;
                    highestWear = 1f;
                    break;
                default:
                    lowestWear = 0f;
                    highestWear = 1f;
                    break;
            }

            return new FloatRange(lowestWear, highestWear);
        }

    }

    public class InputSkin
    {
        public decimal WearValue { get; set; }
        public float Price { get; set; }
        public Currency SkinCurrency { get; set; }

        public InputSkin(decimal wear, float price, Currency currency)
        {
            WearValue = wear;
            Price = price;
            SkinCurrency = currency;
        }

        public InputSkin(double wear, float price, Currency currency) : this((decimal)wear, price, currency) { }

        internal int CompareTo(InputSkin b)
        {
            return WearValue > b.WearValue ? 1 : (WearValue < b.WearValue ? -1 : 0);
        }

        private RelayCommand copyCommand;

        public RelayCommand CopyCommand
        {
            get
            {
                return copyCommand ??= new RelayCommand(obj =>
                {
                    Clipboard.SetText(WearValue.ToString("0.00000000000000", CultureInfo.InvariantCulture));
                });
            }
        }
    }
}
