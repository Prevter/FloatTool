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
        public float MinFloat { get; set; }
        public float MaxFloat { get; set; }
        public Quality Rarity { get; set; }

        public Skin(string name, float minWear, float maxWear, Quality rarity)
        {
            Name = name;
            MinFloat = minWear;
            MaxFloat = maxWear;
            Rarity = rarity;
        }

        public override string ToString()
        {
            return $"{Name} ({Rarity}) | {MinFloat}-{MaxFloat}";
        }
    }
}
