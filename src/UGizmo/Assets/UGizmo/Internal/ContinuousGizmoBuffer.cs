using System;
using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal readonly struct ContinuousGizmoData<TJobData> where TJobData : unmanaged
    {
        public readonly DateTime EndTime;
        public readonly TJobData JobData;

        public ContinuousGizmoData(DateTime endTime, TJobData jobData)
        {
            EndTime = endTime;
            JobData = jobData;
        }
    }

    internal unsafe class ContinuousGizmoBuffer<TJobData> where TJobData : unmanaged
    {
        private int length = 0;
        private ContinuousGizmoData<TJobData>[] array;
        private Action<TJobData> enqueueData;

        private const int InitialCapacity = 1024;

        public ContinuousGizmoBuffer(Action<TJobData> enqueueData)
        {
            array = new ContinuousGizmoData<TJobData>[InitialCapacity];
            this.enqueueData = enqueueData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData jobData, float duration)
        {
            EnsureCapacity(1);

            array[length++] = new ContinuousGizmoData<TJobData>(DateTime.Now + TimeSpan.FromSeconds(duration), jobData);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(TJobData* jobData, int count, float duration)
        {
            EnsureCapacity(count);

            int offset = length;
            length += count;

            for (int i = 0; i < count; i++)
            {
                array[offset + i] = new ContinuousGizmoData<TJobData>(DateTime.Now + TimeSpan.FromSeconds(duration), jobData[i]);
            }
        }

        private void EnsureCapacity(int offset)
        {
            if (length + offset > array.Length)
            {
                Array.Resize(ref array, Math.Max(length + offset, array.Length * 2));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnqueueAllJobData()
        {
            int index = 0;
            DateTime now = DateTime.Now;

            while (index < length)
            {
                ContinuousGizmoData<TJobData> data = array[index];

                if (data.EndTime < now)
                {
                    array[index] = array[--length];
                    continue;
                }

                enqueueData(data.JobData);
                index++;
            }
        }

        public void Clear()
        {
            length = 0;
        }
    }
}