using System.Runtime.CompilerServices;
using Unity.Jobs;
using UnityEngine;

namespace UGizmos
{
    public static class Gizmo<TObject, TCustom>
        where TObject : IGizmoElement, new()
        where TCustom : unmanaged
    {
        private static GizmoRenderer<TCustom> renderer;

        private const int MaxInstanceCount = 8192;

        public static void Initialize()
        {
            var objectData = new TObject();
            Mesh mesh = Resources.Load<Mesh>(objectData.MeshPath);
            Material material = Resources.Load<Material>(objectData.MaterialPath);

            if (mesh == null)
            {
                Debug.LogError("Mesh is null!");
                return;
            }

            if (material == null)
            {
                Debug.LogError("Material is null!");
                return;
            }

            renderer = new GizmoRenderer<TCustom>(mesh, material, MaxInstanceCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in GizmoData<TCustom> data)
        {
            renderer.Add(data);
        }

        public static void Dispose()
        {
            renderer.Dispose();
        }

        public static IGizmoUpdater GetUpdater()
        {
            return renderer;
        }
    }
}