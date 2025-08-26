Shader "Custom/SphericalFadeShader"
{
    Properties
    {
        _FadeAmount ("Fade Amount", Range(0, 1)) = 0
        _FadeColor ("Fade Color", Color) = (0, 0, 0, 1)
        _FadeExponent ("Fade Exponent", Float) = 2.0
    }
    
    SubShader
    {
        Tags {"Queue"="Overlay" "RenderType"="Transparent"}
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _FadeAmount;
            float4 _FadeColor;
            float _FadeExponent;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 centeredUV = i.uv * 2 - 1;
                float distSquared = dot(centeredUV, centeredUV);
                float fadeStrength = pow(distSquared, _FadeExponent) * _FadeAmount;
                fadeStrength = saturate(fadeStrength);
                
                return float4(_FadeColor.rgb, fadeStrength * _FadeColor.a);
            }
            ENDCG
        }
    }
}