using System;
using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal sealed class NoResizableList<T>
    {
        private const int DefaultCapacity = 64;

        private T[] items;
        private int count;

        public NoResizableList(int capacity = DefaultCapacity)
        {
            items = new T[capacity];
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count;
        }

        public Span<T> AsSpan() => items.AsSpan(0, count);

        public void SetArray(T[] items)
        {
            this.items = items;
        }

        /// <summary>
        /// Grows on demand. The "no resizable" part is about <see cref="AsSpan"/> staying valid
        /// while it is being iterated: Add only ever runs during registration, before any frame has
        /// been drawn, so growing here cannot invalidate a span held by the per-frame loops.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (items.Length == count)
            {
                Array.Resize(ref items, count == 0 ? DefaultCapacity : count * 2);
            }

            items[count++] = item;
        }
    }
}