using UGizmo.Extension.Jobs;
using Unity.Jobs;
using UnityEngine;

namespace UGizmo.Extension
{
    public unsafe class WireArrow2d : PreparingJobScheduler<WireArrow2d, WireArrow2dData>
    {
        public override void Schedule()
        {
            PrimitiveData* triangleBuffer = Gizmo<Triangle, PrimitiveData>.Reserve(InstanceCount);
            LineData* lineBuffer = Gizmo<WireLine, LineData>.Reserve(InstanceCount * 4);

            JobHandle createHandle = new CreateWireArrow2dJob()
            {
                GizmoDataPtr = JobDataPtr,
                TriangleResult = triangleBuffer,
                LineResult = lineBuffer
            }.Schedule(InstanceCount, 16);

            Gizmo<WireLine, LineData>.AddDependency(createHandle);
            Gizmo<Triangle, PrimitiveData>.AddDependency(createHandle);
        }
    }
}