﻿Shader "Custom/Alpha Transparent" // src https://forum.unity.com/threads/transparency-with-standard-surface-shader.394551/
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags {"RenderType"="AlphaTest" "Queue"="AlphaTest"}
        LOD 200
        Pass {
            ColorMask 0
        }
        // Render normally
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        
        CGPROGRAM
        
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
        
        sampler2D _MainTex;
        
        struct Input {
            float2 uv_MainTex;
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Standard"
}