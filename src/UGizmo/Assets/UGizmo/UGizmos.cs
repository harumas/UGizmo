using System;
using System.Runtime.InteropServices;
using UGizmo.Internal;
using UGizmo.Internal.Extension.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGizmo
{
    [BurstCompile]
    public static partial class UGizmos
    {
        private static readonly Color trueColor = new Color(0.12f, 0.75f, 1f, 1f);
        private static readonly Color trueDarkColor = new Color(0.09f, 0.32f, 0.72f);
        private static readonly Color falseColor = new Color(1f, 0.09f, 0.12f, 1f);
        private static readonly Color falseDarkColor = new Color(1f, 0.05f, 0.11f);
        private static readonly Color invalidColor = new Color(0.19f, 0.18f, 0.18f, 0.22f);
        private static readonly float pointRadius = 0.08f;
        private static readonly float alphaRate = 0.3f;

        #region Primitive

        /// <summary>
        /// Draw a solid sphere.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawSphere(Vector3 center, float radius, Color color)
        {
            DrawSphereCore(center, Quaternion.identity, radius, color);
        }

        /// <summary>
        /// Draw a wire sphere.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            DrawWireSphereCore(center, Quaternion.identity, radius, color);
        }

        /// <summary>
        /// Draw a wire sphere with rotation.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireSphere(Vector3 center, Quaternion rotation, float radius, Color color)
        {
            DrawWireSphereCore(center, rotation, radius, color);
        }

        /// <summary>
        /// Draw a solid cube.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawCube(Vector3 center, Quaternion rotation, Vector3 size, Color color)
        {
            DrawCubeCore(center, rotation, size, color);
        }

        /// <summary>
        /// Draw a solid cube.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size, Color color)
        {
            DrawWireCubeCore(center, rotation, size, color);
        }

        /// <summary>
        /// Draw a solid capsule.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public static void DrawCapsule(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            DrawCapsuleCore(position, rotation * Vector3.up, scale.y, math.max(scale.x, scale.z), color);
        }

        /// <summary>
        /// Draw a solid capsule with upAxis.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="upAxis"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawCapsule(Vector3 center, Vector3 upAxis, float height, float radius, Color color)
        {
            DrawCapsuleCore(center, upAxis, height, radius, color);
        }

        /// <summary>
        /// Draw a solid capsule extending from point1 to point2.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawCapsule(Vector3 point1, Vector3 point2, float radius, Color color)
        {
            float3 diff = point2 - point1;
            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);
            DrawCapsuleCore((float3)point1 + diff * 0.5f, normal, length + radius * 2f, radius, color);
        }

        /// <summary>
        /// Draw a wire capsule.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public static void DrawWireCapsule(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            DrawWireCapsuleCore(position, rotation * Vector3.up, scale.y, math.max(scale.x, scale.z), color);
        }

        /// <summary>
        /// Draw a wire capsule with upAxis.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="upAxis"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireCapsule(Vector3 center, Vector3 upAxis, float height, float radius, Color color)
        {
            DrawWireCapsuleCore(center, upAxis, height, radius, color);
        }

        /// <summary>
        /// Draw a wire capsule extending from point1 to point2.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireCapsule(Vector3 point1, Vector3 point2, float radius, Color color)
        {
            float3 diff = point2 - point1;
            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);
            DrawWireCapsuleCore((float3)point1 + diff * 0.5f, normal, length + radius * 2f, radius, color);
        }

        /// <summary>
        /// Draw a solid cylinder.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public static void DrawCylinder(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            DrawCylinderCore(position, rotation, scale, color);
        }

        /// <summary>
        /// Draw a solid cylinder extending from point1 to point2.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawCylinder(Vector3 point1, Vector3 point2, float radius, Color color)
        {
            float3 diff = point2 - point1;
            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);
            GizmoUtil.FromUpToRotation(normal, out quaternion rotation);
            DrawCylinderCore(point1 * diff * 0.5f, rotation, new float3(radius * 2f, length, radius * 2f), color);
        }

        /// <summary>
        /// Draw a wire cylinder.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public static void DrawWireCylinder(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            DrawWireCylinderCore(position, rotation * Vector3.up, scale.y, math.max(scale.x, scale.z), color);
        }

        /// <summary>
        /// Draw a wire cylinder extending from point1 to point2.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireCylinder(Vector3 point1, Vector3 point2, float radius, Color color)
        {
            float3 diff = point2 - point1;
            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);
            DrawWireCylinderCore((float3)point1 + diff * 0.5f, normal, length + radius * 2f, radius, color);
        }

        /// <summary>
        /// Draw a solid cone.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public static void DrawCone(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            float maxX = math.max(scale.x, scale.y) * 0.5f;
            DrawConeCore(position, rotation * Vector3.forward, scale.z, math.atan2(maxX, scale.z), color);
        }

        /// <summary>
        /// Draw a solid cone with direction.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="color"></param>
        public static void DrawCone(float3 origin, float3 direction, float distance, float angle, Color color)
        {
            DrawConeCore(origin, direction, distance, angle, color);
        }

        /// <summary>
        /// Draw a wire cone.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public static void DrawWireCone(Vector3 position, Quaternion rotation, Vector3 scale, Color color)
        {
            float maxX = math.max(scale.x, scale.y) * 0.5f;
            DrawWireConeCore(position, rotation * Vector3.forward, scale.z, math.atan2(maxX, scale.z), color);
        }

        /// <summary>
        /// Draw a wire cone with direction.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="color"></param>
        public static void DrawWireCone(float3 origin, float3 direction, float distance, float angle, Color color)
        {
            DrawWireConeCore(origin, direction, distance, angle, color);
        }

        /// <summary>
        /// Draw a plane.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawPlane(Vector3 position, Quaternion rotation, Vector2 size, Color color)
        {
            DrawPlaneCore(position, rotation, size, color);
        }

        /// <summary>
        /// Draw a wireframe plane.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWirePlane(Vector3 position, Quaternion rotation, Vector2 size, Color color)
        {
            DrawWirePlaneCore(position, rotation, size, color);
        }

        #endregion

        #region Primitive2D

        /// <summary>
        /// Draw a 2D circle.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawCircle2D(Vector3 position, Quaternion rotation, float radius, Color color)
        {
            GizmoUtil.Rotate90X(rotation, out quaternion result);
            DrawCircleCore(position, result, radius, color);
        }

        /// <summary>
        /// Draw a 2D circle oriented in the z direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawCircle2D(Vector3 position, float radius, Color color)
        {
            DrawCircleCore(position, GizmoUtil.GetRotate90X(), radius, color);
        }

        /// <summary>
        /// Draw a 2D wire circle.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireCircle2D(Vector3 position, Quaternion rotation, float radius, Color color)
        {
            GizmoUtil.Rotate90X(rotation, out quaternion result);
            DrawWireCircleCore(position, result, radius, color);
        }

        /// <summary>
        /// Draw a 2D wire circle oriented in the z direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireCircle2D(Vector3 position, float radius, Color color)
        {
            DrawWireCircleCore(position, GizmoUtil.GetRotate90X(), radius, color);
        }

        /// <summary>
        /// Draw a 2D box.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawBox2D(Vector3 position, Quaternion rotation, Vector2 size, Color color)
        {
            GizmoUtil.Rotate90X(rotation, out quaternion result);
            DrawPlaneCore(position, result, size, color);
        }

        /// <summary>
        /// Draw a 2D box oriented in the z direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawBox2D(Vector3 position, float angle, Vector2 size, Color color)
        {
            GizmoUtil.PlaneToQuad(angle, out quaternion rotation);
            DrawPlaneCore(position, rotation, size, color);
        }

        /// <summary>
        /// Draw a 2D wire box.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWireBox2D(Vector3 position, Quaternion rotation, Vector2 size, Color color)
        {
            GizmoUtil.Rotate90X(rotation, out quaternion result);
            DrawWirePlaneCore(position, result, size, color);
        }

        /// <summary>
        /// Draw a 2D wire box oriented in the z direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWireBox2D(Vector3 position, float angle, Vector2 size, Color color)
        {
            GizmoUtil.PlaneToQuad(angle, out quaternion rotation);
            DrawWirePlaneCore(position, rotation, size, color);
        }

        /// <summary>
        /// Draw a 2D triangle.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawTriangle2D(Vector3 position, Quaternion rotation, Vector2 size, Color color)
        {
            DrawTriangleCore(position, rotation, size, color);
        }

        /// <summary>
        /// Draw a 2D triangle oriented in the z direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawTriangle2D(Vector3 position, float angle, Vector2 size, Color color)
        {
            GizmoUtil.Rotate2D(angle, out quaternion to);
            DrawTriangleCore(position, to, size, color);
        }

        /// <summary>
        /// Draw a 2D wire triangle.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWireTriangle2D(Vector3 position, Quaternion rotation, Vector2 size, Color color)
        {
            DrawWireTriangleCore(position, rotation, size, color);
        }

        /// <summary>
        /// Draw a 2D wire triangle oriented in the z direction.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        public static void DrawWireTriangle2D(Vector3 position, float angle, Vector2 size, Color color)
        {
            GizmoUtil.Rotate2D(angle, out quaternion to);
            DrawWireTriangleCore(position, to, size, color);
        }

        /// <summary>
        /// Draw a wire triangle connecting the three points.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="color"></param>
        public static void DrawWireTriangle2D(Vector3 point1, Vector3 point2, Vector3 point3, Color color)
        {
            DrawWireLineCore(point1, point2, color);
            DrawWireLineCore(point2, point3, color);
            DrawWireLineCore(point3, point1, color);
        }

        /// <summary>
        /// Draw a 2D capsule.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawCapsule2D(Vector3 center, Quaternion rotation, float height, float radius, Color color)
        {
            DrawCapsule2DCore(center, rotation, height, radius, color);
        }

        /// <summary>
        /// Draw a 2D capsule oriented in the z direction.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="angle"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawCapsule2D(Vector3 center, float angle, float height, float radius, Color color)
        {
            GizmoUtil.Rotate2D(angle, out quaternion to);
            DrawCapsule2DCore(center, to, height, radius, color);
        }

        /// <summary>
        /// Draw a 2D wire capsule.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireCapsule2D(Vector3 center, Quaternion rotation, float height, float radius, Color color)
        {
            DrawWireCapsule2DCore(center, rotation, height, radius, color);
        }

        /// <summary>
        /// Draw a 2D wire capsule oriented in the z direction.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="angle"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawWireCapsule2D(Vector3 center, float angle, float height, float radius, Color color)
        {
            GizmoUtil.Rotate2D(angle, out quaternion to);
            DrawWireCapsule2DCore(center, to, height, radius, color);
        }

        #endregion

        #region Utility

        /// <summary>
        /// Draw points. This is drawn in the foreground.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void DrawPoint(Vector3 position, float radius, Color color)
        {
            DrawPointCore(position, Quaternion.identity, radius, color);
        }

        /// <summary>
        /// Draws a line starting at from towards to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            DrawWireLineCore(from, to, color);
        }

        /// <summary>
        /// Draws multiple lines between pairs of points. 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="color"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static unsafe void DrawLineList(ReadOnlySpan<Vector3> points, Color color)
        {
            if (points.Length / 2 > wireLineBuffer.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(points.Length), $"Length must be in the range of 0 to {wireLineBuffer.Length - 1}.");
            }

            fixed (float3* ptr = MemoryMarshal.Cast<Vector3, float3>(points))
            {
                fixed (LineData* destination = wireLineBuffer)
                {
                    WriteLineBufferList_Burst(ptr, destination, points.Length, (float4*)&color);
                }
            }

            fixed (LineData* ptr = wireLineBuffer)
            {
                DrawWireLineRangeCore(ptr, points.Length / 2);
            }
        }

        /// <summary>
        /// Draws a line between each point in the supplied span.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="loop"></param>
        /// <param name="color"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static unsafe void DrawLineStrip(ReadOnlySpan<Vector3> points, bool loop, Color color)
        {
            if (points.Length > wireLineBuffer.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(points.Length), $"Length must be in the range of 0 to {wireLineBuffer.Length - 1}.");
            }

            fixed (float3* ptr = MemoryMarshal.Cast<Vector3, float3>(points))
            {
                fixed (LineData* destination = wireLineBuffer)
                {
                    WriteLineBufferStrip_Burst(ptr, destination, points.Length, loop, (float4*)&color);
                }
            }

            fixed (LineData* ptr = wireLineBuffer)
            {
                DrawWireLineRangeCore(ptr, points.Length);
            }
        }

        [BurstCompile]
        private static unsafe void WriteLineBufferList_Burst(float3* points, LineData* destination, int count, float4* color)
        {
            for (int i = 0; i < count / 2; i++)
            {
                destination[i] = new LineData(points[i * 2], points[i * 2 + 1], *(Color*)color);
            }
        }

        [BurstCompile]
        private static unsafe void WriteLineBufferStrip_Burst(float3* points, LineData* destination, int count, bool loop, float4* color)
        {
            for (int i = 0; i < count; i++)
            {
                destination[i] = new LineData(points[i], points[i + 1], *(Color*)color);
            }

            if (loop)
            {
                destination[count] = new LineData(points[count - 1], points[0], *(Color*)color);
            }
        }

        /// <summary>
        ///	Draws a ray starting at from to from + direction.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="direction"></param>
        /// <param name="color"></param>
        public static void DrawRay(Vector3 from, Vector3 direction, Color color)
        {
            DrawWireLineCore(from, from + direction, color);
        }

        /// <summary>
        /// Draw a ray.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="color"></param>
        public static void DrawRay(Ray ray, Color color)
        {
            DrawWireLineCore(ray.origin, ray.origin + ray.direction, color);
        }

        /// <summary>
        /// Draw a camera frustum.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="rotation"></param>
        /// <param name="fov"></param>
        /// <param name="farClipPlane"></param>
        /// <param name="nearClipPlane"></param>
        /// <param name="aspect"></param>
        /// <param name="color"></param>
        public static void DrawFrustum(
            Vector3 center,
            Quaternion rotation,
            float fov,
            float farClipPlane,
            float nearClipPlane,
            float aspect,
            Color color)
        {
            DrawFrustumCore(center, rotation, fov, farClipPlane, nearClipPlane, aspect, color);
        }

        /// <summary>
        /// Draw a camera frustum.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="color"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void DrawFrustum(Camera camera, Color color)
        {
            if (camera == null)
            {
                throw new ArgumentNullException(camera.ToString());
            }

            DrawFrustumCore(
                camera.transform.position,
                camera.transform.rotation,
                camera.fieldOfView,
                camera.farClipPlane,
                camera.nearClipPlane,
                camera.aspect,
                color);
        }


        /// <summary>
        /// Draws a distance starting at from towards to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="headLength"></param>
        /// <param name="headWidth"></param>
        public static void DrawDistance(Vector3 from, Vector3 to, Color color, float headLength = 0.5f, float headWidth = 0.3f)
        {
            Camera camera = Camera.current;
            float3 diff = to - from;
            float3 center = (float3)from + diff * 0.5f;
            float3 camDiff = (float3)camera.transform.position - center;

            float3 camNormal = math.normalizesafe(camDiff, new float3(0f, 0f, 1f));

            DrawWireArrow2dCore(center, from, camNormal, color, headLength, headWidth);
            DrawWireArrow2dCore(center, to, camNormal, color, headLength, headWidth);

#if UNITY_EDITOR
            float3 textNormal = math.normalizesafe(math.cross(camNormal, diff), new float3(0f, 0f, 1f));

            GUIStyle skinLabel = GUI.skin.label;
            TextAnchor original = skinLabel.alignment;
            skinLabel.alignment = TextAnchor.MiddleCenter;
            Handles.Label(center + textNormal * 0.2f, math.length(diff).ToString("0.00"), skinLabel);
            skinLabel.alignment = original;
#endif
        }

        /// <summary>
        /// Draw a line with interval.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="interval"></param>
        /// <param name="color"></param>
        public static void DrawMeasure(Vector3 from, Vector3 to, float interval, Color color)
        {
            Camera camera = Camera.current;
            float3 a = from;
            float3 b = to;
            float3 diff = to - from;
            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);

            float3 cross = math.normalizesafe(math.cross(camera.transform.forward, normal));

            float3 offset = cross * 0.5f;
            DrawWireLineCore(a, b, color);
            DrawWireLineCore(a + offset, a - offset, color);
            DrawWireLineCore(b + offset, b - offset, color);

            int count = (int)(length / interval);
            float3 intervalOffset = normal * interval;
            offset = cross * 0.3f;
            for (int i = 0; i < count; i++)
            {
                a += intervalOffset;
                DrawWireLineCore(a + offset, a - offset, color);
            }
        }

        #endregion

        #region Arrow

        /// <summary>
        /// Draws a solid arrow starting at from towards to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="headLength"></param>
        /// <param name="width"></param>
        public static void DrawArrow(Vector3 from, Vector3 to, Color color, float headLength = 0.5f, float width = 0.3f)
        {
            DrawArrowCore(from, to, color, headLength, width);
        }

        /// <summary>
        /// Draws a 2D arrow always facing the given normal.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="normal"></param>
        /// <param name="color"></param>
        /// <param name="headLength"></param>
        /// <param name="width"></param>
        public static void DrawArrow2d(Vector3 from, Vector3 to, Vector3 normal, Color color, float headLength = 0.5f, float width = 0.3f)
        {
            DrawArrow2dCore(from, to, normal, color, headLength, width);
        }

        /// <summary>
        /// Draws a 2D front-facing arrow.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="headLength"></param>
        /// <param name="width"></param>
        public static void DrawFacingArrow2d(Vector3 from, Vector3 to, Color color, float headLength = 0.5f, float width = 0.3f)
        {
            Camera camera = Camera.current;
            float3 position = from + (to - from) * 0.5f;
            float3 normal = math.normalizesafe((float3)camera.transform.position - position, new float3(0f, 0f, 1f));

            DrawArrow2dCore(from, to, normal, color, headLength, width);
        }

        /// <summary>
        /// Draws a 2D wire arrow always facing the given normal.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="normal"></param>
        /// <param name="color"></param>
        /// <param name="headLength"></param>
        /// <param name="headWidth"></param>
        public static void DrawWireArrow(Vector3 from, Vector3 to, Vector3 normal, Color color, float headLength = 0.5f, float headWidth = 0.3f)
        {
            DrawWireArrow2dCore(from, to, normal, color, headLength, headWidth);
        }

        /// <summary>
        /// Draws a 2D wire front-facing arrow.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="headLength"></param>
        /// <param name="headWidth"></param>
        public static void DrawFacingWireArrow(Vector3 from, Vector3 to, Color color, float headLength = 0.5f, float headWidth = 0.3f)
        {
            Camera camera = Camera.current;
            float3 position = from + (to - from) * 0.5f;
            float3 normal = math.normalizesafe((float3)camera.transform.position - position, new float3(0f, 0f, 1f));

            DrawWireArrow2dCore(from, to, normal, color, headLength, headWidth);
        }

        #endregion

        #region Physics

        /// <summary>
        /// Draws a raycast visualizer. Returns true if it hits something.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="maxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="queryTriggerInteraction"></param>
        /// <returns></returns>
        public static bool Raycast(Vector3 origin,
            Vector3 direction,
            float maxDistance,
            int layerMask = -5,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            bool isHit = Physics.Raycast(origin, direction, out RaycastHit hitInfo, maxDistance, layerMask, queryTriggerInteraction);
            DrawRaycast(origin, direction, maxDistance, isHit, in hitInfo);
            return isHit;
        }

        /// <summary>
        /// Draw a raycast visualizer.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="maxDistance"></param>
        /// <param name="isHit"></param>
        /// <param name="hitInfo"></param>
        public static void DrawRaycast(Vector3 origin, Vector3 direction, float maxDistance, bool isHit, in RaycastHit hitInfo)
        {
            DrawShapeCast(origin, direction, maxDistance, isHit, in hitInfo, out _);
        }

        /// <summary>
        /// Draws a linecast visualizer. Returns true if it hits something.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="layerMask"></param>
        /// <param name="queryTriggerInteraction"></param>
        /// <returns></returns>
        public static bool Linecast(
            Vector3 start,
            Vector3 end,
            int layerMask = -5,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            float3 diff = end - start;
            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);
            return Raycast(start, normal, length, layerMask, queryTriggerInteraction);
        }

        /// <summary>
        /// Draws a linecast visualizer.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isHit"></param>
        /// <param name="hitInfo"></param>
        public static void DrawLinecast(
            Vector3 start,
            Vector3 end,
            bool isHit,
            in RaycastHit hitInfo)
        {
            float3 diff = end - start;
            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);
            DrawShapeCast(start, normal, length, isHit, in hitInfo, out _);
        }

        /// <summary>
        /// Draws a SphereCast visualizer. Returns true if it hits something.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="direction"></param>
        /// <param name="maxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="queryTriggerInteraction"></param>
        /// <returns></returns>
        public static bool SphereCast(Vector3 origin,
            float radius,
            Vector3 direction,
            float maxDistance,
            int layerMask = -5,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            bool isHit = Physics.SphereCast(origin, radius, direction, out RaycastHit hitInfo, maxDistance, layerMask, queryTriggerInteraction);
            DrawSphereCast(origin, radius, direction, maxDistance, isHit, in hitInfo);
            return isHit;
        }

        /// <summary>
        /// Draws a SphereCast visualizer.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="direction"></param>
        /// <param name="maxDistance"></param>
        /// <param name="isHit"></param>
        /// <param name="hitInfo"></param>
        public static void DrawSphereCast(Vector3 origin,
            float radius,
            Vector3 direction,
            float maxDistance,
            bool isHit,
            in RaycastHit hitInfo)
        {
            Color color = isHit ? trueColor : falseColor;
            DrawShapeCast(origin, direction, maxDistance, isHit, in hitInfo, out Vector3 target);

            if (isHit)
            {
                DrawPoint(hitInfo.point, pointRadius, trueDarkColor);
            }

            DrawWireSphere(origin, radius, color);
            DrawWireSphere(target, radius, color);

            color.a *= alphaRate;
            DrawSphere(origin, radius, color);
            DrawSphere(target, radius, color);
        }

        /// <summary>
        /// Draws a BoxCast visualizer. Returns true if it hits something.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="halfExtents"></param>
        /// <param name="direction"></param>
        /// <param name="orientation"></param>
        /// <param name="maxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="queryTriggerInteraction"></param>
        /// <returns></returns>
        public static bool BoxCast(Vector3 origin,
            Vector3 halfExtents,
            Vector3 direction,
            Quaternion orientation,
            float maxDistance,
            int layerMask = -5,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            bool isHit = Physics.BoxCast(origin, halfExtents, direction, out RaycastHit hitInfo, orientation, maxDistance, layerMask,
                queryTriggerInteraction);
            DrawBoxCast(origin, halfExtents, direction, orientation, maxDistance, isHit, in hitInfo);
            return isHit;
        }

        /// <summary>
        /// Draws a BoxCast visualizer.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="halfExtents"></param>
        /// <param name="direction"></param>
        /// <param name="orientation"></param>
        /// <param name="maxDistance"></param>
        /// <param name="isHit"></param>
        /// <param name="hitInfo"></param>
        public static void DrawBoxCast(Vector3 origin,
            Vector3 halfExtents,
            Vector3 direction,
            Quaternion orientation,
            float maxDistance,
            bool isHit,
            in RaycastHit hitInfo)
        {
            Color color = isHit ? trueColor : falseColor;

            DrawShapeCast(origin, direction, maxDistance, isHit, in hitInfo, out Vector3 target);

            if (isHit)
            {
                DrawPoint(hitInfo.point, pointRadius, trueDarkColor);
            }

            Vector3 size = halfExtents * 2f;
            DrawWireCube(origin, orientation, size, color);
            DrawWireCube(target, orientation, size, color);

            color.a *= alphaRate;
            DrawCube(origin, orientation, size, color);
            DrawCube(target, orientation, size, color);
        }

        /// <summary>
        /// Draws a CapsuleCast visualizer. Returns true if it hits something.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="radius"></param>
        /// <param name="direction"></param>
        /// <param name="maxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="queryTriggerInteraction"></param>
        /// <returns></returns>
        public static bool CapsuleCast(
            Vector3 point1,
            Vector3 point2,
            float radius,
            Vector3 direction,
            float maxDistance,
            int layerMask = -5,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            bool isHit = Physics.CapsuleCast(point1, point2, radius, direction, out RaycastHit hitInfo, maxDistance, layerMask,
                queryTriggerInteraction);
            DrawCapsuleCast(point1, point2, radius, direction, maxDistance, isHit, in hitInfo);
            return isHit;
        }

        /// <summary>
        /// Draws a CapsuleCast visualizer.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="radius"></param>
        /// <param name="direction"></param>
        /// <param name="maxDistance"></param>
        /// <param name="isHit"></param>
        /// <param name="hitInfo"></param>
        public static void DrawCapsuleCast(Vector3 point1,
            Vector3 point2,
            float radius,
            Vector3 direction,
            float maxDistance,
            bool isHit,
            in RaycastHit hitInfo)
        {
            Color color = isHit ? trueColor : falseColor;
            float3 diff = point2 - point1;
            Vector3 origin = point1 + (Vector3)diff * 0.5f;

            DrawShapeCast(origin, direction, maxDistance, isHit, in hitInfo, out Vector3 target);

            if (isHit)
            {
                DrawPoint(hitInfo.point, pointRadius, trueDarkColor);
            }

            GizmoUtil.LengthAndNormalize(diff, out float length, out float3 normal);
            float height = length + radius * 2f;
            DrawWireCapsule(origin, normal, height, radius, color);
            DrawWireCapsule(target, normal, height, radius, color);

            color.a *= alphaRate;
            DrawCapsule(origin, normal, height, radius, color);
            DrawCapsule(target, normal, height, radius, color);
        }

        private static void DrawShapeCast(Vector3 origin, Vector3 direction, float maxDistance, bool isHit, in RaycastHit hitInfo, out Vector3 target)
        {
            Color color = isHit ? trueColor : falseColor;
            Color darkColor = isHit ? trueDarkColor : falseDarkColor;

            if (isHit)
            {
                Vector3 point = hitInfo.point;

                DrawFacingWireArrow(point, point + hitInfo.normal, color, 0.2f, 0.1f);
                DrawFacingWireArrow(origin, origin + direction * maxDistance, invalidColor, 0.2f, 0.1f);
            }

            target = origin + direction * (isHit ? hitInfo.distance : maxDistance);
            DrawFacingWireArrow(origin, target, color, 0.2f, 0.1f);
            DrawPoint(origin, pointRadius, darkColor);
            DrawPoint(target, pointRadius, darkColor);
        }

        #endregion

        #region Physics2D

        /// <summary>
        /// Draws a 2D raycast visualizer. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static RaycastHit2D Raycast2D(
            Vector2 origin,
            Vector2 direction,
            float distance = float.PositiveInfinity,
            int layerMask = -1,
            float minDepth = float.MinValue,
            float maxDepth = float.MaxValue)
        {
            ContactFilter2D legacyFilter = CreateLegacyFilter(layerMask, minDepth, maxDepth);
            return Raycast2D(origin, direction, legacyFilter, distance);
        }

        /// <summary>
        /// Draws a 2D raycast visualizer with contact filter 2D. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="contactFilter2D"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static RaycastHit2D Raycast2D(
            Vector2 origin,
            Vector2 direction,
            ContactFilter2D contactFilter2D,
            float distance = float.PositiveInfinity)
        {
            RaycastHit2D hitInfo = Physics2D.defaultPhysicsScene.Raycast(origin, direction, distance, contactFilter2D);
            bool isHit = hitInfo.collider != null;
            DrawShapeCast2D(origin, direction, distance, isHit, hitInfo, out _);
            return hitInfo;
        }

        /// <summary>
        /// Draws a 2D linecast visualizer. Returns RaycastHit2D.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static RaycastHit2D Linecast2D(
            Vector2 start,
            Vector2 end,
            float distance = float.PositiveInfinity,
            int layerMask = -1,
            float minDepth = float.MinValue,
            float maxDepth = float.MaxValue)
        {
            ContactFilter2D legacyFilter = CreateLegacyFilter(layerMask, minDepth, maxDepth);
            return Linecast2D(start, end, legacyFilter, distance);
        }

        /// <summary>
        /// Draws a 2D linecast visualizer with contact filter 2D. Returns RaycastHit2D.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="contactFilter2D"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static RaycastHit2D Linecast2D(
            Vector2 start,
            Vector2 end,
            ContactFilter2D contactFilter2D,
            float distance = float.PositiveInfinity)
        {
            RaycastHit2D hitInfo = Physics2D.defaultPhysicsScene.Linecast(start, end, contactFilter2D);
            bool isHit = hitInfo.collider != null;
            DrawShapeCast2D(start, end, distance, isHit, hitInfo, out _);
            return hitInfo;
        }

        /// <summary>
        /// Draws a 2D circlecast visualizer. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static RaycastHit2D Circlecast2D(
            Vector2 origin,
            float radius,
            Vector2 direction,
            float distance = float.PositiveInfinity,
            int layerMask = -1,
            float minDepth = float.MinValue,
            float maxDepth = float.MaxValue)
        {
            ContactFilter2D legacyFilter = CreateLegacyFilter(layerMask, minDepth, maxDepth);
            return Circlecast2D(origin, radius, direction, legacyFilter, distance);
        }

        /// <summary>
        /// Draws a 2D circlecast visualizer with contact filter 2D. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radius"></param>
        /// <param name="direction"></param>
        /// <param name="contactFilter2D"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static RaycastHit2D Circlecast2D(
            Vector2 origin,
            float radius,
            Vector2 direction,
            ContactFilter2D contactFilter2D,
            float distance = float.PositiveInfinity)
        {
            RaycastHit2D hitInfo = Physics2D.defaultPhysicsScene.CircleCast(origin, radius, direction, distance, contactFilter2D);
            bool isHit = hitInfo.collider != null;

            DrawShapeCast2D(origin, direction, distance, isHit, hitInfo, out Vector2 target);

            Color color = isHit ? trueColor : falseColor;
            if (isHit)
            {
                DrawPoint(hitInfo.point, pointRadius, trueDarkColor);
            }

            DrawWireCircle2D(origin, radius, color);
            DrawWireCircle2D(target, radius, color);

            color.a *= alphaRate;
            DrawCircle2D(origin, radius, color);
            DrawCircle2D(target, radius, color);

            return hitInfo;
        }

        /// <summary>
        /// Draws a 2D boxcast visualizer. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="size"></param>
        /// <param name="angle"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static RaycastHit2D Boxcast2D(
            Vector2 origin,
            Vector2 size,
            float angle,
            Vector2 direction,
            float distance = float.PositiveInfinity,
            int layerMask = -1,
            float minDepth = float.MinValue,
            float maxDepth = float.MaxValue)
        {
            ContactFilter2D legacyFilter = CreateLegacyFilter(layerMask, minDepth, maxDepth);
            return Boxcast2D(origin, size, angle, direction, legacyFilter, distance);
        }

        /// <summary>
        /// Draws a 2D boxcast visualizer with contact filter 2D. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="size"></param>
        /// <param name="angle"></param>
        /// <param name="direction"></param>
        /// <param name="contactFilter2D"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static RaycastHit2D Boxcast2D(
            Vector2 origin,
            Vector2 size,
            float angle,
            Vector2 direction,
            ContactFilter2D contactFilter2D,
            float distance = float.PositiveInfinity)
        {
            RaycastHit2D hitInfo = Physics2D.defaultPhysicsScene.BoxCast(origin, size, angle * Mathf.Rad2Deg, direction, distance, contactFilter2D);
            bool isHit = hitInfo.collider != null;

            DrawShapeCast2D(origin, direction, distance, isHit, hitInfo, out Vector2 target);

            Color color = isHit ? trueColor : falseColor;
            if (isHit)
            {
                DrawPoint(hitInfo.point, pointRadius, trueDarkColor);
            }

            DrawWireBox2D(origin, angle, size, color);
            DrawWireBox2D(target, angle, size, color);

            color.a *= alphaRate;
            DrawBox2D(origin, angle, size, color);
            DrawBox2D(target, angle, size, color);

            return hitInfo;
        }

        /// <summary>
        /// Draws a 2D Capsulecast2D visualizer. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="size"></param>
        /// <param name="angle"></param>
        /// <param name="capsuleDirection"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="layerMask"></param>
        /// <param name="minDepth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public static RaycastHit2D Capsulecast2D(
            Vector2 origin,
            Vector2 size,
            float angle,
            CapsuleDirection2D capsuleDirection,
            Vector2 direction,
            float distance = float.PositiveInfinity,
            int layerMask = -1,
            float minDepth = float.MinValue,
            float maxDepth = float.MaxValue)
        {
            ContactFilter2D legacyFilter = CreateLegacyFilter(layerMask, minDepth, maxDepth);
            return Capsulecast2D(origin, size, angle, capsuleDirection, direction, legacyFilter, distance);
        }

        /// <summary>
        /// Draws a 2D Capsulecast2D visualizer with contact filter 2D. Returns RaycastHit2D.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="size"></param>
        /// <param name="angle"></param>
        /// <param name="capsuleDirection"></param>
        /// <param name="direction"></param>
        /// <param name="contactFilter2D"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static RaycastHit2D Capsulecast2D(
            Vector2 origin,
            Vector2 size,
            float angle,
            CapsuleDirection2D capsuleDirection,
            Vector2 direction,
            ContactFilter2D contactFilter2D,
            float distance = float.PositiveInfinity)
        {
            RaycastHit2D hitInfo = Physics2D.defaultPhysicsScene.CapsuleCast(origin, size, capsuleDirection, angle * Mathf.Rad2Deg, direction,
                distance,
                contactFilter2D);
            bool isHit = hitInfo.collider != null;

            DrawShapeCast2D(origin, direction, distance, isHit, hitInfo, out Vector2 target);

            Color color = isHit ? trueColor : falseColor;
            if (isHit)
            {
                DrawPoint(hitInfo.point, pointRadius, trueDarkColor);
            }

            float radius = (capsuleDirection == CapsuleDirection2D.Vertical ? size.x : size.y) * 0.5f;
            float height = (capsuleDirection == CapsuleDirection2D.Vertical ? size.y : size.x);
            DrawWireCapsule2D(origin, angle, height, radius, color);
            DrawWireCapsule2D(target, angle, height, radius, color);

            color.a *= alphaRate;
            DrawCapsule2D(origin, angle, height, radius, color);
            DrawCapsule2D(target, angle, height, radius, color);

            return hitInfo;
        }

        private static ContactFilter2D CreateLegacyFilter(
            int layerMask,
            float minDepth,
            float maxDepth)
        {
            ContactFilter2D legacyFilter = new ContactFilter2D();
            legacyFilter.useTriggers = Physics2D.queriesHitTriggers;
            legacyFilter.SetLayerMask((LayerMask)layerMask);
            legacyFilter.SetDepth(minDepth, maxDepth);
            return legacyFilter;
        }

        private static void DrawShapeCast2D(
            Vector2 origin,
            Vector2 direction,
            float maxDistance,
            bool isHit,
            in RaycastHit2D hitInfo,
            out Vector2 target)
        {
            Color color = isHit ? trueColor : falseColor;
            Color darkColor = isHit ? trueDarkColor : falseDarkColor;

            if (isHit)
            {
                Vector2 point = hitInfo.point;

                DrawFacingWireArrow(point, point + hitInfo.normal * 0.5f, color, 0.2f, 0.1f);
                DrawFacingWireArrow(origin, origin + direction * maxDistance, invalidColor, 0.2f, 0.1f);
            }

            target = origin + direction * (isHit ? hitInfo.distance : maxDistance);
            DrawFacingWireArrow(origin, target, color, 0.2f, 0.1f);
            DrawPoint(origin, pointRadius, darkColor);
            DrawPoint(target, pointRadius, darkColor);
        }

        #endregion
    }
}