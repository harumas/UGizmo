using UnityEngine;

namespace UGizmo.Samples.Benchmark
{
    /// <summary>
    /// Finds how many gizmos can be drawn while still holding <see cref="targetFps"/>, and reports
    /// the min, max and average across <see cref="tryMax"/> runs. Flip <see cref="useUGizmo"/> to
    /// measure the same workload through the built-in Gizmos class for comparison.
    ///
    /// Shape and colour vary per instance, but are derived from a hash of the index rather than
    /// randomised, so every run draws exactly the same picture and the two paths stay comparable.
    ///
    /// Both paths draw from OnDrawGizmos so that the call site is identical. That makes this an
    /// editor-only measurement, since Unity only calls OnDrawGizmos in the editor - but the file
    /// still has to live outside an Editor folder, because a MonoBehaviour compiled into an editor
    /// assembly cannot be attached to a GameObject.
    /// </summary>
    public class UGizmoBenchmark : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int targetFps = 60;
        [SerializeField] private bool useUGizmo;
        [SerializeField] private int tryMax = 5;

        [Header("Shape")]
        [SerializeField] private int drawCount;
        [SerializeField] private float radius = 5f;
        [SerializeField] private Vector3 center;
        [SerializeField] private float cubeSize = 1f;

        [Header("Result (read only)")]
        [SerializeField] private int attempt;
        [SerializeField] private int drawCountSum;
        [SerializeField] private int drawCountMin = int.MaxValue;
        [SerializeField] private int drawCountMax = int.MinValue;
        [SerializeField] private float elapsed;

        private const int StepCount = 64;
        private const float SampleDuration = 0.5f;

        /// <summary>
        /// Shapes that both UGizmo and the built-in Gizmos class can draw, so the two paths render
        /// the same picture and the comparison stays honest.
        /// </summary>
        private enum ShapeKind
        {
            Cube,
            WireCube,
            Sphere,
            WireSphere,
            Line,
        }

        private static readonly int ShapeKindCount = System.Enum.GetValues(typeof(ShapeKind)).Length;

        private static readonly Color[] Palette =
        {
            new Color(0.12f, 0.75f, 1f),
            new Color(1f, 0.34f, 0.36f),
            new Color(0.45f, 0.92f, 0.53f),
            new Color(1f, 0.83f, 0.31f),
            new Color(0.78f, 0.52f, 1f),
            new Color(0.35f, 0.98f, 0.92f),
            new Color(1f, 0.6f, 0.85f),
            new Color(0.95f, 0.95f, 0.95f),
        };

        private int sampledFrames;

        private void Start()
        {
            // A frame rate cap would put a ceiling on the measurement well before the draw cost does.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;

            ResetRun();
        }

        private void Update()
        {
            if (attempt >= tryMax)
            {
                return;
            }

            elapsed += Time.unscaledDeltaTime;
            sampledFrames++;

            if (elapsed < SampleDuration)
            {
                return;
            }

            float averageFps = sampledFrames / elapsed;

            elapsed = 0f;
            sampledFrames = 0;

            if (averageFps >= targetFps)
            {
                drawCount += StepCount;

                return;
            }

            RecordAttempt();
        }

        private void RecordAttempt()
        {
            attempt++;
            drawCountSum += drawCount;
            drawCountMin = Mathf.Min(drawCountMin, drawCount);
            drawCountMax = Mathf.Max(drawCountMax, drawCount);

            Debug.Log($"[UGizmo Benchmark] attempt {attempt}/{tryMax}: {drawCount} cubes at {targetFps}fps ({(useUGizmo ? "UGizmo" : "Gizmos")})");

            if (attempt >= tryMax)
            {
                Debug.Log($"[UGizmo Benchmark] done ({(useUGizmo ? "UGizmo" : "Gizmos")}) - " +
                          $"min {drawCountMin}, max {drawCountMax}, average {drawCountSum / attempt}");

                return;
            }

            drawCount = 0;
        }

        private void ResetRun()
        {
            attempt = 0;
            drawCount = 0;
            drawCountSum = 0;
            drawCountMin = int.MaxValue;
            drawCountMax = int.MinValue;
            elapsed = 0f;
            sampledFrames = 0;
        }

        private void OnDrawGizmos()
        {
            Vector3 size = Vector3.one * cubeSize;
            float radiusHalf = cubeSize * 0.5f;

            for (int i = 0; i < drawCount; i++)
            {
                uint hash = Hash(i);

                Vector3 position = center + GetPosition(i);
                Color color = Palette[hash % Palette.Length];
                ShapeKind shape = (ShapeKind)(hash / (uint)Palette.Length % (uint)ShapeKindCount);

                if (useUGizmo)
                {
                    DrawWithUGizmo(shape, position, size, radiusHalf, color);
                }
                else
                {
                    Gizmos.color = color;
                    DrawWithGizmos(shape, position, size, radiusHalf);
                }
            }
        }

        private static void DrawWithUGizmo(ShapeKind shape, Vector3 position, Vector3 size, float radius, Color color)
        {
            switch (shape)
            {
                case ShapeKind.Cube:
                    UGizmos.DrawCube(position, Quaternion.identity, size, color);
                    break;
                case ShapeKind.WireCube:
                    UGizmos.DrawWireCube(position, Quaternion.identity, size, color);
                    break;
                case ShapeKind.Sphere:
                    UGizmos.DrawSphere(position, radius, color);
                    break;
                case ShapeKind.WireSphere:
                    UGizmos.DrawWireSphere(position, radius, color);
                    break;
                case ShapeKind.Line:
                    UGizmos.DrawLine(position, position + Vector3.up * size.y, color);
                    break;
            }
        }

        private static void DrawWithGizmos(ShapeKind shape, Vector3 position, Vector3 size, float radius)
        {
            switch (shape)
            {
                case ShapeKind.Cube:
                    Gizmos.DrawCube(position, size);
                    break;
                case ShapeKind.WireCube:
                    Gizmos.DrawWireCube(position, size);
                    break;
                case ShapeKind.Sphere:
                    Gizmos.DrawSphere(position, radius);
                    break;
                case ShapeKind.WireSphere:
                    Gizmos.DrawWireSphere(position, radius);
                    break;
                case ShapeKind.Line:
                    Gizmos.DrawLine(position, position + Vector3.up * size.y);
                    break;
            }
        }

        /// <summary>
        /// Colour and shape come from a hash of the index rather than Random, so every attempt and
        /// both draw paths produce an identical picture. Randomising per frame would make the two
        /// measurements incomparable.
        /// </summary>
        private static uint Hash(int index)
        {
            uint value = (uint)index * 2654435761u;
            value ^= value >> 15;
            value *= 2246822519u;
            value ^= value >> 13;

            return value;
        }

        /// <summary>
        /// Spreads the cubes over a sphere with a golden angle spiral, so the layout is even and,
        /// more importantly, identical between runs and between the two draw paths.
        /// </summary>
        private Vector3 GetPosition(int index)
        {
            const float goldenAngle = 2.399963f;

            float t = drawCount <= 1 ? 0f : index / (float)(drawCount - 1);
            float y = 1f - 2f * t;
            float ringRadius = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
            float angle = goldenAngle * index;

            return new Vector3(Mathf.Cos(angle) * ringRadius, y, Mathf.Sin(angle) * ringRadius) * radius;
        }
    }
}
