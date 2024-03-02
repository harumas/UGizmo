using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace UGizmos
{
    public sealed class RenderDataList<T> : IDisposable where T : unmanaged
    {
        public UnsafeList<T> DataArray;

        public int Count => counter;
        private int counter;
        private bool isDisposed;

        public RenderDataList(int capacity = 8192)
        {
            DataArray = new UnsafeList<T>(capacity, Allocator.Persistent);
        }

        public void Add(in T data)
        {
            if (isDisposed)
            {
                return;
            }

            DataArray.Add(data);
        }

        public void Reset()
        {
            counter = 0;
        }

        public void Dispose()
        {
            Reset();
            DataArray.Dispose();
            isDisposed = true;
        }
    }
}