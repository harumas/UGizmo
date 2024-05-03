using System.Runtime.CompilerServices;
using UnityEngine;

namespace UGizmo.Internal
{
    internal static unsafe class Gizmo<TRenderer, TJobData>
        where TRenderer : GizmoRenderer<TJobData>
        where TJobData : unmanaged
    {
        private static GizmoRenderBuffer<TJobData> gizmoBuffer;
        private static bool isInitialized;

        public static void Initialize(TRenderer renderer)
        {
            gizmoBuffer = renderer.RenderBuffer;
            isInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TJobData data)
        {
            if (!isInitialized)
            {
                return;
            }

            gizmoBuffer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDataRange(TJobData* data, int length)
        {
            if (!isInitialized)
            {
                return;
            }
            
            gizmoBuffer?.AddRange(data, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TJobData* Reserve(int count)
        {
            return gizmoBuffer.Reserve(count);
        }
    }
}