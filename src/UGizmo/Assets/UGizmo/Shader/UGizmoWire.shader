Shader "UGizmo/Wire"
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
            ZTest Always
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