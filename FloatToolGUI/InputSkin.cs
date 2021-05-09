using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatToolGUI
{
    public class InputSkin
    {
        public Skin SkinReference { get; set; }
        public decimal WearValue { get; set; }
        public float Price { get; set; }
        public Currency SkinCurrency { get; set; }


        public InputSkin(decimal wear, float price, Currency currency) {
            WearValue = wear;
            Price = price;
            SkinCurrency = currency;
        }

        public InputSkin(double wear, float price, Currency currency)
        {
            WearValue = (decimal)wear;
            Price = price;
            SkinCurrency = currency;
        }

        internal int CompareTo(InputSkin b)
        {
            return WearValue > b.WearValue ? 1 : (WearValue < b.WearValue ? -1 : 0);
        }

        public override string ToString()
        {
            return WearValue.ToString();
        }
    }
}
