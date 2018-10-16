namespace GZipCompressor.Logic.Interfaces
{
    internal interface ISortAlg<TValue>
    {
        void Sort(ref TValue[] array, int length);
    }
}
