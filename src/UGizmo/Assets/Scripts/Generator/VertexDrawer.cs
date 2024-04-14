using System;
using UGizmo;
using UnityEngine;

namespace Editor.Generator
{
    public class VertexDrawer : MonoBehaviour
    {
        [SerializeField] [Range(0, 48)] private int count;

        private void OnDrawGizmos()
        {
            Color color = Color.black;
            for (var index = 0; index < count + 1; index++)
            {
                var vertex = ObjImporter.Vertex[index];
                color += new Color(0.001f, 0.001f, 0.001f, 0f);
                UGizmos.DrawPoint(vertex, 0.05f, color);
            }
        }
    }
}