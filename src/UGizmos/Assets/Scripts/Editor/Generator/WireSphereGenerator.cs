using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.Generator
{
    public class WireSphereGenerator
    {
        private const int PointCount = 64;

        [MenuItem("Assets/Create/UGizmos/WireSphere")]
        public static void Generate()
        {
            Mesh sphereMesh = new Mesh()
            {
                name = "WireSphere"
            };

            Vector3[] vertices = new Vector3[PointCount * 3];
            List<int> indices = new List<int>();

            const float oneCycle = 2.0f * Mathf.PI;
            Quaternion rotY = Quaternion.Euler(0, 90, 0);
            Quaternion rotX = Quaternion.Euler(90, 0, 0);

            for (var i = 0; i < PointCount; ++i)
            {
                float point = (float)i / PointCount * oneCycle;

                float x = Mathf.Cos(point);
                float y = Mathf.Sin(point);

                Vector3 position = new Vector3(x, y);
                vertices[i] = position;
                vertices[i + PointCount] = rotY * position;
                vertices[i + PointCount * 2] = rotX * position;
            }

            indices.Add(0);
            indices.Add(PointCount - 1);

            indices.Add(PointCount);
            indices.Add(PointCount * 2 - 1);

            indices.Add(PointCount * 2);
            indices.Add(PointCount * 3 - 1);

            for (var i = 0; i < PointCount - 1; ++i)
            {
                indices.Add(i);
                indices.Add(i + 1);

                indices.Add(i + PointCount);
                indices.Add(i + 1 + PointCount);

                indices.Add(i + PointCount * 2);
                indices.Add(i + 1 + PointCount * 2);
            }

            sphereMesh.SetVertices(vertices);
            sphereMesh.SetIndices(indices, MeshTopology.Lines, 0, true);
            sphereMesh.Optimize();
            sphereMesh.UploadMeshData(true);

            GeneratorUtil.CreateMeshAsset(sphereMesh);
        }
    }
}