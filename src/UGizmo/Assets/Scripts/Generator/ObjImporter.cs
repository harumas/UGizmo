using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Generator
{
    public static class ObjImporter
    {
        public static Vector3[] Vertex;
        private static readonly string filePath = Application.dataPath + "/Scripts/UGizmo/Resources/UGizmo/Meshes/WireCupsule2D.txt";

        [MenuItem("Assets/Create/UGizmos/WireHemisphere")]
        private static void Import()
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("File does not exist: " + filePath);
                return;
            }

            Mesh sphereMesh = new Mesh()
            {
                name = "WireSemicircle"
            };

            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (reader.ReadLine() is { } line)
                {
                    string[] parts = line.Split(' ');

                    if (parts[0] == "v")
                    {
                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);
                        float z = float.Parse(parts[3]);
                        vertices.Add(new Vector3(x, y, z));
                    }
                    else if (parts[0] == "l")
                    {
                        int a = int.Parse(parts[1]);
                        int b = int.Parse(parts[2]);
                        indices.Add(a - 1);
                        indices.Add(b - 1);
                    }

                    // Vertex
                }
            }

            sphereMesh.SetVertices(vertices);
            sphereMesh.SetIndices(indices, MeshTopology.Lines, 0, true);
            sphereMesh.Optimize();
            sphereMesh.UploadMeshData(true);

            GeneratorUtil.CreateMeshAsset(sphereMesh);
        }
    }
}