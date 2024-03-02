namespace UGizmos
{
    public interface IGizmoElement
    {
        void Register(GizmoDispatcher dispatcher);
        string MeshPath { get; }
        string MaterialPath { get; }
    }

    public abstract class GizmoElement<TObject, TCustom> : IGizmoElement
        where TObject : IGizmoElement, new()
        where TCustom : unmanaged
    {
        public abstract string MeshPath { get; }
        public abstract string MaterialPath { get; }

        public void Register(GizmoDispatcher dispatcher)
        {
            dispatcher.Register<TObject, TCustom>();
        }
    }
}