/*
- Copyright(C) 2023 Prevter
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
using System.Globalization;
using System.Text.RegularExpressions;

namespace ItemsParser
{
	public sealed class ItemParser
	{
		// I hate valve for this...
		readonly string[] ExcludeStattrak = new string[]
		{
			"Anubis Collection Package"
		};
		readonly Dictionary<string, dynamic> items_game;
		readonly Dictionary<string, dynamic> language;

		List<PaintKit>? paintKits;
		List<SkinSet>? lootLists;
		Dictionary<string, string>? rarities;
		readonly Dictionary<string, string> translatedWeapons = new();

		readonly Regex weaponPaintKitRegex = new(@"\[([a-zA-Z0-9_-]{1,})\]([a-z0-9_]{1,})");

		public ItemParser(dynamic items_game, dynamic language)
		{
			this.items_game = items_game;
			this.language = language;
		}

		public string GetTranslation(string key)
		{
			if (key.StartsWith('#'))
				key = key[1..];

			key = key.ToLower();
			if (language["lang"]["Tokens"].ContainsKey(key))
				return language["lang"]["Tokens"][key];

			return key;
		}

		public List<PaintKit> GetPaintKits()
		{
			if (paintKits is not null)
				return paintKits;

			paintKits = new();
			foreach (var key in ((Dictionary<string, object>)items_game["items_game"]["paint_kits"]).Keys)
			{
				Dictionary<string, object> paint_kit = items_game["items_game"]["paint_kits"][key];
				var paintKitObj = new PaintKit()
				{
					Id = key
				};

				if (paint_kit.ContainsKey("name"))
					paintKitObj.Name = paint_kit["name"] as string;

				if (paint_kit.ContainsKey("description_tag"))
					paintKitObj.PaintKitName = GetTranslation(paint_kit["description_tag"] as string);

				if (paint_kit.ContainsKey("wear_remap_min"))
					paintKitObj.WearRemapMin = double.Parse((string)paint_kit["wear_remap_min"], CultureInfo.InvariantCulture);

				if (paint_kit.ContainsKey("wear_remap_max"))
					paintKitObj.WearRemapMax = double.Parse((string)paint_kit["wear_remap_max"], CultureInfo.InvariantCulture);

				paintKits.Add(paintKitObj);
			}
			return paintKits;
		}

		public PaintKit? FindByName(string name)
		{
			List<PaintKit> paints = GetPaintKits();
			foreach (var paint in paints)
				if (paint.Name == name)
					return paint;
			return null;
		}

		public string GetTranslatedWeaponName(string weapon)
		{
			if (translatedWeapons.ContainsKey(weapon))
				return translatedWeapons[weapon];

			foreach (var key in ((Dictionary<string, object>)items_game["items_game"]["prefabs"]).Keys)
			{
				Dictionary<string, object> item = items_game["items_game"]["prefabs"][key];
				if (item.ContainsKey("anim_class") && item["anim_class"] as string == weapon)
				{
					string translated = GetTranslation(item["item_name"] as string);
					translatedWeapons.Add(weapon, translated);
					return translated;
				}

				if (item.ContainsKey("item_class") && item["item_class"] as string == weapon)
				{
					string translated = GetTranslation(item["item_name"] as string);
					translatedWeapons.Add(weapon, translated);
					return translated;
				}
			}

			return weapon;
		}

		public List<SkinSet> GetCollections()
		{
			List<SkinSet> collections = new();
			foreach (var key in ((Dictionary<string, object>)items_game["items_game"]["item_sets"]).Keys)
			{
				Dictionary<string, object> item_set = items_game["items_game"]["item_sets"][key];
				List<string> skins = new();

				foreach (var skin in ((Dictionary<string, object>)item_set["items"]).Keys)
				{
					if (weaponPaintKitRegex.Match(skin).Success)
						skins.Add(skin);
				}

				if (!skins.Any()) continue;

				collections.Add(new SkinSet
				{
					Id = key,
					Name = GetTranslation(item_set["name"] as string),
					Skins = skins
				});
			}

			return collections;
		}

		public Dictionary<string, string> GetRarities()
		{
			if (rarities is not null)
				return rarities;

			rarities = new();
			foreach (var key in ((Dictionary<string, object>)items_game["items_game"]["rarities"]).Keys)
			{
				Dictionary<string, object> rarity = items_game["items_game"]["rarities"][key];
				rarities.Add(key, GetTranslation(rarity["loc_key_weapon"] as string).Replace(" Grade", ""));
			}
			return rarities;
		}

		public List<SkinSet> GetLootLists()
		{
			if (lootLists is not null)
				return lootLists;

			List<string> _parseLootTable(string loot_table_id)
			{
				// Check if the loot table exists
				if (!items_game["items_game"]["client_loot_lists"].ContainsKey(loot_table_id))
					return new List<string>();

				// Get the loot table
				var loot_table = items_game["items_game"]["client_loot_lists"][loot_table_id];

				// Get the items in the loot table
				List<string> items = new();
				foreach (var item_id in loot_table.Keys)
					items.Add(item_id);

				return items;
			}

			lootLists = new();
			foreach (var key in ((Dictionary<string, object>)items_game["items_game"]["revolving_loot_lists"]).Keys)
			{
				var loot_list = items_game["items_game"]["revolving_loot_lists"][key];
				List<string> items = new();

				if (items_game["items_game"]["client_loot_lists"].ContainsKey(loot_list))
				{
					var loot_table = items_game["items_game"]["client_loot_lists"][loot_list];
					foreach (var loot_id in loot_table.Keys)
					{
						var loot = _parseLootTable(loot_id);
						if (loot.Count > 0)
							items.AddRange(loot);
						else
							items.Add(loot_id);
					}
				}

				lootLists.Add(new SkinSet
				{
					Id = key,
					Name = loot_list,
					Skins = items
				});
			}
			return lootLists;
		}

		public CaseSet? FindCaseSet(string collection_id)
		{
			foreach (var key in ((Dictionary<string, object>)items_game["items_game"]["items"]).Keys)
			{
				Dictionary<string, dynamic> item = items_game["items_game"]["items"][key];
				if (item.ContainsKey("prefab") && item["prefab"].Contains("weapon_case") && item.ContainsKey("tags")
					&& item.ContainsKey("attributes") && !item["attributes"].ContainsKey("tournament event id")
					&& item["tags"].ContainsKey("ItemSet") && item["tags"]["ItemSet"]["tag_value"] == collection_id)
				{
					CaseSet caseSet = new();
					if (item.ContainsKey("id")) caseSet.Id = item["id"];
					else caseSet.Id = item["attributes"]["set supply crate series"]["value"];

					caseSet.ImageInventory = item["image_inventory"].Split("/")[2];
					caseSet.Name = item["name"];
					caseSet.ItemName = item["item_name"];
					return caseSet;
				}
			}
			return null;
		}

		public string FindRarity(string item, string paintKit)
		{
			// find which item in client_loot_lists contains the item
			foreach (var key in ((Dictionary<string, object>)items_game["items_game"]["client_loot_lists"]).Keys)
			{
				Dictionary<string, object> loot_list = items_game["items_game"]["client_loot_lists"][key];
				if (loot_list.ContainsKey(item))
					return GetRarities()[key.Split('_').Last()];
			}
			// if we can't find the rarity, find it for the paintkit
			if (items_game["items_game"]["paint_kits_rarity"].ContainsKey(paintKit))
			{
				return GetRarities()[items_game["items_game"]["paint_kits_rarity"][paintKit] as string];
			}
			return "common";
		}

		public int CompareRarity(string a, string b)
		{
			var rarities = GetRarities().Values.ToList();
			return rarities.IndexOf(a) - rarities.IndexOf(b);
		}

		public List<Collection> ComposeData()
		{
			List<Collection> collections = new();

			foreach (var collection in GetCollections())
			{
				Console.WriteLine($"Processing collection {collection.Name}...");

				Collection collectionObj = new();

				var caseSet = FindCaseSet(collection.Id);
				if (caseSet is null)
				{
					collectionObj.Name = collection.Name;
					collectionObj.Link = $"https://stash.clash.gg/collection/{collection.Name.Replace(' ', '+')}";
				}
				else
				{
					collectionObj.Name = GetTranslation(caseSet.ItemName);

					// Check for exclude list:
					if (!ExcludeStattrak.Contains(collectionObj.Name))
						collectionObj.CanBeStattrak = true;

					var set = GetLootLists().FirstOrDefault(set => set.Name == caseSet.Name || set.Name == caseSet.ImageInventory, null);
					var set_id = set is null ? caseSet.Id : set.Id;
					collectionObj.Link = $"https://stash.clash.gg/case/{set_id}/{collection.Name.Replace(' ', '-')}";
				}

				string lowestRarity = "Contraband";
				string highestRarity = "Stock";

				foreach (var skin in collection.Skins)
				{
					Match match = weaponPaintKitRegex.Match(skin);
					string weapon = match.Groups[2].Value;
					string paintKitName = match.Groups[1].Value;
					PaintKit paintKit = FindByName(paintKitName)!;
					string name = $"{GetTranslatedWeaponName(weapon)} | {paintKit.PaintKitName}";
					bool found = false;
					foreach (var s in collectionObj.Skins)
					{
						if (s.Name == name)
						{
							Console.WriteLine($"Duplicate skin {name} found in collection {collection.Name}!");
							found = true;
							break;
						}
					}

					if (found) continue;

					string rarity = FindRarity(skin, paintKitName);

					if (CompareRarity(rarity, lowestRarity) < 0)
						lowestRarity = rarity;

					if (CompareRarity(rarity, highestRarity) > 0)
						highestRarity = rarity;

					collectionObj.Skins.Add(new Skin
					{
						Name = name,
						Rarity = rarity,
						MinWear = (float)paintKit.WearRemapMin,
						MaxWear = (float)paintKit.WearRemapMax,
					});
				}

				collectionObj.HighestRarity = highestRarity;
				collectionObj.LowestRarity = lowestRarity;

				collections.Add(collectionObj);
			}

			collections.Reverse();

			return collections;
		}


		public sealed class SkinSet
		{
			public string Id { get; set; } = "";
			public string Name { get; set; } = "";
			public List<string> Skins { get; set; } = new();
		}

		public sealed class CaseSet
		{
			public string Id { get; set; } = "";
			public string ItemName { get; set; } = "";
			public string Name { get; set; } = "";
			public string ImageInventory { get; set; } = "";
		}

	}
}
