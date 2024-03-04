using System.Runtime.CompilerServices;

namespace UGizmo
{
    public static class Gizmo<TRenderer, TJobData>
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
    }
}