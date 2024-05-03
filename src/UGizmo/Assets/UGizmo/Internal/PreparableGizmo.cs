using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal static class PreparableGizmo<TScheduler, TPrepareData>
        where TScheduler : PreparingJobScheduler<TScheduler, TPrepareData>, new()
        where TPrepareData : unmanaged
    {
        private static TScheduler scheduler;
        private static bool isInitialized;

        public static void Initialize(TScheduler gizmo)
        {
            scheduler = gizmo;
            isInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TPrepareData data)
        {
            if (!isInitialized)
            {
                return;
            }
            
            scheduler.Add(data);
        }
    }
}