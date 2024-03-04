using System;
using System.Runtime.CompilerServices;

namespace UGizmo
{
    internal sealed class NoResizableList<T>
    {
        private readonly T[] items;
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
    }
}