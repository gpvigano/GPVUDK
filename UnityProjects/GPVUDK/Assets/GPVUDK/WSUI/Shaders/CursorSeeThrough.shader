// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
// Copyright (C) Microsoft. All rights reserved.
//

Shader "GPVUDK/CursorSeeThrough"
{
    Properties
    {
        [Header(Colors)]
        _Color("Main Color", Color) = (1,1,1,1)
        _ShadowColor("Shadow Color", Color) = (0,0,0,0)
        _HiddenColor("Hidden Color", Color) = (0.25,0.25,0.25,1)
        [Space(20)]
        _ShadowSize("Shadow size", Range(-0.5, 0.5)) = 0.1
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"

        [Header(Base(RGBA))]
        [Toggle] _UseMainTex("Enabled?", Float) = 1
        _MainTex("Base (RGBA)", 2D) = "white" {}
    }

    SubShader
    {
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull[_Cull]

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "Always" "Queue" = "Transparent+1" }
			ZTest LEqual
			ZWrite On

            CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog

				// We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
				#pragma target 5.0
				#pragma only_renderers d3d11
				#include "UnityCG.cginc"

				UNITY_DECLARE_TEX2D(_MainTex);
				float4 _MainTex_ST;
				float4 _Color;
				float4 _ShadowColor;
				float _ShadowSize;
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					float4 c;
					float4 c1;
					float4 c2;
					if (_ShadowColor.a > 0)
					{
						c1 = UNITY_SAMPLE_TEX2D(_MainTex, i.texcoord);
						c1 *= _Color;

						float k = 1.0f - _ShadowSize;
						c2 = UNITY_SAMPLE_TEX2D(_MainTex, (i.texcoord - float2(0.5, 0.5)) * k + float2(0.5, 0.5));
						c2 *= _ShadowColor;

						float t = c1.a;
						c = c1 * t + c2 * (1 - t);
					}
					else
					{
						c = UNITY_SAMPLE_TEX2D(_MainTex, i.texcoord);
						c *= _Color;
					}
					return c;
				}

            ENDCG
        }
		
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "Always" "Queue" = "Transparent+1" }
        ZTest Greater
			ZWrite Off

            CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				// We only target the HoloLens (and the Unity editor), so take advantage of shader model 5.
				#pragma target 5.0
				#pragma only_renderers d3d11
				#include "UnityCG.cginc"

				UNITY_DECLARE_TEX2D(_MainTex);
				float4 _MainTex_ST;
				float4 _HiddenColor;
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					float4 c;

						c = UNITY_SAMPLE_TEX2D(_MainTex, i.texcoord);

						c *= _HiddenColor;

					return c;
				}

            ENDCG
        }
		
    }
}