Shader "Custom/Fog"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", float) = 1
        _Origin ("Origin", Vector) = (0, 1, 0)
        _Direction ("Light Direction", Vector) = (0, -1, 0)
        _Density ("Density", float) = 1
        _Steps ("Step Count", int) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        // Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float4 screenSpace : TEXCOORD1;
                float3 viewVector : TEXCOORD2;
                float3 originPoint : TEXCOORD3;
                float3 lightDirection : TEXCOORD4;
            };

            float4 _BaseColor;
            float _Radius;
            float3 _Origin;
            float3 _Direction;
            int _Steps;

            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenSpace = ComputeScreenPos(o.vertex);

                o.originPoint = mul(unity_ObjectToWorld, float4(_Origin, 1));
                o.lightDirection = mul(unity_ObjectToWorld, _Direction);

                return o;
            }

            float2 solveQuadratic(float a, float b, float c)
            {
                float discr = b * b - 4 * a * c;
                if (discr < 0) return 0;
                if (discr == 0) return -0.5 * b / a;
                
                float sqrtDiscr = sqrt(discr);

                float x = (-b - sqrtDiscr) / (a * 2);
                float y = (-b + sqrtDiscr) / (a * 2);
                return (x, y);
            }

            float2 checkSphere(float3 origin, float radius, float rayOrigin, float rayDir)
            {
                float3 L = rayOrigin - origin;
                float a = dot(rayDir, rayDir);
                float b = 2 * dot(rayDir, L);
                float c = dot(L, L) - radius * radius;

                return solveQuadratic(a, b, c);
            }

            float calculateLight(float3 startPoint, float3 rayDir, float rayLength, float3 lightOrigin, float3 lightDirection)
            {
                float3 samplePoint = startPoint;
                float stepSize = rayLength / (_Steps - 1);

                for(int i = 0; i < _Steps; i++)
                {
                    float3 originDir = lightOrigin - samplePoint;

                    samplePoint += rayDir * stepSize;
                }
                return 1;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //Attempt at volumetric fog, something here is broken
                float3 cameraPos = _WorldSpaceCameraPos;
                float3 viewVector = i.worldPos - cameraPos;

                float2 screenSpaceUV = i.screenSpace.xy / i.screenSpace.w;
                float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenSpaceUV);
                float sceneDepth = LinearEyeDepth(sceneDepthNonLinear);

                float3 rayDir = normalize(viewVector);

                float2 hitInfo = checkSphere(i.originPoint, _Radius, cameraPos, rayDir);
                float dstToFog = hitInfo.x;
                float dstThroughFog = min(hitInfo.y, sceneDepth) - dstToFog;

                // return fixed4(hitInfo.y - hitInfo.x, 0, 0, 1);
                return fixed4(dstThroughFog, 0, 0, 1);

                if(dstThroughFog > 0)
                {
                    float3 pointOnFog = cameraPos + rayDir * dstToFog;
                    float light = calculateLight(pointOnFog, rayDir, dstThroughFog, i.originPoint, i.lightDirection);
                    // return _BaseColor * (1 - light) + light;
                    return _BaseColor;
                }
                return 0;
            }
            ENDCG
        }
    }
}
