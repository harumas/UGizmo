using UnityEditor;
using UnityEngine;

namespace Editor.Generator
{
    public class CubeGenerator
    {
        [MenuItem("Assets/Create/UGizmos/Cube")]
        public static void Generate()
        {
            Mesh cubeMesh = new Mesh()
            {
                name = "Cube"
            };

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),
            };

            int[] indices = new int[]
            {
                0, 1, 1, 2, 2, 3, 3, 0, 0, 4, 1, 5, 2, 6, 3, 7, 4, 5, 5, 6, 6, 7, 7, 4
            };

            cubeMesh.SetVertices(vertices);
            cubeMesh.SetIndices(indices, MeshTopology.Lines, 0, true);
            cubeMesh.Optimize();
            cubeMesh.UploadMeshData(true);

            GeneratorUtil.CreateMeshAsset(cubeMesh);
        }
    }
}