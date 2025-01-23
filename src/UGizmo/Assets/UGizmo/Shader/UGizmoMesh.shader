Shader "UGizmo/Mesh"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "Queue" = "Transparent"
        }


        Pass
        {
            Cull Back
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
                float4x4 mat;
                float4 color;
            };

            StructuredBuffer<DrawData> _DrawBuffer;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
            };

            Varyings vert(Attributes IN, uint instanceID : SV_InstanceID)
            {
                Varyings o;
                DrawData drawData = _DrawBuffer[instanceID];

                float4 pos = mul(drawData.mat, IN.positionOS);
                o.positionHCS = UnityObjectToClipPos(pos.xyz);

                IN.normal = mul(drawData.mat, IN.normal);

                float strength = dot(IN.normal, -UNITY_MATRIX_V[2].xyz) * 0.15f + 0.85f;
                o.color.rgb = drawData.color * (strength * strength);
                o.color.a = drawData.color.a;

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
            Cull Back
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
                float4x4 mat;
                float4 color;
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
                DrawData drawData = _DrawBuffer[instanceID];

                float4 pos = mul(drawData.mat, IN.positionOS);
                o.positionHCS = UnityObjectToClipPos(pos.xyz);
                o.color = drawData.color;
                o.color.a *= 0.3;

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