using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatToolGUI
{
    public class Skin
    {
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
            FloatRange = MaxFloat - MinFloat;
            Rarity = rarity;
        }

        public override string ToString()
        {
            return $"{Name} ({Rarity}) | {MinFloat}-{MaxFloat}";
        }
    }
}
