using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace UGizmo.Internal.Utility
{
    internal static class AssetUtility
    {
        private static readonly int ZTestPropertyId = Shader.PropertyToID("_ZTest");
        private static readonly int ZTestOccludedPropertyId = Shader.PropertyToID("_ZTestOccluded");

        private static readonly Dictionary<string, Material> MaterialInstances = new Dictionary<string, Material>();

        /// <summary>
        /// Mirrors UGizmos.depthTest so that materials created later can be brought in line with it.
        /// </summary>
        private static bool depthTestEnabled;

        /// <summary>
        /// Loads meshes and materials from a given path.
        /// </summary>
        /// <param name="meshPath"></param>
        /// <param name="materialPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal static (Mesh mesh, Material material) CreateMeshAndMaterial(string meshPath, string materialPath)
        {
            const string rootPath = "UGizmo";
            Mesh mesh = Resources.Load<Mesh>(Path.Combine(rootPath, "Meshes", meshPath));

            if (mesh == null)
            {
                Debug.LogError("Mesh is null!");
                throw new ArgumentNullException(nameof(mesh));
            }

            // The cached instance may have been destroyed by Unity without clearing this entry,
            // e.g. exiting Play Mode with domain reload disabled destroys runtime-created
            // Materials while this static dictionary survives. Re-check liveness on every hit.
            if (!MaterialInstances.TryGetValue(materialPath, out Material material) || material == null)
            {
                Material loadedMaterial = Resources.Load<Material>(Path.Combine(rootPath, "Materials", materialPath));

                if (loadedMaterial == null)
                {
                    Debug.LogError("Material is null!");
                    throw new ArgumentNullException(nameof(loadedMaterial));
                }

                material = new Material(loadedMaterial);
                MaterialInstances[materialPath] = material;

                // A fresh instance starts from the shader defaults, which are the always-on-top
                // states. Without this, recreating a material silently reverts UGizmos.depthTest.
                ApplyDepthTest(material, depthTestEnabled);
            }

            return (mesh, material);
        }

        /// <summary>
        /// Applies the depth test mode to every instantiated material.
        /// </summary>
        internal static void ApplyDepthTest(bool depthTest)
        {
            depthTestEnabled = depthTest;

            foreach (Material material in MaterialInstances.Values)
            {
                ApplyDepthTest(material, depthTest);
            }
        }

        /// <summary>
        /// The two properties are declared by different shaders, and setting one a shader does not
        /// declare is a no-op, so a single uniform call covers both families.
        ///
        /// _ZTest belongs to the wire and point shaders, whose single pass draws on top by default.
        /// _ZTestOccluded belongs to the solid shaders, and gates their second pass - the one that
        /// shows occluded parts faintly.
        /// </summary>
        private static void ApplyDepthTest(Material material, bool depthTest)
        {
            // Wire and point: obey depth instead of always winning.
            CompareFunction zTest = depthTest ? CompareFunction.LessEqual : CompareFunction.Always;

            // Solid: drop the see-through pass entirely, so occluded parts are hidden rather than
            // faded. Off, it stays Greater and keeps the original look.
            CompareFunction zTestOccluded = depthTest ? CompareFunction.Never : CompareFunction.Greater;

            material.SetFloat(ZTestPropertyId, (float)zTest);
            material.SetFloat(ZTestOccludedPropertyId, (float)zTestOccluded);
        }

        /// <summary>
        /// Releases every instantiated material.
        /// </summary>
        internal static void Clear()
        {
            foreach (Material material in MaterialInstances.Values)
            {
                UnityEngine.Object.DestroyImmediate(material);
            }

            MaterialInstances.Clear();
        }
    }
}