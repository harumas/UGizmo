using System;
using UGizmo.Internal.Extension.Gizmo;
using UGizmo.Internal.Utility;
using UnityEngine;

namespace UGizmo.Internal
{
    internal class GizmoInstanceActivator : IDisposable
    {
        public NoResizableList<IGizmoDrawer> Drawers { get; private set; }
        public NoResizableList<IPreparingJobScheduler> PreparingJobSchedulers { get; private set; }
        public NoResizableList<IGizmoJobScheduler> JobSchedulers { get; private set; }

        public void Activate()
        {
            Drawers = new NoResizableList<IGizmoDrawer>();
            PreparingJobSchedulers = new NoResizableList<IPreparingJobScheduler>();
            JobSchedulers = new NoResizableList<IGizmoJobScheduler>();

            RegisterElements();

            var sortedArray = Drawers.AsSpan().ToArray();
            Array.Sort(sortedArray, (a, b) => a.RenderQueue.CompareTo(b.RenderQueue));
            Drawers.SetArray(sortedArray);
        }

        private void RegisterElements()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && type.IsAbstract)
                    {
                        continue;
                    }

                    Type[] interfaces = type.GetInterfaces();
                    
                    if (Array.IndexOf(interfaces, typeof(IGizmoCreator)) >= 0)
                    {
                        var gizmoElement = (IGizmoCreator)Activator.CreateInstance(type);
                        gizmoElement.Create(this);
                    }
                    else if (Array.IndexOf(interfaces, typeof(IPreparingJobScheduler)) >= 0)
                    {
                        var preparingJobScheduler = (IPreparingJobScheduler)Activator.CreateInstance(type);
                        preparingJobScheduler.Register(this);
                    }
                    else if (Array.IndexOf(interfaces, typeof(IGizmoJobScheduler)) >= 0)
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