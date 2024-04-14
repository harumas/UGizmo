using System;
using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace UGizmo
{
    public interface IPreparingJobScheduler
    {
        void Schedule();
        void Register(GizmoInstanceActivator activator);
        void Clear();
    }

    public abstract unsafe class PreparingJobScheduler<TPreparingScheduler, TPrepareData> : IPreparingJobScheduler
        where TPreparingScheduler : PreparingJobScheduler<TPreparingScheduler, TPrepareData>, new()
        where TPrepareData : unmanaged
    {
        protected TPrepareData[] JobData;
        protected int InstanceCount;
        private int maxInstanceCount = 8192;

        protected TPrepareData* JobDataPtr
        {
            get
            {
                fixed (TPrepareData* ptr = JobData)
                {
                    return ptr;
                }
            }
        }

        protected PreparingJobScheduler()
        {
            JobData = new TPrepareData[maxInstanceCount];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TPrepareData data)
        {
            if (InstanceCount >= JobData.Length)
            {
                return;
            }

            JobData[InstanceCount++] = data;
        }

        public abstract void Schedule();

        public void Register(GizmoInstanceActivator activator)
        {
            activator.RegisterScheduler<TPreparingScheduler, TPrepareData>();
        }

        public void Clear()
        {
            InstanceCount = 0;
        }
    }
}