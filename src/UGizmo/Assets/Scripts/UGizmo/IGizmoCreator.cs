using System;
using UnityEngine;

namespace UGizmo
{
    public interface IGizmoCreator
    {
        void Create(GizmoInstanceActivator dispatcher);
    }

    public abstract class GizmoAsset<TRenderer, TJobData> : IGizmoCreator
        where TRenderer : GizmoRenderer<TJobData>, new()
        where TJobData : unmanaged
    {
        public abstract string MeshName { get; }
        public abstract string MaterialName { get; }

        public void Create(GizmoInstanceActivator activator)
        {
            activator.Register<TRenderer, TJobData>(this);
        }
    }
}