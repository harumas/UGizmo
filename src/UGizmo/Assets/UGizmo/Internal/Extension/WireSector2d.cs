using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal unsafe class WireSector2d : PreparingJobScheduler<WireSector2d, Sector2dData>
    {
        public override JobHandle Schedule()
        {
            int totalLines = 0;
            for (int i = 0; i < InstanceCount; i++)
            {
                JobDataPtr[i].StartIndex = totalLines;
                totalLines += (JobDataPtr[i].Segments + 2);
            }

            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(totalLines);

            JobHandle jobHandle = new CreateWireSector2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}
