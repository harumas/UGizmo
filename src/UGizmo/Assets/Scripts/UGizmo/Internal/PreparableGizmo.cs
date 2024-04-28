using System.Runtime.CompilerServices;

namespace UGizmo.Internal
{
    internal static class PreparableGizmo<TPreparingJobScheduler, TPrepareData>
        where TPreparingJobScheduler : PreparingJobScheduler<TPreparingJobScheduler, TPrepareData>, new()
        where TPrepareData : unmanaged
    {
        private static TPreparingJobScheduler preparableGizmo;

        public static void Initialize(TPreparingJobScheduler gizmo)
        {
            preparableGizmo = gizmo;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddData(in TPrepareData data)
        {
            preparableGizmo.Add(data);
        }
    }
}