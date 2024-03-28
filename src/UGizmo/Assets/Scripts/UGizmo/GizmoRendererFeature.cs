using System;
using System.Linq;
using UGizmo;
using UGizmo.Extension;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GizmoRendererFeature : ScriptableRendererFeature
{
    private NoResizableList<IGizmoUpdater> updaters;
    private GizmoRenderPass gizmoRenderPass;

    private bool isDisposed = true;

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

    public override void Create()
    {
        if (!isDisposed)
        {
            Dispose(true);
        }
        
        updaters = new NoResizableList<IGizmoUpdater>();
        RegisterAllElement();
        gizmoRenderPass = new GizmoRenderPass();
        gizmoRenderPass.SetUpdaters(updaters);
        isDisposed = false;
    }

    protected override void Dispose(bool disposing)
    {
        for (var i = 0; i < updaters.Count; i++)
        {
            updaters[i].Dispose();
        }

        gizmoRenderPass.Dispose();
        isDisposed = true;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(gizmoRenderPass);
    }
}