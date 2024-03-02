using System;
using System.Reflection;
using UnityEditor;

namespace UGizmos
{
    public static class GameViewUtility
    {
        public static readonly Func<bool> IsShowingGizmos;

        static GameViewUtility()
        {
            string assemblyName = "UnityEditor.dll";
            Assembly assembly = Assembly.Load(assemblyName);
            Type gameView = assembly.GetType("UnityEditor.PlayModeView");
            EditorWindow editorWindow = EditorWindow.GetWindow(gameView);
            MethodInfo method = gameView.GetMethod("IsShowingGizmos", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance);
            IsShowingGizmos = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), editorWindow, method);
        }
    }
}