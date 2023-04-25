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
namespace ItemsParser
{
	public sealed class Collection
	{
		public string Name { get; set; } = "";
		public string Link { get; set; } = "";
		public bool CanBeStattrak { get; set; }
		public string LowestRarity { get; set; } = "";
		public string HighestRarity { get; set; } = "";
		public List<Skin> Skins { get; set; } = new();
	}

	public sealed class Skin
	{
		public string Name { get; set; } = "";
		public string Rarity { get; set; } = "";
		public float MinWear { get; set; } = 0;
		public float MaxWear { get; set; } = 1;
	}

	public sealed class PaintKit
	{
		public string Id { get; set; } = "";
		public string Name { get; set; } = "default";
		public string PaintKitName { get; set; } = "#PaintKit_Default";
		public double WearRemapMin { get; set; } = 0.06;
		public double WearRemapMax { get; set; } = 0.8;
	}
}
