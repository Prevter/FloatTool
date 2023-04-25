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

namespace FloatTool.Common
{
	public sealed class Collection
	{
		public string Name;
		public bool CanBeStattrak;
		public string LowestRarity;
		public string HighestRarity;
		public string Link;
		public List<SkinModel> Skins;
	}

	public struct SkinModel
	{
		public string Name;
		public string Rarity;
		public double MinWear;
		public double MaxWear;

		public bool IsQualityInRange(string quality)
		{
			var range = Skin.GetFloatRangeForQuality(quality);
			return new FloatRange(MinWear, MaxWear).IsOverlapped(range);
		}
	}

	public enum Quality
	{
		Consumer,
		Industrial,
		MilSpec,
		Restricted,
		Classified,
		Covert
	}

	public struct Skin
	{
		public string Name;
		public double MinFloat;
		public double MaxFloat;
		public double FloatRange;
		public Quality Rarity;

		public Skin(string name, double minWear, double maxWear, Quality rarity)
		{
			Name = name;
			MinFloat = minWear;
			MaxFloat = maxWear;
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

	public struct InputSkin
	{
		public double WearValue;
		public float Price;
		public Currency SkinCurrency;
		public string ListingID;

		public double GetWearValue => WearValue;

		public InputSkin(double wear, float price, Currency currency, string listingId = "")
		{
			WearValue = wear;
			Price = price;
			SkinCurrency = currency;
			ListingID = listingId;
		}

		internal int CompareTo(InputSkin b)
		{
			return WearValue.CompareTo(b.WearValue);
		}
	}
}
