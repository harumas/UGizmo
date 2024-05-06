using System;
using UGizmo;
using UnityEngine;
using Random = UnityEngine.Random;

public class UGizmoBenchmark : MonoBehaviour
{
    [SerializeField] private float targetFps = 60f;
    [SerializeField] private bool useUGizmo;
    [SerializeField] private int tryMax = 10;
    [SerializeField] private int drawCount;

    [SerializeField] private float radius = 5f;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private float cubeSize = 1f;

    private int attempt;
    private long drawCountSum;
    private int drawCountMin = Int32.MaxValue;
    private int drawCountMax = Int32.MinValue;  
    private float elapsed;

    private void Update()
    {
        double fps = 1f / Time.deltaTime;
        elapsed += Time.deltaTime;

        if (fps > targetFps)
        {
            drawCount++;
            elapsed = 0f;
        }

        //the attempt ends when elapsed exceeds 5 seconds
        if (elapsed >= 5f)
        {
            if (drawCount > drawCountMax)
            {
                drawCountMax = drawCount;
            }

            if (drawCount < drawCountMin)
            {
                drawCountMin = drawCount;
            }
            
            Debug.Log($"#{attempt} drawCount = {drawCount}");
            drawCountSum += drawCount;
            attempt++;
            
            drawCount = 0;
            elapsed = 0f;

            if (attempt == tryMax)
            {
                Debug.Log($"Finished! Ave: {drawCountSum / (float)attempt}, Min: {drawCountMin}, Max: {drawCountMax}");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Set random number generator state
        Random.InitState(123);

        if (useUGizmo)
        {
            DrawUGizmos();
        }
        else
        {
            DrawGizmos();
        }
    }

    private void DrawGizmos()
    {
        float hue = 0f;

        for (int i = 0; i < drawCount; i++)
        {
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            color.a = 1f;

            //Draw Cube
            Vector3 randomPosition = Random.insideUnitSphere * radius + center;
            Gizmos.color = color;
            Gizmos.DrawCube(randomPosition, new Vector3(cubeSize, cubeSize, cubeSize));

            //Draw Sphere
            randomPosition = Random.insideUnitSphere * radius + center;
            Gizmos.DrawSphere(randomPosition, cubeSize);

            //Draw Line
            Vector3 point1 = Random.insideUnitSphere * radius + center;
            Vector3 point2 = Random.insideUnitSphere * radius + center;
            Gizmos.DrawLine(point1, point2);

            hue += 1f / drawCount;
        }
    }

    private void DrawUGizmos()
    {
        float hue = 0f;

        for (int i = 0; i < drawCount; i++)
        {
            Color color = Color.HSVToRGB(hue, 1f, 1f);
            color.a = 1f;

            //Draw Cube
            Vector3 randomPosition = Random.insideUnitSphere * radius + center;
            UGizmos.DrawCube(randomPosition, Quaternion.identity, new Vector3(cubeSize, cubeSize, cubeSize), color);

            //Draw Sphere
            randomPosition = Random.insideUnitSphere * radius + center;
            UGizmos.DrawSphere(randomPosition, cubeSize, color);

            //Draw Line
            Vector3 point1 = Random.insideUnitSphere * radius + center;
            Vector3 point2 = Random.insideUnitSphere * radius + center;
            UGizmos.DrawLine(point1, point2, color);

            hue += 1f / drawCount;
        }
    }
}