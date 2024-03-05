Shader "Sample/DrawMeshInstancedSampleShader"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        Pass
        {
            HLSLPROGRAM
            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                uint instancedId : SV_InstanceID;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
            };

            // C#側から座標情報が渡される
            StructuredBuffer<float3> _Positions;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionOS = IN.positionOS.xyz + _Positions[IN.instancedId];
                OUT.vertex = TransformWorldToHClip(positionOS);
                return OUT;
            }

            half4 frag() : SV_Target
            {
                return half4(0, 0, 0.5, 1);
            }
            ENDHLSL
        }
    }
}