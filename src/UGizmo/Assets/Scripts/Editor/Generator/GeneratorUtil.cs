using UnityEditor;
using UnityEngine;

namespace Editor.Generator
{
    public class GeneratorUtil
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
    }
}