using System;
using System.Linq;
using UGizmo.Extension;
using UnityEngine;

namespace UGizmo
{
    public class GizmoInstanceActivator : IDisposable
    {
        public NoResizableList<IGizmoRenderer> Updaters;
        public NoResizableList<IPreparingJobScheduler> PreparingJobSchedulers;
        public NoResizableList<IGizmoJobScheduler> JobSchedulers;

        public void Activate()
        {
            Updaters = new NoResizableList<IGizmoRenderer>();
            PreparingJobSchedulers = new NoResizableList<IPreparingJobScheduler>();
            JobSchedulers = new NoResizableList<IGizmoJobScheduler>();

            RegisterElements();

            var sortedArray = Updaters.Take(Updaters.Count).OrderBy(renderer => renderer.RenderQueue).ToArray();
            Updaters.SetArray(sortedArray);
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

        internal void RegisterRenderer<TRenderer, TJobData>(GizmoAsset<TRenderer, TJobData> asset)
            where TRenderer : GizmoRenderer<TJobData>, new()
            where TJobData : unmanaged
        {
            TRenderer gizmoRenderer = new TRenderer();
            (Mesh mesh, Material material) = AssetUtility.CreateMeshAndMaterial(asset.MeshName, asset.MaterialName);
            gizmoRenderer.Initialize(mesh, material);

            Gizmo<TRenderer, TJobData>.Initialize(gizmoRenderer);
            Updaters.Add(gizmoRenderer);
        }

        internal void RegisterScheduler<TPreparingScheduler, TPrepareData>()
            where TPreparingScheduler : PreparingJobScheduler<TPreparingScheduler, TPrepareData>, new()
            where TPrepareData : unmanaged
        {
            TPreparingScheduler preparableGizmo = new TPreparingScheduler();
            PreparableGizmo<TPreparingScheduler, TPrepareData>.Initialize(preparableGizmo);
            PreparingJobSchedulers.Add(preparableGizmo);
        }

        public void Dispose()
        {
            foreach (var gizmoRenderer in Updaters.AsSpan())
            {
                gizmoRenderer.Dispose();
            }

            foreach (var jobScheduler in JobSchedulers.AsSpan())
            {
                jobScheduler.Dispose();
            }
        }
    }
}