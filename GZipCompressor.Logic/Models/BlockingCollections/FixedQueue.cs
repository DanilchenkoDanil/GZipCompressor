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

        public virtual bool TryEnqueue(TValue item) {
            if (Count < Size) {
                base.Enqueue(item);
                return true;
            }

            return false;
        }

        public virtual bool TryDequeue(out TValue item) {
            item = default(TValue);
            if (Count > 0) {
                item = base.Dequeue();
                return true;
            }

            return false;
        }

        public bool TryGetPeek(out TValue item) {
            item = default(TValue);
            if (Count > 0) {
                item = base.GetPeek();
                return true;
            }

            return false;
        }

        protected override void MoveNext(ref int index) {
            base.MoveNext(ref index);
            if (index == Data.Length) index = 0;
        }
    }
}
