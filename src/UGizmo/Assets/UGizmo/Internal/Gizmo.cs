using System.Runtime.CompilerServices;
using UnityEngine;

namespace UGizmo.Internal
{
    internal static unsafe class Gizmo<TRenderer, TJobData>
        where TRenderer : GizmoDrawer<TJobData>
        where TJobData : unmanaged
    {
        private static TRenderer gizmoRenderer;
        private static bool isInitialized;

        public static void Initialize(TRenderer renderer)
        {
            isInitialized = true;
            gizmoRenderer = renderer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TJobData data)
        {
            if (!isInitialized)
            {
                return;
            }

            gizmoRenderer.GizmoDrawBuffer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDataRange(TJobData* data, int length)
        {
            if (!isInitialized)
            {
                return;
            }

            gizmoRenderer.GizmoDrawBuffer.AddRange(data, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TJobData* Reserve(int count)
        {
            return gizmoRenderer.GizmoDrawBuffer.Reserve(count);
        }
    }
}