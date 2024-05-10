using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UGizmo.Internal
{
    public class UnsafeListReference<T> : IDisposable where T : unmanaged
    {
        private UnsafeList<T> jobBuffer;

        public ref UnsafeList<T> Ref => ref jobBuffer;

        public UnsafeListReference(int capacity, Allocator allocator)
        {
            jobBuffer = new UnsafeList<T>(capacity, allocator);
        }

        public void Dispose()
        {
            jobBuffer.Dispose();
        }
    }
}