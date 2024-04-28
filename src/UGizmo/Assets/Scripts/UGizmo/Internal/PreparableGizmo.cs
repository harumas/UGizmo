using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal static class PreparableGizmo<TScheduler, TPrepareData>
        where TScheduler : PreparingJobScheduler<TScheduler, TPrepareData>, new()
        where TPrepareData : unmanaged
    {
        private static TScheduler scheduler;

        public static void Initialize(TScheduler gizmo)
        {
            scheduler = gizmo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TPrepareData data)
        {
            scheduler.Add(data);
        }
    }
}