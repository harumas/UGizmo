using System;
using UnityEngine;

namespace UGizmo
{
    public interface IGizmoCreator
    {
        void Create(GizmoDispatcher dispatcher);
    }

    public abstract class GizmoAsset<TRenderer, TJobData> : IGizmoCreator
        where TRenderer : GizmoRenderer<TJobData>, new()
        where TJobData : unmanaged
    {
        public abstract string MeshName { get; }
        public abstract string MaterialName { get; }

        public void Create(GizmoDispatcher dispatcher)
        {
            dispatcher.Register<TRenderer, TJobData>(this);
        }
    }
}