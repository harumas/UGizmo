using System;
using System.IO;
using System.Linq;
using UGizmo.Extension;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace UGizmo
{
    [ExecuteAlways]
    [DefaultExecutionOrder(-1000)]
    public class GizmoDispatcher : MonoBehaviour
    {
        private NoResizableList<IGizmoUpdater> updaters;
        private event Action DisposeEvents;
        private NativeArray<JobHandle> jobHandles;

        [NonSerialized] private bool isFirstFrame = true;

        private bool UseDomainReload()
        {
            return !EditorSettings.enterPlayModeOptionsEnabled ||
                   (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableDomainReload) == 0;
        }

        private void Initialize()
        {
            int capacity = 64;
            updaters = new NoResizableList<IGizmoUpdater>(capacity);
            jobHandles = new NativeArray<JobHandle>(capacity, Allocator.Persistent);

            RegisterAllElement();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            isFirstFrame = false;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (UseDomainReload())
                {
                    Dispose();
                }

                isFirstFrame = true;
            }
            else if (UseDomainReload() && state == PlayModeStateChange.ExitingEditMode)
            {
                Dispose();
            }
        }

        private void RegisterAllElement()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var types in assemblies.Select(assembly => assembly.GetTypes()))
            {
                foreach (var type in types)
                {
                    if ((type.IsClass && type.IsAbstract) || !type.GetInterfaces().Contains(typeof(IGizmoCreator))) continue;

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

            DisposeEvents += gizmoRenderer.Dispose;
        }

        public void Dispose()
        {
            if (jobHandles.IsCreated)
            {
                jobHandles.Dispose();
            }

            DisposeEvents?.Invoke();
            DisposeEvents = null;
            updaters.Clear();
            isFirstFrame = true;
        }

        private void OnRenderObject()
        {
            if (isFirstFrame)
            {
                Initialize();
            }
        }

        private void OnDrawGizmos()
        {
            Profiler.BeginSample("UGizmo");

            if (!UseDomainReload() && isFirstFrame)
            {
                Initialize();
            }

            var updaterSpan = updaters.AsSpan();

            int i = 0;
            foreach (var updater in updaterSpan)
            {
                jobHandles[i++] = updater.CreateJobHandle();
            }

            JobHandle createDataJob = JobHandle.CombineDependencies(jobHandles.Slice(0, updaterSpan.Length));
            createDataJob.Complete();

            foreach (var updater in updaterSpan)
            {
                updater.Render();
            }

            Profiler.EndSample();
        }
    }
}