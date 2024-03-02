using UGizmos;
using UGizmos.Extension;
using Unity.Jobs;

[assembly: RegisterGenericJobType(typeof(CreateRenderDataJob<NoCustom>))]

namespace UGizmos.Extension
{
    public readonly struct NoCustom
    {
        public static readonly NoCustom None = new NoCustom();
    }
}