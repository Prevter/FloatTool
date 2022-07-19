namespace ItemsParser
{
    public class Collection
    {
        public string Name { get; set; } = "";
        public string Link { get; set; } = "";
        public bool CanBeStattrak { get; set; }
        public string LowestRarity { get; set; } = "";
        public string HighestRarity { get; set; } = "";
        public List<Skin> Skins { get; set; } = new();
    }

    public class Skin
    {
        public string Name { get; set; } = "";
        public string Rarity { get; set; } = "";
        public float MinWear { get; set; } = 0;
        public float MaxWear { get; set; } = 1;
    }
}
