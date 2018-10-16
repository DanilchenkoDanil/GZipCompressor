using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    class Queue<TValue> : IEnumerable<TValue>
    {
        protected TValue[] Data;
        protected int Head;
        protected int Tail;

        public int Count = 0;

        public Queue() { }

        public virtual void Enqueue(TValue item) {
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
            ++index;
        }

        public IEnumerator<TValue> GetEnumerator() {
            return Data.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
