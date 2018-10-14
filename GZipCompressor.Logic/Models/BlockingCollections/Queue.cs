using System;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    class Queue<TValue>
    {
        protected TValue[] Data;
        protected int Head;
        protected int Tail;

        public int Count = 0;

        protected Queue() { }

        public virtual void Enque(TValue item) {
            if (item == null)
                throw new ArgumentNullException($"Parameter {nameof(item)} is null");

            Data[Tail] = item;
            MoveNext(ref Tail);
            Count++;
        }

        public virtual TValue Dequeue() {
            if (Data.Length == 0) {
                throw new InvalidOperationException("Queue is empty");
            }
            TValue dequeuedItem = Data[Head];
            Data[Head] = default(TValue);
            MoveNext(ref Head);
            Count--;

            return dequeuedItem;
        }

        public virtual TValue GetPeek() {
            if (Count == 0)
                throw new InvalidOperationException("Queue is empty");

            return Data[Head];
        }

        protected virtual void MoveNext(ref int index) {
            Tail = ++index;
        }
    }
}
