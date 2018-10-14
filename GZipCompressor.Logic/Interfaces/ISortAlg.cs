using GZipCompressor.Logic.Models;

namespace GZipCompressor.Logic.Interfaces
{
    internal interface ISortAlg<TValue> where TValue : FilePart
    {
        void Sort(ref TValue[] array);
    }
}
