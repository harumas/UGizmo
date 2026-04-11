using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    /// <summary>
    /// Class for static access to GizmoDrawer.
    /// </summary>
    /// <typeparam name="TDrawer"></typeparam>
    /// <typeparam name="TJobData"></typeparam>
    internal static unsafe class Gizmo<TDrawer, TJobData>
        where TDrawer : GizmoDrawer<TJobData>
        where TJobData : unmanaged
    {
        private static TDrawer gizmoDrawer;

        public static void Initialize(TDrawer drawer)
        {
            gizmoDrawer = drawer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TJobData data, float duration)
        {
            if (!UGizmos.enabled)
            {
                return;
            }

            gizmoDrawer.Add(data, duration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDataRange(TJobData* data, int length, float duration)
        {
            if (!UGizmos.enabled)
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