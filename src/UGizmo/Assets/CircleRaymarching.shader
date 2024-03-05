Shader "Unlit/CircleRaymarching"
{
    Properties
    {
        _Radius ("Radius", Float) = 1.0
        _Width ("Width", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            Cull Off
            AlphaToMask On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 positionCS : POSITION1;
                float2 uv : TEXCOORD0;
            };


            float _Radius;
            float _Width;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.positionCS = mul(unity_ObjectToWorld, v.positionOS);
                o.uv = v.uv;
                return o;
            }


            float torus(float3 p, const float w)
            {
                const float2 q = float2(length(p.xy) - _Radius, p.z);
                const float2 r = float2(length(p.yz) - _Radius, p.x);
                const float2 s = float2(length(p.xz) - _Radius, p.y);

                return min(min(length(q), length(r)), length(s)) - _Width * w;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float3 position = i.positionCS.xyz;
                float3 dir = normalize(position.xyz - _WorldSpaceCameraPos);
                half4 color = half4(0, 0, 0, 0);

                for (int j = 0; j < 32; j++)
                {
                    float distance = torus(position, i.positionHCS.w);

                    if (distance < 0.0001)
                    {
                        color = half4(1, 1, 1, 1);
                        break;
                    }

                    position += distance * dir;
                }

                return color;
            }
            ENDHLSL
        }
    }
}