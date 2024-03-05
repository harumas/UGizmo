using System;
using System.Runtime.CompilerServices;
using UGizmo.Extension;
using UGizmo.Extension.Jobs;
using UnityEngine;

namespace UGizmo
{
    public static class UGizmos
    {
        public static Color DefaultColor
        {
            get => defaultColor;
            set => defaultColor = value;
        }

        private static Color defaultColor = Color.red;

        #region WireSphere

        public static void DrawWireSphere(Vector3 position, float radius)
        {
            QueueWireSphere(position, Quaternion.identity, radius, defaultColor);
        }

        public static void DrawWireSphere(Vector3 position, float radius, Color color)
        {
            QueueWireSphere(position, Quaternion.identity, radius, color);
        }

        #endregion

        #region WireCube

        public static void DrawWireCube(Vector3 position, float size)
        {
            QueueDrawWireCube(position, new Vector3(size, size, size), defaultColor);
        }

        public static void DrawWireCube(Vector3 position, float size, Color color)
        {
            QueueDrawWireCube(position, new Vector3(size, size, size), color);
        }

        public static void DrawWireCube(Vector3 position, Vector3 size)
        {
            QueueDrawWireCube(position, size, defaultColor);
        }

        public static void DrawWireCube(Vector3 position, Vector3 size, Color color)
        {
            QueueDrawWireCube(position, size, color);
        }

        #endregion

        #region Line

        public static void DrawLine(Vector3 from, Vector3 to)
        {
            QueueDrawWireLine(from, to, Color.red);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            QueueDrawWireLine(from, to, color);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void QueueWireSphere(Vector3 position, Quaternion rotation, float radius, Color color)
        {
            var data = new PrimitiveData(position, rotation, new Vector3(radius, radius, radius), color);
            Gizmo<WireSphere, PrimitiveData>.AddData(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void QueueDrawWireLine(Vector3 from, Vector3 to, Color color)
        {
            var data = new LineData(from, to, color);
            Gizmo<WireLine, LineData>.AddData(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void QueueDrawWireCube(Vector3 position, Vector3 size, Color color)
        {
            var data = new PrimitiveData(position, Quaternion.identity, size, color);
            Gizmo<WireCube, PrimitiveData>.AddData(data);
        }
    }
}