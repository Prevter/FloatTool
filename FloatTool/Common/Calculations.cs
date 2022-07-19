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
using System.Numerics;

namespace FloatTool
{
    static public class Calculations
    {
        static public decimal Craft(InputSkin[] ingridients, decimal minFloat, decimal floatRange)
        {
            return floatRange * (ingridients[0].WearValue
                + ingridients[1].WearValue
                + ingridients[2].WearValue
                + ingridients[3].WearValue
                + ingridients[4].WearValue
                + ingridients[5].WearValue
                + ingridients[6].WearValue
                + ingridients[7].WearValue
                + ingridients[8].WearValue
                + ingridients[9].WearValue) + minFloat;
        }

        static public bool NextCombination(int[] num, int n)
        {
            bool finished = false;
            for (int i = 9; !finished; --i)
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
            return false;
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
