namespace UGizmo.Internal
{
    internal interface IGizmoCreator
    {
        void Create(GizmoInstanceActivator dispatcher);
    }

    internal abstract class GizmoAsset<TDrawer, TJobData> : IGizmoCreator
        where TDrawer : GizmoDrawer<TJobData>, new()
        where TJobData : unmanaged
    {
        public abstract string MeshName { get; }
        public abstract string MaterialName { get; }

        public void Create(GizmoInstanceActivator activator)
        {
            activator.RegisterDrawer<TDrawer, TJobData>(this);
        }
    }
}