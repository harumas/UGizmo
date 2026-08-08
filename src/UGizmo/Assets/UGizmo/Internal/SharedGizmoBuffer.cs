using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UGizmo.Internal
{
    [BurstCompile]
    internal unsafe class SharedGizmoBuffer<TJobData> : IDisposable where TJobData : unmanaged
    {
        public ref UnsafeList<TJobData> JobBuffer => ref backJobBuffer;

        public ref UnsafeList<DrawData> DrawBuffer
        {
            get
            {
                drawBuffer.Resize(backJobBuffer.Length);
                return ref drawBuffer;
            }
        }

        private static SharedGizmoBuffer<TJobData> instance;

        public static SharedGizmoBuffer<TJobData> GetSharedBuffer()
        {
            return instance ??= new SharedGizmoBuffer<TJobData>();
        }

        private const int InitialCapacity = 4096;
        private UnsafeList<TJobData> backJobBuffer = new UnsafeList<TJobData>(InitialCapacity, Allocator.Persistent);
        private UnsafeList<DrawData> drawBuffer = new UnsafeList<DrawData>(InitialCapacity, Allocator.Persistent);

        public int Add(in TJobData jobData)
        {
            int handle = backJobBuffer.Length;
            backJobBuffer.Add(jobData);
            return handle;
        }

        public (int start, int length) AddRange(TJobData* jobData, int count)
        {
            int length = backJobBuffer.Length;
            backJobBuffer.AddRange(jobData, count);
            return (length, count);
        }

        /// <summary>
        /// Reserves a range and returns where it starts. Deliberately returns an index rather than
        /// a pointer: a later Reserve can grow this list past its capacity, which reallocates and
        /// frees the old block, so any pointer handed out earlier would dangle. Resolve the index
        /// with <see cref="GetPointer"/> once every reservation for the frame has been made.
        /// </summary>
        public (int start, int length) Reserve(int count)
        {
            int length = backJobBuffer.Length;
            backJobBuffer.Resize(length + count);
            return (length, count);
        }

        /// <summary>
        /// Resolves an index returned by <see cref="Reserve"/> into a pointer. Only valid once all
        /// reservations for the frame are done, because the backing memory can move until then.
        /// </summary>
        public TJobData* GetPointer(int index)
        {
            return backJobBuffer.Ptr + index;
        }

        public void SetRenderData(int* handles, DrawData* destination, int count)
        {
            SetRenderData_Burst(handles, drawBuffer.Ptr, drawBuffer.Length, destination, count);
        }

        [BurstCompile]
        private static void SetRenderData_Burst(int* handles, DrawData* origin, int originLength, DrawData* destination, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int index = handles[i];

                // A stale handle used to read arbitrary memory here and take the process down with
                // it, which is a miserable thing to debug. The check is compiled out of release
                // builds, so the hot path is unchanged where it matters.
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint)index >= (uint)originLength)
                {
                    throw new IndexOutOfRangeException("UGizmo: gizmo handle points outside the shared draw buffer.");
                }
#endif

                destination[i] = origin[index];
            }
        }

        /// <summary>
        /// Resets the buffers for the next frame. Called once per job data type per frame, from
        /// IGizmoJobScheduler.Clear via GizmoDrawSystem.ClearScheduler.
        /// </summary>
        public void Clear()
        {
            backJobBuffer.Clear();
            drawBuffer.Clear();
        }

        public void Dispose()
        {
            backJobBuffer.Dispose();
            drawBuffer.Dispose();
        }
    }
}