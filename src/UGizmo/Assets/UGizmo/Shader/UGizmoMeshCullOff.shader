Shader "UGizmo/MeshCullOff"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "Queue" = "Transparent"
        }

        Pass
        {
            Cull Off
            ZTest LEqual
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

            Lighting Off
            Fog
            {
                Mode Off
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct DrawData
            {
                float m00, m10, m20;
                float m01, m11, m21;
                float m02, m12, m22;
                float m03, m13, m23;
                uint color;
            };

            StructuredBuffer<DrawData> _DrawBuffer;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 Normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
            };

            Varyings vert(Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings o;
                DrawData rawData = _DrawBuffer[instanceID];

                float4x4 drawData_mat = float4x4(
                    rawData.m00, rawData.m01, rawData.m02, rawData.m03,
                    rawData.m10, rawData.m11, rawData.m12, rawData.m13,
                    rawData.m20, rawData.m21, rawData.m22, rawData.m23,
                    0, 0, 0, 1
                );
                float4 drawData_color = float4(
                    (rawData.color & 0xFF) / 255.0,
                    ((rawData.color >> 8) & 0xFF) / 255.0,
                    ((rawData.color >> 16) & 0xFF) / 255.0,
                    ((rawData.color >> 24) & 0xFF) / 255.0
                );

                float4 pos = mul(drawData_mat, IN.positionOS);
                o.positionHCS = UnityObjectToClipPos(pos.xyz);
                o.color = drawData_color;

                return o;
            }

            half4 frag(Varyings v) : SV_Target
            {
                return v.color;
            }
            ENDCG
        }

        Pass
        {
            Cull Off
            ZTest Greater
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

            Lighting Off
            Fog
            {
                Mode Off
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct DrawData
            {
                float m00, m10, m20;
                float m01, m11, m21;
                float m02, m12, m22;
                float m03, m13, m23;
                uint color;
            };

            StructuredBuffer<DrawData> _DrawBuffer;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
            };

            Varyings vert(Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings o;
                DrawData rawData = _DrawBuffer[instanceID];

                float4x4 drawData_mat = float4x4(
                    rawData.m00, rawData.m01, rawData.m02, rawData.m03,
                    rawData.m10, rawData.m11, rawData.m12, rawData.m13,
                    rawData.m20, rawData.m21, rawData.m22, rawData.m23,
                    0, 0, 0, 1
                );
                float4 drawData_color = float4(
                    (rawData.color & 0xFF) / 255.0,
                    ((rawData.color >> 8) & 0xFF) / 255.0,
                    ((rawData.color >> 16) & 0xFF) / 255.0,
                    ((rawData.color >> 24) & 0xFF) / 255.0
                );

                float4 pos = mul(drawData_mat, IN.positionOS);
                o.positionHCS = UnityObjectToClipPos(pos.xyz);
                o.color = drawData_color;
                o.color.a *= 0.4;

                return o;
            }

            half4 frag(Varyings v) : SV_Target
            {
                return v.color;
            }
            ENDCG
        }
    }
}