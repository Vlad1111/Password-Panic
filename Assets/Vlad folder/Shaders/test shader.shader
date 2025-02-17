Shader "Unlit/test shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
            };

            sampler2D _MainTex;
			sampler2D _AlphaTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
					if (_AlphaSplitEnabled)
						color.a = tex2D (_AlphaTex, uv).r;
				#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = SampleSpriteTexture (i.texcoord) * i.color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                //col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
}
