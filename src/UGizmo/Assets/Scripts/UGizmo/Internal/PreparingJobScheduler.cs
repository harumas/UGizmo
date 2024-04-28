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

    internal abstract unsafe class PreparingJobScheduler<TPreparingScheduler, TPrepareData> : IPreparingJobScheduler
        where TPreparingScheduler : PreparingJobScheduler<TPreparingScheduler, TPrepareData>, new()
        where TPrepareData : unmanaged
    {
        protected int InstanceCount => jobData.Length;

        private const int InitialCapacity = 4096;
        private UnsafeList<TPrepareData> jobData;

        protected TPrepareData* JobDataPtr => jobData.Ptr;

        protected PreparingJobScheduler()
        {
            jobData = new UnsafeList<TPrepareData>(InitialCapacity, Allocator.Persistent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TPrepareData data)
        {
            jobData.Add(data);
        }

        public abstract JobHandle Schedule();

        public void Register(GizmoInstanceActivator activator)
        {
            activator.RegisterScheduler<TPreparingScheduler, TPrepareData>((TPreparingScheduler)this);
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