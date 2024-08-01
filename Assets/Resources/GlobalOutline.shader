﻿Shader "Hidden/GlobalOutline"
{
	Properties
{
    _MainTex ("Texture", 2D) = "white" {}
    _OutlineColor("Outline Color", Color) = (0, 1, 0, 1)
    _OutlineThickness("Outline Thickness", Range(0, 10)) = 6
}

SubShader
{
    Tags
    {
        "RenderType" = "Transparent"
        "Queue" = "Overlay+1"
    }

    Cull Off
    ZWrite Off
    ZTest Always
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
        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;

        fixed4 _OutlineColor;
        float _OutlineThickness; // Declare the property here

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);

            fixed leftPixel = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x * _OutlineThickness, 0)).a;
            fixed upPixel = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y * _OutlineThickness)).a;
            fixed rightPixel = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x * _OutlineThickness, 0)).a;
            fixed bottomPixel = tex2D(_MainTex, i.uv + float2(0, -_MainTex_TexelSize.y * _OutlineThickness)).a;

            fixed outline = (1 - leftPixel * upPixel * rightPixel * bottomPixel) * col.a;

            return lerp(col, _OutlineColor, outline);
        }
        ENDCG
    }
}
}
