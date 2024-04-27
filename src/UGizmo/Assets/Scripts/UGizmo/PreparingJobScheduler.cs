using System.Runtime.CompilerServices;
using Unity.Jobs;

namespace UGizmo
{
    public interface IPreparingJobScheduler
    {
        void Register(GizmoInstanceActivator activator);
        JobHandle Schedule();
        void Clear();
    }

    public abstract unsafe class PreparingJobScheduler<TPreparingScheduler, TPrepareData> : IPreparingJobScheduler
        where TPreparingScheduler : PreparingJobScheduler<TPreparingScheduler, TPrepareData>, new()
        where TPrepareData : unmanaged
    {
        private readonly TPrepareData[] jobData;
        protected int InstanceCount;
        private int maxInstanceCount = 8192;

        protected TPrepareData* JobDataPtr
        {
            get
            {
                fixed (TPrepareData* ptr = jobData)
                {
                    return ptr;
                }
            }
        }

        protected PreparingJobScheduler()
        {
            jobData = new TPrepareData[maxInstanceCount];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in TPrepareData data)
        {
            jobData[InstanceCount++] = data;
        }

        public abstract JobHandle Schedule();

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