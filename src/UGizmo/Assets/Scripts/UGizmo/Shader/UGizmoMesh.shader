Shader "UGizmo"
{
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Cull Back
            ZTest LEqual
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1

            Lighting Off
            Fog
            {
                Mode Off
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct RenderData
            {
                float4x4 mat;
                float4 color;
            };

            StructuredBuffer<RenderData> _RenderBuffer;

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
                RenderData render_data = _RenderBuffer[instanceID];

                float4 pos = mul(render_data.mat, IN.positionOS);
                o.positionHCS = TransformObjectToHClip(pos.xyz);

                IN.normal = mul(render_data.mat, IN.normal);

                float strength = dot(IN.normal, -GetViewForwardDir()) * 0.5f + 0.5f;
                o.color.rgb = render_data.color * (strength * strength);
                o.color.a = render_data.color.a;

                return o;
            }

            half4 frag(Varyings v) : SV_Target
            {
                return v.color;
            }
            ENDHLSL
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

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct RenderData
            {
                float4x4 mat;
                float4 color;
            };

            StructuredBuffer<RenderData> _RenderBuffer;

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
                RenderData render_data = _RenderBuffer[instanceID];

                float4 pos = mul(render_data.mat, IN.positionOS);
                o.positionHCS = TransformObjectToHClip(pos.xyz);
                o.color = render_data.color;
                o.color.a *= 0.3;

                return o;
            }

            half4 frag(Varyings v) : SV_Target
            {
                return v.color;
            }
            ENDHLSL
        }
    }
}