using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UGizmo.Extension
{
    public static class AssetUtility
    {
        public static void CreateMeshAsset(Mesh mesh)
        {
            foreach (Object obj in Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.DeepAssets))
            {
                if (obj is not DefaultAsset) continue;

                string path = AssetDatabase.GetAssetPath(obj);

                if (AssetDatabase.IsValidFolder(path))
                {
                    AssetDatabase.CreateAsset(mesh, path + $"/{mesh.name}.asset");
                    AssetDatabase.SaveAssets();
                    Debug.Log($"{mesh.name} Mesh has been generated.");
                }
            }
        }

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