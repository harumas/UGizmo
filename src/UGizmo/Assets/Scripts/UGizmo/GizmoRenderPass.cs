using System;
using System.Runtime.InteropServices;
using UGizmo;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public readonly struct RenderData
{
    public readonly float4x4 Matrix;
    public readonly Color Color;

    public RenderData(in float4x4 matrix, in Color color)
    {
        this.Matrix = matrix;
        this.Color = color;
    }
}

public class GizmoRenderPass : ScriptableRenderPass
{
    private NoResizableList<IGizmoUpdater> updaters;
    private NativeArray<JobHandle> jobHandles;

    public GizmoRenderPass()
    {
        renderPassEvent = RenderPassEvent.AfterRendering;
        jobHandles = new NativeArray<JobHandle>(64, Allocator.Persistent);
    }

    public void SetUpdaters(NoResizableList<IGizmoUpdater> updaters)
    {
        this.updaters = updaters;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (updaters.Count == 0)
        {
            return;
        }
        
        CommandBuffer cmd = CommandBufferPool.Get();

        var updaterSpan = updaters.AsSpan();

        int i = 0;
        foreach (var updater in updaterSpan)
        {
            jobHandles[i++] = updater.CreateJobHandle(1);
        }

        JobHandle createDataJob = JobHandle.CombineDependencies(jobHandles.Slice(0, updaterSpan.Length));
        createDataJob.Complete();

        foreach (IGizmoUpdater updater in updaters.AsSpan())
        {
            updater.Render(cmd);
        }

        context.ExecuteCommandBuffer(cmd);

        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        jobHandles.Dispose();
    }
}