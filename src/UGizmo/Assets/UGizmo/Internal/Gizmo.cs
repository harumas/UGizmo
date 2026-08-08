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
        private static bool isInitialized = false;

        public static void Initialize(TDrawer drawer)
        {
            gizmoDrawer = drawer;
            isInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TJobData data, float duration)
        {
            if (!UGizmos.enabled || !isInitialized)
            {
                return;
            }

            gizmoDrawer.Add(data, duration);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDataRange(TJobData* data, int length, float duration)
        {
            if (!UGizmos.enabled || !isInitialized)
            {
                return;
            }

            gizmoDrawer.AddRange(data, length, duration);
        }

        /// <summary>
        /// Reserves a range and returns its start index. Call during the reservation phase, then
        /// resolve with <see cref="GetReservedPointer"/> in the scheduling phase - reserving grows
        /// a shared buffer that can reallocate, which would invalidate an earlier pointer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Reserve(int count)
        {
            return gizmoDrawer.Reserve(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TJobData* GetReservedPointer(int start)
        {
            return gizmoDrawer.GetReservedPointer(start);
        }
    }
}