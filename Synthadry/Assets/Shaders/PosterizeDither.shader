Shader "Custom/PosterizeDither"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PosterizeLevels ("Posterize Levels", Range(2, 32)) = 4
        _DitherAmount ("Dither Amount", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PosterizeLevels;
            float _DitherAmount;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float GetBayerValue(int x, int y)
            {
                // Bayer 4x4 matrix values / 16
                float bayer[16] = {
                    0.0 / 16.0,  8.0 / 16.0,  2.0 / 16.0, 10.0 / 16.0,
                    12.0 / 16.0, 4.0 / 16.0, 14.0 / 16.0,  6.0 / 16.0,
                    3.0 / 16.0, 11.0 / 16.0,  1.0 / 16.0,  9.0 / 16.0,
                    15.0 / 16.0, 7.0 / 16.0, 13.0 / 16.0,  5.0 / 16.0
                };
                return bayer[y * 4 + x];
            }

            float BayerDither4x4(float2 uv)
            {
                int x = (int)fmod(floor(uv.x * 4.0), 4.0);
                int y = (int)fmod(floor(uv.y * 4.0), 4.0);
                return GetBayerValue(x, y);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Постеризация
                col.rgb = floor(col.rgb * _PosterizeLevels) / (_PosterizeLevels - 1.0);

                // Дезеринг
                float dither = BayerDither4x4(i.uv * _PosterizeLevels);
                col.rgb += (dither - 0.5) * _DitherAmount / _PosterizeLevels;

                return saturate(col);
            }
            ENDCG
        }
    }
}

