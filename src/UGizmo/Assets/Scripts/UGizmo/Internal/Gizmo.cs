using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal static unsafe class Gizmo<TRenderer, TJobData>
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
            gizmoRenderer.AddRange(data, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TJobData* Reserve(int count)
        {
            return gizmoRenderer.Reserve(count);
        }
    }

}