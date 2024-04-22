Shader "Custom/DistanceBasedBrightness"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MinDistance ("Min Distance", Float) = 0.0
        _MaxDistance ("Max Distance", Float) = 10.0
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
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            uniform float4 _Color;
            uniform float _MinDistance;
            uniform float _MaxDistance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float distance = length(_WorldSpaceCameraPos - i.worldPos);
                float brightness = saturate(1 - (distance - _MinDistance) / (_MaxDistance - _MinDistance));
                return _Color * brightness;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}