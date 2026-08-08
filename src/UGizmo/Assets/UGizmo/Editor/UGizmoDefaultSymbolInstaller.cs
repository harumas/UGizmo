using UnityEditor;
using UnityEditor.Build;

namespace UGizmo.Editor
{
    /// <summary>
    /// Defines the UGIZMO symbol once, the first time the package is loaded in a project, so gizmos
    /// show up in release builds without the user having to discover the symbol first.
    ///
    /// It has to be a define symbol rather than a default baked into the package, because
    /// [Conditional] is evaluated against the symbols of the assembly that *calls* the draw method,
    /// and it cannot express a negative condition. Stripping the calls - arguments included - is
    /// only possible through a symbol that the calling assembly can see.
    /// </summary>
    [InitializeOnLoad]
    internal static class UGizmoDefaultSymbolInstaller
    {
        private const string Symbol = "UGIZMO";

        static UGizmoDefaultSymbolInstaller()
        {
            // Player settings are not reliably writable while the domain is still loading.
            EditorApplication.delayCall += ApplyDefault;
        }

        private static void ApplyDefault()
        {
            // Applied once per project per machine. Without this, unticking the menu item would be
            // undone on the next domain reload and the toggle would be unusable.
            string key = $"UGizmo.DefaultSymbolApplied.{PlayerSettings.productGUID}";

            if (EditorPrefs.GetBool(key, false))
            {
                return;
            }

            EditorPrefs.SetBool(key, true);

            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;

            if (group == BuildTargetGroup.Unknown)
            {
                return;
            }

            NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(group);
            string defined = PlayerSettings.GetScriptingDefineSymbols(buildTarget);

            foreach (string symbol in defined.Split(';'))
            {
                if (symbol.Trim() == Symbol)
                {
                    return;
                }
            }

            string separator = string.IsNullOrWhiteSpace(defined) ? string.Empty : ";";
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, defined + separator + Symbol);
        }
    }
}
