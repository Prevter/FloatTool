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
using ItemsParser;
using Newtonsoft.Json;

// Download game files
if (args.Contains("--redownload") || args.Contains("-r")
|| !File.Exists("items_game.vdf") || !File.Exists("csgo_english.vdf"))
{
	Console.WriteLine("Downloading files...");
	await Utils.DownloadFile("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CSGO/master/csgo/scripts/items/items_game.txt", "items_game.vdf");
	await Utils.DownloadFile("https://raw.githubusercontent.com/SteamDatabase/GameTracking-CSGO/master/csgo/resource/csgo_english.txt", "csgo_english.vdf");
}

// Parse game files
Console.WriteLine("Parsing items_game.vdf...");
var items_game = VdfConvert.Parse(File.ReadAllText("items_game.vdf"));

Console.WriteLine("Parsing csgo_english.vdf...");
var csgo_english = VdfConvert.Parse(File.ReadAllText("csgo_english.vdf"));

// Optionally convert to JSON
if (args.Contains("--json") || args.Contains("-j"))
{
	Console.WriteLine("Saving items_game.json");
	File.WriteAllText("items_game.json", JsonConvert.SerializeObject(items_game, Formatting.Indented));

	Console.WriteLine("Saving csgo_english.json");
	File.WriteAllText("csgo_english.json", JsonConvert.SerializeObject(csgo_english, Formatting.Indented));
}

// Make all language keys lowercase for easier access
Dictionary<string, string> tokens = new();
foreach (var key in csgo_english["lang"]["Tokens"].Keys)
	tokens[key.ToLower()] = csgo_english["lang"]["Tokens"][key];
csgo_english["lang"]["Tokens"] = tokens;

// Parse data
ItemParser parser = new(items_game, csgo_english);
List<Collection> collections = parser.ComposeData();

// Save result
File.WriteAllText("SkinList.json", JsonConvert.SerializeObject(collections, Formatting.Indented));