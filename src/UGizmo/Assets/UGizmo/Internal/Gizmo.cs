using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal static unsafe class Gizmo<TRenderer, TJobData>
        where TRenderer : GizmoRenderer<TJobData>
        where TJobData : unmanaged
    {
        private static GizmoRenderBuffer<TJobData> gizmoBuffer;

        public static void Initialize(TRenderer renderer)
        {
            gizmoBuffer = renderer.RenderBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TJobData data)
        {
            gizmoBuffer.Add(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddDataRange(TJobData* data, int length)
        {
            gizmoBuffer.AddRange(data, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TJobData* Reserve(int count)
        {
            return gizmoBuffer.Reserve(count);
        }
    }
}