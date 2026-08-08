using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;

namespace UGizmo.Editor
{
    /// <summary>
    /// Toggles the UGIZMO scripting define symbol for the build target currently selected in Build
    /// Settings. Draw calls are stripped from release builds by default, and a release build with
    /// no gizmos is indistinguishable from a broken one - no error, no warning, nothing drawn - so
    /// this exposes the opt-in as a checkable menu item instead of a symbol you have to know about
    /// and type by hand.
    /// </summary>
    internal static class UGizmoDefineSymbolMenu
    {
        private const string MenuPath = "Tools/UGizmo/Keep Gizmos in Release Builds";
        private const string Symbol = "UGIZMO";

        [MenuItem(MenuPath)]
        private static void Toggle()
        {
            if (!TryGetSelectedBuildTarget(out NamedBuildTarget buildTarget))
            {
                return;
            }

            List<string> symbols = GetSymbols(buildTarget);

            // RemoveAll rather than Remove: the symbol may already be listed more than once if it
            // was entered by hand in Player Settings.
            if (symbols.Contains(Symbol))
            {
                symbols.RemoveAll(symbol => symbol == Symbol);
            }
            else
            {
                symbols.Add(Symbol);
            }

            PlayerSettings.SetScriptingDefineSymbols(buildTarget, string.Join(";", symbols));
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleValidate()
        {
            bool isDefined = TryGetSelectedBuildTarget(out NamedBuildTarget buildTarget)
                             && GetSymbols(buildTarget).Contains(Symbol);

            Menu.SetChecked(MenuPath, isDefined);

            return true;
        }

        private static bool TryGetSelectedBuildTarget(out NamedBuildTarget buildTarget)
        {
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;

            if (group == BuildTargetGroup.Unknown)
            {
                buildTarget = default;

                return false;
            }

            buildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

            return true;
        }

        private static List<string> GetSymbols(NamedBuildTarget buildTarget)
        {
            string defined = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            List<string> symbols = new List<string>();

            // Symbols typed by hand can carry surrounding spaces. Without trimming, an existing
            // " UGIZMO " would not match, so the menu would read as unchecked and toggling it would
            // append a second copy of the symbol.
            foreach (string symbol in defined.Split(';'))
            {
                string trimmed = symbol.Trim();

                if (trimmed.Length == 0)
                {
                    continue;
                }

                symbols.Add(trimmed);
            }

            return symbols;
        }
    }
}
