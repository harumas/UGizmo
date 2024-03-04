using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UGizmo.Extension.Generator
{
    public class WireLineGenerator
    {
        [MenuItem("Assets/Create/UGizmos/WireLine")]
        public static void Generate()
        {
            Mesh lineMesh = new Mesh()
            {
                name = "WireLine"
            };
            Vector3[] vertices = new Vector3[2]
            {
                new Vector3(0, 0, 0.5f),
                new Vector3(0, 0, -0.5f)
            };

            List<int> indices = new List<int>()
            {
                0, 1
            };

            lineMesh.SetVertices(vertices);
            lineMesh.SetIndices(indices, MeshTopology.Lines, 0, true);
            lineMesh.Optimize();
            lineMesh.UploadMeshData(true);

            AssetUtility.CreateMeshAsset(lineMesh);
        }
    }
}