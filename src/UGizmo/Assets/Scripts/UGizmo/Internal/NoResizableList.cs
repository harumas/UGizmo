using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal sealed class NoResizableList<T> : IEnumerable<T>
    {
        private T[] items;
        private int count;

        public NoResizableList(int capacity = 64)
        {
            items = new T[capacity];
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count;
        }

        public Span<T> AsSpan() => items.AsSpan(0, count);

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => items[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => items[index] = value;
        }

        public void SetArray(T[] items)
        {
            this.items = items;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (items.Length == count)
            {
                throw new OutOfMemoryException();
            }

            items[count++] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}