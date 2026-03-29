using System;
using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal interface IContinuousGizmoReceiver<TJobData> where TJobData : unmanaged
    {
        void AddFromContinuous(in TJobData data);
    }

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

    internal unsafe class ContinuousGizmoBuffer<TJobData, TReceiver>
        where TJobData : unmanaged
        where TReceiver : IContinuousGizmoReceiver<TJobData>
    {
        private int length = 0;
        private ContinuousGizmoData<TJobData>[] array;
        private TReceiver receiver;

        private const int InitialCapacity = 1024;

        public ContinuousGizmoBuffer(TReceiver receiver)
        {
            array = new ContinuousGizmoData<TJobData>[InitialCapacity];
            this.receiver = receiver;
        }

        /// <summary>
        /// Add JobData with duration.
        /// </summary>
        /// <param name="jobData"></param>
        /// <param name="duration"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData jobData, float duration)
        {
            EnsureCapacity(1);

            array[length++] = new ContinuousGizmoData<TJobData>(DateTime.Now + TimeSpan.FromSeconds(duration), jobData);
        }

        /// <summary>
        /// Adds JobData for a given range with duration
        /// </summary>
        /// <param name="jobData"></param>
        /// <param name="count"></param>
        /// <param name="duration"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(TJobData* jobData, int count, float duration)
        {
            EnsureCapacity(count);

            int offset = length;
            length += count;
            DateTime endTime = DateTime.Now + TimeSpan.FromSeconds(duration);

            for (int i = 0; i < count; i++)
            {
                array[offset + i] = new ContinuousGizmoData<TJobData>(endTime, jobData[i]);
            }
        }

        private void EnsureCapacity(int offset)
        {
            if (length + offset > array.Length)
            {
                Array.Resize(ref array, Math.Max(length + offset, array.Length * 2));
            }
        }

        /// <summary>
        /// Enqueue all data being retained.
        /// </summary>
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
                    //Remove and Swap
                    array[index] = array[--length];
                    continue;
                }

                receiver.AddFromContinuous(data.JobData);
                index++;
            }
        }

        /// <summary>
        /// Clear buffer.
        /// </summary>
        public void Clear()
        {
            length = 0;
        }
    }
}