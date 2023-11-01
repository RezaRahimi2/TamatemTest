Shader "Billboard/ColorCutout"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        _Fade ("Fade", Range(0,1)) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha // Enable blending
        ZWrite Off // Disable writing to the Z buffer

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma alpha:fade

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
            float4 _MainColor;
            float _Cutoff;
            float _Fade;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                tex.rgb += _MainColor.rgb; // Add the color
                tex.a *= _MainColor.a * _Fade; // Apply the alpha value of _MainColor and the fade
                clip(tex.a - _Cutoff); // Discard the fragment if its alpha is below _Cutoff
                return tex;
            }
            ENDCG
        }
    }
}