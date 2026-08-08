using System;
using System.Collections.Generic;
using System.Reflection;
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

            AssetUtility.ApplyDepthTest(UGizmos.depthTest);
        }

        private void RegisterElements()
        {
            foreach (Assembly assembly in GetCandidateAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    // Only concrete types can be instantiated. Interfaces must be excluded here
                    // because IsAssignableFrom below also matches the interface type itself.
                    if (type.IsAbstract || type.IsInterface)
                    {
                        continue;
                    }

                    // IsAssignableFrom avoids the array GetInterfaces() allocates for every type.
                    if (typeof(IGizmoCreator).IsAssignableFrom(type))
                    {
                        var gizmoElement = (IGizmoCreator)Activator.CreateInstance(type);
                        gizmoElement.Create(this);
                    }
                    else if (typeof(IPreparingJobScheduler).IsAssignableFrom(type))
                    {
                        var preparingJobScheduler = (IPreparingJobScheduler)Activator.CreateInstance(type);
                        preparingJobScheduler.Register(this);
                    }
                    else if (typeof(IGizmoJobScheduler).IsAssignableFrom(type))
                    {
                        var jobScheduler = (IGizmoJobScheduler)Activator.CreateInstance(type);
                        JobSchedulers.Add(jobScheduler);
                    }
                }
            }
        }

        /// <summary>
        /// Scanning every loaded assembly is wasted work: implementing any of the three interfaces
        /// requires a compile-time reference to this assembly, so anything that does not reference
        /// UGizmo cannot contribute a type. In a real project this skips almost every assembly.
        /// </summary>
        private static IEnumerable<Assembly> GetCandidateAssemblies()
        {
            Assembly ugizmoAssembly = typeof(IGizmoCreator).Assembly;

            yield return ugizmoAssembly;

            string ugizmoName = ugizmoAssembly.GetName().Name;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly == ugizmoAssembly)
                {
                    continue;
                }

                foreach (AssemblyName referenced in assembly.GetReferencedAssemblies())
                {
                    if (referenced.Name == ugizmoName)
                    {
                        yield return assembly;

                        break;
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
            gizmoDrawer.Initialize(mesh, material, asset.MeshName, asset.MaterialName);

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

#if UNITY_EDITOR
            // Only relevant in the editor: with domain reload disabled, materials
            // instantiated at runtime are destroyed on exiting Play Mode while this
            // static cache survives, so it must be cleared explicitly. In a build,
            // Dispose runs on Application.quitting where the process is tearing down
            // anyway - destroying engine objects there risks the render loop touching
            // a destroyed material during its last frames.
            AssetUtility.Clear();
#endif
        }
    }
}