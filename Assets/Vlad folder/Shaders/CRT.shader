Shader "Unlit/CRT_Unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanLineAlpha ("Scan Line Alpha", Range(0, 1)) = 0.5
        _ScanSpeed ("Scan Speed", Range(0, 1)) = 0.5
        _ScanLineSize ("Scan Line Size", Range(0, 2)) = 0.1
        _CutoutThreshold ("Cutout Threshold", Range(0, 1)) = 0.25
    }
    SubShader
    {
        // Originally "Queue"="Transparent"
        // Might need to use the "Overlay" queue so the effect is on top of the UI
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // Since it's a semitransparent effect, we need to disable the ZWrite
        ZWrite Off

        // Traditional transparency blending options (using alpha channel)
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST; // don't remove this
            float _ScanLineAlpha;
            float _ScanSpeed;
            float _ScanLineSize;
            float _CutoutThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Method A - Simple and looks really nice

                // fixed4 col = fixed4(0, 0, 0, 1);
                // float scanline = 0.5 + 0.5 * sin(i.uv.y * _ScanLineSize + _Time.y * _ScanSpeed * 10);
                // col.a *= _ScanLineAlpha;
                // col.a *= scanline;

                // Method B - More complex but more realistic

                fixed4 col = tex2D(_MainTex, (i.uv + _Time.y * _ScanSpeed) * _ScanLineSize);
                col.a = col.r * _ScanLineAlpha;
                col.rgb = fixed3(0, 0, 0);

                // Not needed but might be useful for other methods of creating the effect
                // if (col.a > _CutoutThreshold) discard;
                return col;
            }
            ENDCG
        }
    }
}
