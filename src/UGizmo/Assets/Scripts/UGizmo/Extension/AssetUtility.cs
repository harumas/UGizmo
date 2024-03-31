using System;
using System.IO;
using UnityEngine;

namespace UGizmo.Extension
{
    
    public static class AssetUtility
    {
        internal static (Mesh mesh, Material material) CreateMeshAndMaterial(string meshPath, string materialPath)
        {
            const string rootPath = "UGizmo";
            Mesh mesh = Resources.Load<Mesh>(Path.Combine(rootPath, "Meshes", meshPath));
            Material material = Resources.Load<Material>(Path.Combine(rootPath, "Materials", materialPath));

            if (mesh == null)
            {
                Debug.LogError("Mesh is null!");
                throw new ArgumentNullException(nameof(mesh));
            }

            if (material == null)
            {
                Debug.LogError("Material is null!");
                throw new ArgumentNullException(nameof(material));
            }

            return (mesh, material);
        }
    }
}