using System.Collections.Generic;
using UnityEngine;
using UGizmos;
using UnityEditor;

public class UGizmoTester : MonoBehaviour
{
    private List<Vector3> positions;
    private List<float> radius;
    private List<Color> colors;

    private void OnValidate()
    {
        colors = new List<Color>();
        radius = new List<float>();
        positions = new List<Vector3>();

        for (int i = 0; i < 5000; i++)
        {
            positions.Add(Random.insideUnitSphere * 50f);
            radius.Add(Random.Range(1f, 10f));
            colors.Add(Random.ColorHSV(0.5f, 1f));
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Color color = colors[i];
            UGizmos.UGizmos.DrawWireSphere(positions[i], radius[i], color);
        }
        
        // for (int i = 0; i < positions.Count; i++)
        // {
        //     Gizmos.color = colors[i];
        //     Gizmos.DrawWireSphere(positions[i], radius[i]);
        // }
    }
}