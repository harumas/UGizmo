using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal static unsafe class Gizmo<TDrawer, TJobData>
        where TDrawer : GizmoDrawer<TJobData>
        where TJobData : unmanaged
    {
        private static TDrawer gizmoDrawer;
        private static bool isInitialized;

        public static void Initialize(TDrawer drawer)
        {
            isInitialized = true;
            gizmoDrawer = drawer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TJobData data, float duration)
        {
            if (!isInitialized)
            {
                return;
            }

            gizmoDrawer.Add(data, duration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDataRange(TJobData* data, int length, float duration)
        {
            if (!isInitialized)
            {
                return;
            }

            gizmoDrawer.AddRange(data, length, duration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TJobData* Reserve(int count)
        {
            return gizmoDrawer.Reserve(count);
        }
    }
}