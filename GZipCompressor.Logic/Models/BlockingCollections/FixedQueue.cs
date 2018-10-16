using System;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    internal class FixedQueue<TValue> : Queue<TValue>
    {
        protected readonly int Size;

        internal FixedQueue(int size) : base() {
            Size = size;
            Data = new TValue[size];
        }

        public override void Enqueue(TValue item) {
            if (Count == Size)
                throw new InvalidOperationException("Queue is full");
            base.Enqueue(item);
        }

        protected override void MoveNext(ref int index) {
            base.MoveNext(ref index);
            if (index == Data.Length) index = 0;
        }
    }
}
