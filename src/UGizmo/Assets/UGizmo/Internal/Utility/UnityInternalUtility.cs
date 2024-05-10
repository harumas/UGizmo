using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UGizmo.Internal.Utility
{
    public static class UnityInternalUtility
    {
        private static readonly MethodInfo setNativeDataMethod;

        static UnityInternalUtility()
        {
            setNativeDataMethod = typeof(GraphicsBuffer).GetRuntimeMethods().First(method => method.Name == "InternalSetNativeData");
        }
        
        public static Action<IntPtr, int, int, int, int> CreateInternalSetDataDelegate(GraphicsBuffer graphicsBuffer)
        {
            Type targetType = typeof(Action<IntPtr, int, int, int, int>);
            return (Action<IntPtr, int, int, int, int>)Delegate.CreateDelegate(targetType, graphicsBuffer, setNativeDataMethod);
        }
    }
}