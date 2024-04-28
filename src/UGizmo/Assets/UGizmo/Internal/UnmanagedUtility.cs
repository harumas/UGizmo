using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UGizmo.Internal
{
    internal static unsafe class UnmanagedUtility
    {
        internal static T* Malloc<T>(int count, Allocator allocator) where T : unmanaged
        {
            long size = (long)UnsafeUtility.SizeOf<T>() * (long)count;

            return (T*)UnsafeUtility.Malloc(size, UnsafeUtility.AlignOf<T>(), allocator);
        }

        internal static void Free<T>(ref T* ptr, Allocator allocator) where T : unmanaged
        {
            if (ptr == (T*)0)
            {
                throw new ObjectDisposedException("The Pointer has been disposed, it is not allowed to access it");
            }

            UnsafeUtility.Free(ptr, allocator);
            ptr = (T*)0;
        }

        internal static void ResizePointer<T>(ref T* pointer, int from, int to, Allocator allocator) where T : unmanaged
        {
            T* newPointer = Malloc<T>(to, allocator);
            UnsafeUtility.MemCpy(newPointer, pointer, UnsafeUtility.SizeOf<T>() * from);
            Free(ref pointer, allocator);

            pointer = newPointer;
        }
    }
}