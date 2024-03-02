using System;
using UnityEngine;

namespace UGizmos
{
    public interface IGizmoCreator
    {
        void Create(GizmoDispatcher dispatcher);
    }

    public abstract class GizmoElement<TRenderer, TJobData> : IGizmoCreator
        where TRenderer : GizmoRenderer<TJobData>
        where TJobData : unmanaged
    {
        public abstract string MeshPath { get; }
        public abstract string MaterialPath { get; }

        protected abstract TRenderer CreateInstance();

        private (Mesh mesh, Material material) CreateMeshAndMaterial()
        {
            Mesh mesh = Resources.Load<Mesh>(MeshPath);
            Material material = Resources.Load<Material>(MaterialPath);

            if (mesh == null)
            {
                Debug.LogError("Mesh is null!");
                throw new ArgumentNullException(nameof(mesh));
            }

            if (material == null)
            {
                Debug.LogError("Material is null!");
                throw new ArgumentNullException(nameof(material));
            }

            return (mesh, material);
        }

        public void Create(GizmoDispatcher dispatcher)
        {
            TRenderer renderer = CreateInstance();

            (Mesh mesh, Material material) = CreateMeshAndMaterial();
            renderer.Initialize(mesh, material);
            dispatcher.Register<TRenderer, TJobData>(renderer);
        }
    }
}