using System;
using System.Linq;
using UGizmo.Internal.Extension.Gizmo;
using UGizmo.Internal.Utility;
using UnityEngine;

namespace UGizmo.Internal
{
    internal class GizmoInstanceActivator : IDisposable
    {
        public NoResizableList<IGizmoDrawer> Drawers;
        public NoResizableList<IPreparingJobScheduler> PreparingJobSchedulers;
        public NoResizableList<IGizmoJobScheduler> JobSchedulers;

        public void Activate()
        {
            Drawers = new NoResizableList<IGizmoDrawer>();
            PreparingJobSchedulers = new NoResizableList<IPreparingJobScheduler>();
            JobSchedulers = new NoResizableList<IGizmoJobScheduler>();

            RegisterElements();

            var sortedArray = Drawers.Take(Drawers.Count).OrderBy(renderer => renderer.RenderQueue).ToArray();
            Drawers.SetArray(sortedArray);
        }

        private void RegisterElements()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var types in assemblies.Select(assembly => assembly.GetTypes()))
            {
                foreach (var type in types)
                {
                    if (type.IsClass && type.IsAbstract)
                    {
                        continue;
                    }

                    Type[] interfaces = type.GetInterfaces();
                    
                    if (interfaces.Contains(typeof(IGizmoCreator)))
                    {
                        var gizmoElement = (IGizmoCreator)Activator.CreateInstance(type);
                        gizmoElement.Create(this);
                    }
                    else if (interfaces.Contains(typeof(IPreparingJobScheduler)))
                    {
                        var preparingJobScheduler = (IPreparingJobScheduler)Activator.CreateInstance(type);
                        preparingJobScheduler.Register(this);
                    }
                    else if (interfaces.Contains(typeof(IGizmoJobScheduler)))
                    {
                        var jobScheduler = (IGizmoJobScheduler)Activator.CreateInstance(type);
                        JobSchedulers.Add(jobScheduler);
                    }
                }
            }
        }

        internal void RegisterDrawer<TDrawer, TJobData>(GizmoAsset<TDrawer, TJobData> asset)
            where TDrawer : GizmoDrawer<TJobData>, new()
            where TJobData : unmanaged
        {
            TDrawer gizmoDrawer = new TDrawer();
            (Mesh mesh, Material material) = AssetUtility.CreateMeshAndMaterial(asset.MeshName, asset.MaterialName);
            gizmoDrawer.Initialize(mesh, material);

            Gizmo<TDrawer, TJobData>.Initialize(gizmoDrawer);
            Drawers.Add(gizmoDrawer);
        }

        internal void RegisterScheduler<TScheduler, TJobData>(TScheduler scheduler)
            where TScheduler : PreparingJobScheduler<TScheduler, TJobData>, new()
            where TJobData : unmanaged
        {
            PreparableGizmo<TScheduler, TJobData>.Initialize(scheduler);
            PreparingJobSchedulers.Add(scheduler);
        }

        public void Dispose()
        {
            foreach (var drawer in Drawers.AsSpan())
            {
                drawer.Dispose();
            }

            foreach (var jobScheduler in JobSchedulers.AsSpan())
            {
                jobScheduler.Dispose();
            }

            foreach (var jobScheduler in PreparingJobSchedulers.AsSpan())
            {
                jobScheduler.Dispose();
            }
        }
    }
}