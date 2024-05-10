using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Internal
{
    internal interface IPreparingJobScheduler : IDisposable
    {
        void Register(GizmoInstanceActivator activator);
        void EnqueueContinuousGizmo();
        void ClearContinuousGizmo();
        JobHandle Schedule();
        void Clear();
    }

    internal abstract unsafe class PreparingJobScheduler<TScheduler, TJobData> : IPreparingJobScheduler
        where TScheduler : PreparingJobScheduler<TScheduler, TJobData>, new()
        where TJobData : unmanaged
    {
        protected int InstanceCount => jobData.Length;

        private const int InitialCapacity = 4096;
        private UnsafeList<TJobData> jobData;
        private ContinuousGizmoBuffer<TJobData> continuousGizmoBuffer;

        protected TJobData* JobDataPtr => jobData.Ptr;

        protected PreparingJobScheduler()
        {
            jobData = new UnsafeList<TJobData>(InitialCapacity, Allocator.Persistent);
            continuousGizmoBuffer = new ContinuousGizmoBuffer<TJobData>(data => jobData.Add(data));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data, float duration)
        {
            if (duration > 0f)
            {
                continuousGizmoBuffer.Add(data, duration);
            }
            else
            {
                jobData.Add(data);
            }
        }

        public void EnqueueContinuousGizmo()
        {
            continuousGizmoBuffer.EnqueueAllJobData();
        }

        public void ClearContinuousGizmo()
        {
            continuousGizmoBuffer.Clear();
        }

        public abstract JobHandle Schedule();

        public void Register(GizmoInstanceActivator activator)
        {
            activator.RegisterScheduler<TScheduler, TJobData>((TScheduler)this);
        }

        public void Clear()
        {
            jobData.Clear();
        }

        public void Dispose()
        {
            jobData.Dispose();
        }
    }
}