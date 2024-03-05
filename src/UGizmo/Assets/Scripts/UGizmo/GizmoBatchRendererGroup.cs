using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo
{
    [BurstCompile]
    public sealed unsafe class GizmoBatchRendererGroup
    {
        private readonly BatchRendererGroup batchRendererGroup;
        private readonly GraphicsBuffer gpuPersistentData;
        private BatchID batchID;
        private BatchMaterialID materialID;
        private BatchMeshID meshID;
        private NativeArray<float4> systemBuffer;

        private int maxInstanceCount;
        private int instanceCount;
        private bool initialized;

        public GizmoBatchRendererGroup(Mesh mesh, Material material, int maxInstanceCount)
        {
            batchRendererGroup = new BatchRendererGroup(OnPerformCulling, IntPtr.Zero);
            this.maxInstanceCount = maxInstanceCount;

            int instanceSize = sizeof(float3x4) * 2 + sizeof(float4);
            int bufferSize = this.maxInstanceCount * instanceSize;

            gpuPersistentData = new GraphicsBuffer(GraphicsBuffer.Target.Raw, bufferSize / sizeof(float), sizeof(float));

            // Create system memory copy of big GPU raw buffer
            systemBuffer = new NativeArray<float4>(bufferSize / sizeof(float4), Allocator.Persistent);

            RegisterMetadata();

            // Setup very large bound to be sure BRG is never culled
            var bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(1048576.0f, 1048576.0f, 1048576.0f));
            batchRendererGroup.SetGlobalBounds(bounds);

            // Register mesh and material
            meshID = batchRendererGroup.RegisterMesh(mesh);
            materialID = batchRendererGroup.RegisterMaterial(material);

            initialized = true;
        }

        private void RegisterMetadata()
        {
            // Batch metadata buffer
            int objectToWorldID = Shader.PropertyToID("unity_ObjectToWorld");
            int worldToObjectID = Shader.PropertyToID("unity_WorldToObject");
            int colorID = Shader.PropertyToID("_BaseColor");

            // In our sample game we're dealing with 3 instanced properties: obj2world, world2obj and baseColor
            var batchMetadata = new NativeArray<MetadataValue>(3, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            int gpuOffset = 0;
            batchMetadata[0] = CreateMetadataValue(objectToWorldID, 0); // matrices

            gpuOffset += maxInstanceCount * sizeof(float3x4);
            batchMetadata[1] = CreateMetadataValue(worldToObjectID, gpuOffset); // inverse matrices

            gpuOffset += maxInstanceCount * sizeof(float3x4);
            batchMetadata[2] = CreateMetadataValue(colorID, gpuOffset); // colors

            batchID = batchRendererGroup.AddBatch(batchMetadata, gpuPersistentData.bufferHandle);

            batchMetadata.Dispose();
            return;

            MetadataValue CreateMetadataValue(int nameID, int gpuOffset)
            {
                const uint kIsPerInstanceBit = 0x80000000;
                return new MetadataValue
                {
                    NameID = nameID,
                    Value = (uint)gpuOffset | kIsPerInstanceBit,
                };
            }
        }

        [BurstCompile]
        public void UploadGpuData(int instanceCount)
        {
            this.instanceCount = instanceCount;

            int worldToObjectOffset = maxInstanceCount * 3;
            int colorOffset = maxInstanceCount * 6;

            gpuPersistentData.SetData(systemBuffer, 0, 0, instanceCount * 3);
            gpuPersistentData.SetData(systemBuffer, worldToObjectOffset, worldToObjectOffset, instanceCount * 3);
            gpuPersistentData.SetData(systemBuffer, colorOffset, colorOffset, instanceCount);
        }

        public Span<float4> GetBuffer()
        {
            return systemBuffer.AsSpan();
        }

        private JobHandle OnPerformCulling(
            BatchRendererGroup rendererGroup,
            BatchCullingContext cullingContext,
            BatchCullingOutput cullingOutput,
            IntPtr userContext)
        {
            if (!initialized)
            {
                return new JobHandle();
            }

            Camera camera = SceneView.currentDrawingSceneView != null ? SceneView.currentDrawingSceneView.camera : null;
            bool isSceneCamera = camera != null && camera.GetInstanceID() == cullingContext.viewID.GetInstanceID();

            if (!isSceneCamera && !GameViewUtility.IsShowingGizmos())
            {
                return new JobHandle();
            }

            int drawCommandCount = 1;

            BatchCullingOutputDrawCommands* drawCommands = (BatchCullingOutputDrawCommands*)cullingOutput.drawCommands.GetUnsafePtr();

            drawCommands->drawCommandCount = drawCommandCount;
            drawCommands->drawRangeCount = 1;
            drawCommands->drawRanges = Malloc<BatchDrawRange>(1);
            drawCommands->drawRanges[0] = new BatchDrawRange
            {
                drawCommandsBegin = 0,
                drawCommandsCount = (uint)drawCommandCount,
                filterSettings = new BatchFilterSettings
                {
                    renderingLayerMask = 1,
                    layer = 0,
                    shadowCastingMode = ShadowCastingMode.Off,
                    receiveShadows = false,
                    staticShadowCaster = false,
                    allDepthSorted = false
                }
            };

            drawCommands->visibleInstances = Malloc<int>(instanceCount);
            for (var i = 0; i < instanceCount; i++)
            {
                drawCommands->visibleInstances[i] = i;
            }

            drawCommands->visibleInstanceCount = instanceCount;

            drawCommands->drawCommands = Malloc<BatchDrawCommand>(drawCommandCount);
            drawCommands->drawCommands[0] = new BatchDrawCommand
            {
                visibleOffset = 0,
                visibleCount = (uint)instanceCount,
                batchID = batchID,
                materialID = materialID,
                meshID = meshID,
                submeshIndex = 0,
                splitVisibilityMask = 0xff,
                sortingPosition = 0
            };

            drawCommands->instanceSortingPositions = null;
            drawCommands->instanceSortingPositionFloatCount = 0;

            return new JobHandle();
        }

        private static U* Malloc<U>(int count) where U : unmanaged
        {
            return (U*)UnsafeUtility.Malloc(
                UnsafeUtility.SizeOf<U>() * count,
                UnsafeUtility.AlignOf<U>(),
                Allocator.TempJob);
        }

        public void Dispose()
        {
            if (!initialized)
            {
                return;
            }

            initialized = false;

            batchRendererGroup.RemoveBatch(batchID);
            batchRendererGroup.UnregisterMaterial(materialID);
            batchRendererGroup.UnregisterMesh(meshID);
            batchRendererGroup.Dispose();

            systemBuffer.Dispose();
            gpuPersistentData.Dispose();
        }
    }
}