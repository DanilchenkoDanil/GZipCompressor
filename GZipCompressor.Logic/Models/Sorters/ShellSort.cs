using GZipCompressor.Logic.Interfaces;

namespace GZipCompressor.Logic.Models.Sorters
{
    internal class ShellSort<TValue> : ISortAlg<TValue> where TValue : FilePart
    {
        void ISortAlg<TValue>.Sort(ref TValue[] array) {
            int i, j, inc;
            TValue temp;
            inc = 3;
            while (inc > 0) {
                for (i = 0; i < array.Length; i++) {
                    j = i;
                    temp = array[i];
                    while ((j >= inc) && (array[j - inc].Index > temp.Index)) {
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
