using System;
using System.Linq;
using UGizmo.Extension;
using UnityEngine;

namespace UGizmo
{
    public class GizmoInstanceActivator
    {
        private NoResizableList<IGizmoUpdater> updaters;
        
        public NoResizableList<IGizmoUpdater> CreateInstance()
        {
            updaters = new NoResizableList<IGizmoUpdater>();
            RegisterAllElement();

            return updaters;
        }
        
        private void RegisterAllElement()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var types in assemblies.Select(assembly => assembly.GetTypes()))
            {
                foreach (var type in types)
                {
                    if ((type.IsClass && type.IsAbstract) || !type.GetInterfaces().Contains(typeof(IGizmoCreator)))
                    {
                        continue;
                    }

                    var gizmoElement = (IGizmoCreator)Activator.CreateInstance(type);
                    gizmoElement.Create(this);
                }
            }
        }

        internal void Register<TRenderer, TJobData>(GizmoAsset<TRenderer, TJobData> asset)
            where TRenderer : GizmoRenderer<TJobData>, new()
            where TJobData : unmanaged
        {
            TRenderer gizmoRenderer = new TRenderer();
            (Mesh mesh, Material material) = AssetUtility.CreateMeshAndMaterial(asset.MeshName, asset.MaterialName);
            gizmoRenderer.Initialize(mesh, material);

            Gizmo<TRenderer, TJobData>.Initialize(gizmoRenderer);
            updaters.Add(gizmoRenderer);
        }
    }
}