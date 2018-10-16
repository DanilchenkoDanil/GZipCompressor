using System;
using GZipCompressor.Logic.Interfaces;

namespace GZipCompressor.Logic.Models.BlockingCollections
{
    class FixedSortQueue<TValue> : FixedQueue<TValue> where TValue : class, IComparable<TValue> {
        private TValue m_minValue;
        public TValue CurrentMin {
            get {
                if (Count == 0) throw new InvalidOperationException("Queue is empty");
                return m_minValue;
            }
        }

        public FixedSortQueue(int size) : base(size) {
            m_minValue = default(TValue);
        }

        public override void Enqueue(TValue item) {
            base.Enqueue(item);
            UpdateMin(item);
        }

        public override TValue Dequeue() {
            var dequeued = base.Dequeue();
            if (dequeued.Equals(m_minValue)) {
                FindNewMin();
            }
            return dequeued;
        }

        public void FindNewMin() {
            if (Count == 0)
                m_minValue = default(TValue);
            m_minValue = Data[Head];
            int index = Head;
            int j = 0; 
            while (j < Count) {
                UpdateMin(Data[index]);
                MoveNext(ref index);
                j++;
            }
        }

        public void UpdateMin(TValue item) {
            if (Count == 1) {
                m_minValue = item;
            }
            m_minValue = m_minValue.CompareTo(item) > 0 ? item : m_minValue;
        }

        public virtual void Sort(ISortAlg<TValue> sortAlg) {
            moveDataToStart();
            sortAlg.Sort(ref Data, Count);
        }

        private void moveDataToStart() {
            if (Head != 0) {
                int destHead, sourceHead, count;
                if (Head < Tail) {
                    destHead = 0;
                    sourceHead = Head;
                    count = Count;
                } else //moving from the head to the tail
                  {
                    destHead = Tail;
                    sourceHead = Head;
                    count = Size - Head;
                }

                //moving and update
                moving(destHead, sourceHead, count);
                Head = 0;
                Tail = Count == Size ? 0 : Count;
            }
        }

        private void moving(int destHead, int sourceHead, int count) {
            if (destHead == sourceHead) {
                return;
            }
            while (count > 0) {
                Data[destHead] = Data[sourceHead];
                Data[sourceHead] = default(TValue);
                destHead++;
                sourceHead++;
                count--;
            }
        }
    }
}
