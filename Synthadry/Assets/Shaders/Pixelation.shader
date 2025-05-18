Shader "Custom/PostProcess/Pixelation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize ("Pixel Size", Float) = 8.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }

        Pass
        {
            Name "PixelationPass"
            ZTest Always Cull Off ZWrite Off
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _PixelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 pixelUV = floor(i.uv * _MainTex_TexelSize.zw / _PixelSize) * _PixelSize * _MainTex_TexelSize.xy;
                return tex2D(_MainTex, pixelUV);
            }

            ENDHLSL
        }
    }
}
