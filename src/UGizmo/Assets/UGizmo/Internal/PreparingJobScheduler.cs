using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UGizmo.Internal
{
    internal interface IPreparingJobScheduler : IDisposable
    {
        /// <summary>
        /// False when nothing was queued this frame. Most schedulers are empty on any given frame,
        /// and scheduling an empty parallel job still costs a job struct copy, safety handle setup
        /// and a scheduler enqueue, so the caller skips them.
        /// </summary>
        bool HasData { get; }

        void Register(GizmoInstanceActivator activator);
        void EnqueueContinuousGizmo();
        void ClearContinuousGizmo();

        /// <summary>
        /// Reserves every range this scheduler needs in the shared buffers. Runs for all schedulers
        /// before any of them schedules, so that no later reservation can reallocate a buffer whose
        /// pointer has already been captured.
        /// </summary>
        void ReserveBuffers();

        JobHandle Schedule();
        void Clear();
    }

    internal abstract unsafe class PreparingJobScheduler<TScheduler, TJobData> : IPreparingJobScheduler, IContinuousGizmoReceiver<TJobData>
        where TScheduler : PreparingJobScheduler<TScheduler, TJobData>, new()
        where TJobData : unmanaged
    {
        protected int InstanceCount => jobData.Length;

        public bool HasData => jobData.Length > 0;

        private const int InitialCapacity = 4096;
        private UnsafeList<TJobData> jobData;
        private ContinuousGizmoBuffer<TJobData, PreparingJobScheduler<TScheduler, TJobData>> continuousGizmoBuffer;

        protected TJobData* JobDataPtr => jobData.Ptr;

        protected PreparingJobScheduler()
        {
            jobData = new UnsafeList<TJobData>(InitialCapacity, Allocator.Persistent);
            continuousGizmoBuffer = new ContinuousGizmoBuffer<TJobData, PreparingJobScheduler<TScheduler, TJobData>>(this);
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

        public abstract void ReserveBuffers();

        public abstract JobHandle Schedule();

        public void Register(GizmoInstanceActivator activator)
        {
            activator.RegisterScheduler<TScheduler, TJobData>((TScheduler)this);
        }

        public void Clear()
        {
            jobData.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFromContinuous(in TJobData data)
        {
            jobData.Add(data);
        }

        public void Dispose()
        {
            jobData.Dispose();
        }
    }
}