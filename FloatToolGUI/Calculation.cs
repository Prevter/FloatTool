using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FloatToolGUI
{
    static class Calculation
    {
        [DllImport("FloatCore.dll")]
        static public extern double GetOutputWear(double[] floats, float minWear, float maxWear);

        /// <summary>
        /// Calculates wear value based of input skins and min/max value of outcome
        /// </summary>
        /// <param name="ingridients">List of 10 skins</param>
        /// <param name="minFloat">Minimal wear value of skin that is going to be crafted</param>
        /// <param name="maxFloat">Maximum wear value of skin that is going to be crafted</param>
        /// <returns>Wear value represented in decimal type</returns>
        static public decimal craft(List<InputSkin> ingridients, decimal minFloat, decimal maxFloat)
        {
            decimal avgFloat = ingridients[0].WearValue;
            for (int i = 1; i < 10; i++)
            {
                avgFloat += ingridients[i].WearValue;
            }
            avgFloat /= 10;
            return (maxFloat - minFloat) * avgFloat + minFloat;
        }

        /// <summary>
        /// Does same job as craft(List<InputSkin>, float, float), but uses float as type
        /// </summary>
        /// <param name="ingridients">List of 10 skins</param>
        /// <param name="minFloat">Minimal wear value of skin that is going to be crafted</param>
        /// <param name="maxFloat">Maximum wear value of skin that is going to be crafted</param>
        /// <returns>Float wear value in string</returns>
        static public string craftF(List<InputSkin> ingridients, decimal minFloat, decimal maxFloat)
        {
            float avgFloat = 0;
            float[] arrInput = new float[10];
            for (int i = 0; i < 10; i++)
            {
                arrInput[i] = Convert.ToSingle(ingridients[i].WearValue);
            }
            for (int i = 0; i < 10; i++)
            {
                avgFloat += Convert.ToSingle(arrInput[i]);
            }
            avgFloat /= 10;
            return setprecission(((float)(maxFloat - minFloat) * avgFloat) + (float)minFloat, 10);
        }

        public static string setprecission(double number, int figures)
        {
            int e = 0;
            while (number >= 10.0)
            {
                e += 1;
                number /= 10;
            }
            while (number < 1.0)
            {
                e -= 1;
                number *= 10;
            }
            figures--;
            number = (float)Math.Round(number, figures);
            figures += 0 - e;
            while (e > 0)
            {
                number *= 10;
                e -= 1;
            }
            while (e < 0)
            {
                number /= 10;
                e += 1;
            }
            if (figures < 0)
                figures = 0;
            return number.ToString($"f{figures}", CultureInfo.InvariantCulture);
        }

        public static Quality FromString(string value)
        {
            if (value == "Consumer") return Quality.Consumer;
            else if (value == "Industrial") return Quality.Industrial;
            else if (value == "Mil-Spec") return Quality.MilSpec;
            else if (value == "Restricted") return Quality.Restricted;
            else if (value == "Classified") return Quality.Classified;
            else return Quality.Covert;
        }

        public static List<Skin>[] GroupOutcomes(List<dynamic> skins)
        {
            var allList = new List<List<Skin>>(); //List with all outcomes
            float[] currIter = { 0f, 1f }; //Last iteration wear range
            List<float[]> floatRanges = new List<float[]>(); //List of all ranges that has been parsed

            foreach (var skin in skins)
            {
                float[] curr = { skin["maxWear"], skin["minWear"] };
                List<Skin> list = new List<Skin>();
                if (curr.SequenceEqual(currIter) || floatRanges.Any(x => (x.SequenceEqual(curr)))) continue; //If range already exists
                else
                {
                    currIter = curr;
                    floatRanges.Add(currIter);
                }
                foreach (var skin1 in skins)
                    if ((skin["maxWear"] == skin1["maxWear"]) && (skin["minWear"] == skin1["minWear"]))
                        list.Add(new Skin(skin1["name"].ToString(), float.Parse(skin1["minWear"].ToString().Replace('.', ',')), float.Parse(skin1["maxWear"].ToString().Replace('.', ',')), FromString(skin1["rarity"].ToString().Split(' ')[0])));
                allList.Add(list);
            }

            return allList.ToArray();
        }

        static public bool NextCombination(IList<int> num, int n)
        {
            bool finished;
            var changed = finished = false;
            for (var i = 9; !finished && !changed; i--)
            {
                if (num[i] < n - 10 + i)
                {
                    num[i]++;
                    if (i < 9)
                        for (var j = i + 1; j < 10; j++)
                            num[j] = num[j - 1] + 1;
                    changed = true;
                }
                finished = i == 0;
            }
            return changed;
        }

        static public IEnumerable Combinations<T>(IEnumerable<T> elements, int start, int skip)
        {
            var elem = elements.ToArray();
            var size = elem.Length;
            if (10 > size) yield break;
            var numbers = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            uint step = 0;
            do
            {
                if ((step - start) % skip == 0)
                    yield return numbers.Select(n => elem[n]);
                step++;
            } while (NextCombination(numbers, size));
        }

        

    }
}
