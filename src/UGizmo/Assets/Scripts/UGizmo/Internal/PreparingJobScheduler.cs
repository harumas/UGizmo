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

        protected TJobData* JobDataPtr => jobData.Ptr;

        protected PreparingJobScheduler()
        {
            jobData = new UnsafeList<TJobData>(InitialCapacity, Allocator.Persistent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TJobData data)
        {
            jobData.Add(data);
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