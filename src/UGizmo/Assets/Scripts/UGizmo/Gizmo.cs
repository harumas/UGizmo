using System.Runtime.CompilerServices;

namespace UGizmo
{
    public static unsafe class Gizmo<TRenderer, TJobData>
        where TRenderer : GizmoRenderer<TJobData>
        where TJobData : unmanaged
    {
        private static TRenderer gizmoRenderer;

        public static void Initialize(TRenderer renderer)
        {
            gizmoRenderer = renderer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TJobData data)
        {
            gizmoRenderer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDataRange(TJobData* data, int length)
        {
            for (int i = 0; i < length; i++)
            {
                gizmoRenderer.Add(*(data + i));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TJobData* Reserve(int count)
        {
            return gizmoRenderer.Reserve(count);
        }
    }

    public static class PreparableGizmo<TPreparingJobScheduler, TPrepareData>
        where TPreparingJobScheduler : PreparingJobScheduler<TPreparingJobScheduler, TPrepareData>, new()
        where TPrepareData : unmanaged
    {
        private static TPreparingJobScheduler preparableGizmo;

        public static void Initialize(TPreparingJobScheduler gizmo)
        {
            preparableGizmo = gizmo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TPrepareData data)
        {
            preparableGizmo.Add(data);
        }
    }
}