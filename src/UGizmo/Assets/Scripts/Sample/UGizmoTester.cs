using System.Collections.Generic;
using UnityEngine;
using UGizmo;
using UnityEditor;
using Random = UnityEngine.Random;

public class UGizmoTester : MonoBehaviour
{
    private List<Vector3> positions;
    private List<float> radius;
    private List<Color> colors;

    private List<Vector3> points;
    private List<Vector3> to;

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [SerializeField] private float width;

    private void OnValidate()
    {
        colors = new List<Color>();
        radius = new List<float>();
        positions = new List<Vector3>();
        points = new List<Vector3>();

        for (int i = 0; i < 5000; i++)
        {
            positions.Add(Random.insideUnitSphere * 50f);
            radius.Add(Random.Range(1f, 10f));
            colors.Add(Random.ColorHSV(0.5f, 1f));
        }

        for (int i = 0; i < 1000; i++)
        {
            points.Add(Random.insideUnitSphere * 50f);
        }
    }

    private void OnDrawGizmos()
    {
        // for (int i = 0; i < positions.Count; i++)
        // {
        //     UGizmos.DrawWireCube(positions[i], radius[i], colors[i]);
        // }
        //

        // UGizmos.DrawWirePlane(transform.position, Vector3.forward, new Vector2(3f, 3f), Color.red);
        UGizmos.DrawCircle2D(transform.position, 1f, Color.red);

        Handles.Label(transform.position, "Text");


        // UGizmos.DrawLine(Vector3.zero, Vector3.up * 10f);
        // Vector3 before = Vector3.zero;
        // for (var i = 0; i < points.Count; i++)
        // {
        //     var point = points[i];
        //     UGizmos.DrawLine(before, point, colors[i]);
        //     before = point;
        // }

        // Vector3 before = Vector3.zero;
        // for (var i = 0; i < points.Count; i++)
        // {
        //     var point = points[i];
        //     Gizmos.color =  colors[i];
        //     Gizmos.DrawLine(before, point);
        //     before = point;
        // }

        // for (int i = 0; i < positions.Count; i++)
        // {
        //     Gizmos.color = colors[i];
        //     Gizmos.DrawWireSphere(positions[i], radius[i]);
        // }
    }
}