using GZipCompressor.Logic.Interfaces;
using System;

namespace GZipCompressor.Logic.Models.Sorters
{
    internal class ShellSort<TValue> : ISortAlg<TValue> where TValue : IComparable<TValue>
    {
        void ISortAlg<TValue>.Sort(ref TValue[] array, int length) {
            int i, j, inc;
            TValue temp;
            inc = 3;
            while (inc > 0) {
                for (i = 0; i < length; i++) {
                    j = i;
                    temp = array[i];
                    while ((j >= inc) && (array[j - inc].CompareTo(temp) > 0)) {
                        array[j] = array[j - inc];
                        j = j - inc;
                    }
                    array[j] = temp;
                }
                if (inc / 2 != 0)
                    inc = inc / 2;
                else if (inc == 1)
                    inc = 0;
                else
                    inc = 1;
            }
        }
    }
}
