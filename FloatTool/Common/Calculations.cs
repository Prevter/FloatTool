using System.Collections.Generic;
using System.Numerics;

namespace FloatTool
{
    static public class Calculations
    {
        static public decimal Craft(InputSkin[] ingridients, decimal minFloat, decimal floatRange)
        {
            decimal avgFloat = ingridients[0].WearValue;
            avgFloat += ingridients[1].WearValue;
            avgFloat += ingridients[2].WearValue;
            avgFloat += ingridients[3].WearValue;
            avgFloat += ingridients[4].WearValue;
            avgFloat += ingridients[5].WearValue;
            avgFloat += ingridients[6].WearValue;
            avgFloat += ingridients[7].WearValue;
            avgFloat += ingridients[8].WearValue;
            avgFloat += ingridients[9].WearValue;
            return floatRange * avgFloat + minFloat;
        }

        static public bool NextCombination(int[] num, int n)
        {
            bool finished = false;
            bool changed = false;
            for (int i = 9; !finished && !changed; --i)
            {
                if (num[i] < n + i)
                {
                    ++num[i];
                    if (i < 9)
                        for (int j = i + 1; j < 10; ++j)
                            num[j] = num[j - 1] + 1;
                    return true;
                }
                finished = i == 0;
            }
            return changed;
        }

        static public IEnumerable<InputSkin[]> Combinations(InputSkin[] elem, int start, int skip)
        {
            int size = elem.Length - 10;
            int[] numbers = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            long step = -start;
            InputSkin[] resultList = new InputSkin[10];

            do
            {
                if (step++ % skip == 0)
                {
                    for (int i = 0; i < 10; ++i)
                        resultList[i] = elem[numbers[i]];
                    yield return resultList;
                }
            } while (NextCombination(numbers, size));
        }

        public static long GetCombinationsCount(int poolSize)
        {
            BigInteger fact1 = poolSize;
            for (int i = poolSize - 1; i > 10; i--)
                fact1 *= i;

            BigInteger fact2 = poolSize - 10;
            for (int i = poolSize - 11; i > 1; i--)
                fact2 *= i;

            return (long)(fact1 / fact2);
        }
    }
}
