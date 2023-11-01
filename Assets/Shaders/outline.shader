Shader "Toon/Outline"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.01
        _OutlineTransparency ("Outline Transparency", Range(0, 1)) = 1
        _MainTex ("Texture", 2D) = "white" {}
        _MainTexScale ("Texture Scale", Vector) = (1,1,0,0)
        _MainTexOffset ("Texture Offset", Vector) = (0,0,0,0)
        _ToonThreshold ("Toon Threshold", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
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
            Name "OUTLINE"
            Tags
            {
                "LightMode" = "Always"
            }

            ZWrite Off
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            float _OutlineTransparency;
            float2 _MainTexScale;
            float2 _MainTexOffset;

            v2f vert(appdata v)
            {
                v2f o;
                float3 viewDir = normalize(_WorldSpaceCameraPos - v.vertex.xyz);
                float dotProduct = dot(v.normal, viewDir);
                if (dotProduct < 0.0)
                {
                    v.vertex.xyz += v.normal * _OutlineWidth;
                }
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTexScale + _MainTexOffset;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return fixed4(_OutlineColor.rgb, _OutlineColor.a * _OutlineTransparency);
            }
            ENDCG
        }

        Pass
        {
            Name "BASE"
            Tags
            {
                "LightMode" = "ForwardBase"
            }

            ZWrite On
            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 pos : SV_POSITION; // Screen space position
                float3 worldPos : WORLDPOS; // World space position
            };

            sampler2D _MainTex;
            float4 _MainColor;
            float _ToonThreshold;
            float _Smoothness;
            float2 _MainTexScale;
            float2 _MainTexOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTexScale + _MainTexOffset;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed3 normal = normalize(i.normal);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 diffuse = _LightColor0.rgb * _MainColor.rgb * tex.rgb * smoothstep(
                    _ToonThreshold - _Smoothness, _ToonThreshold + _Smoothness, dot(normal, lightDir));
                return fixed4(diffuse, _MainColor.a * tex.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}