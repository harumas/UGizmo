using UGizmo.Internal.Extension.Jobs;
using Unity.Jobs;

namespace UGizmo.Internal.Extension
{
    internal unsafe class Sector2d : PreparingJobScheduler<Sector2d, Sector2dData>
    {
        public override JobHandle Schedule()
        {
            int totalTriangles = 0;
            for (int i = 0; i < InstanceCount; i++)
            {
                JobDataPtr[i].StartIndex = totalTriangles;
                totalTriangles += JobDataPtr[i].Segments;
            }

            PrimitiveData* triangleBuffer = Gizmo<Triangle, PrimitiveData>.Reserve(totalTriangles);

            JobHandle jobHandle = new CreateSector2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                TriangleResult = triangleBuffer
            }.Schedule(InstanceCount, 16);

            return jobHandle;
        }
    }
}
