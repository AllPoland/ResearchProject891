Shader "Custom/CRT"
{
    Properties
    {
        _LineCount ("Lines", Float) = 1.0
        _SineAmp ("Sine Amplitude", Float) = 1.0
        _CosAmp ("Cosine Amplitude", Float) = 1.0
        _SineOffset ("Sine Brightness", Float) = 0.0
        _CosOffset ("Cosine Brightness", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend DstColor Zero

        pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _LineCount;
            float _SineAmp;
            float _CosAmp;
            float _SineOffset;
            float _CosOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 1;

                float sine = (sin(i.uv.y * _LineCount) * _SineAmp) + _SineOffset;
                float cosine = (cos(i.uv.y * _LineCount) * _CosAmp) + _CosOffset;

                col.r *= cosine;
                col.g *= sine;
                col.b *= cosine;

                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
