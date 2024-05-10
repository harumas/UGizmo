using System;
using System.Collections;

namespace UGizmo.Internal
{
    public sealed class AscendingPriorityQueue<T> where T : unmanaged, IComparable<T>
    {
        private readonly T[] array;
        private readonly IComparer comparer;

        public int Count { get; private set; } = 0;
        public T Root => array[0];

        public AscendingPriorityQueue(int capacity, IComparer comparer = null)
        {
            array = new T[capacity];
            this.comparer = comparer;
        }

        /// <summary>
        /// 要素を挿入する
        /// </summary>
        public void Push(T item)
        {
            array[Count++] = item;

            var n = Count - 1; // 末尾(追加した)のノードの番号
            while (n != 0)
            {
                var parent = (n - 1) / 2; // 親ノードの番号

                if (Compare(array[n], array[parent]) < 0)
                {
                    Swap(n, parent);
                    n = parent;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 優先度の一番高いものを取り出す
        /// </summary>
        public T Pop()
        {
            Swap(0, this.Count - 1); // 先頭要素を末尾にする
            Count -= 1;

            var parent = 0; // 親ノードの番号
            while (true)
            {
                var child = 2 * parent + 1; // 子ノードの番号
                if (child > Count - 1) break;

                // 値の大きい方の子を選ぶ
                if (child < Count - 1 && Compare(array[child], array[child + 1]) > 0) child += 1;

                // 子の方が親より大きければ入れ替える
                if (Compare(array[parent], array[child]) > 0)
                {
                    Swap(parent, child);
                    parent = child;
                }
                else
                {
                    break;
                }
            }

            return array[Count];
        }

        private int Compare(T a, T b)
        {
            if (comparer == null) return a.CompareTo(b);
            return comparer.Compare(a, b);
        }

        private void Swap(int a, int b)
        {
            (array[a], array[b]) = (array[b], array[a]);
        }
    }
}