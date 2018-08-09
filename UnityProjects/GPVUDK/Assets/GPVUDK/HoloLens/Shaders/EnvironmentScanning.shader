// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
// Copyright (C) Microsoft. All rights reserved.
//

Shader "GPVUDK/EnvironmentScanning"
{
	Properties
	{
		_BaseColor("Base color", Color) = (0.0, 0.0, 0.0, 0.5)
		_WireColor("Wire color", Color) = (1.0, 1.0, 1.0, 1.0)
		_WireThickness("Wire thickness", Range(0, 800)) = 100
		_LightColor("Light color", Color) = (0.5, 0.5, 0.5, 0.5)
		_Radius("Light radius", Range(0, 10)) = 1
		_Centre("Light centre", Vector) = (0,0,0,0)

		[Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull", Float) = 2 //"Back"
	}
	SubShader
	{
		LOD 100
		Cull[_Cull]
		Tags{ "RenderType" = "Fade" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _BaseColor;
			float4 _WireColor;
			float _WireThickness;
			float4 _LightColor;
			float _Radius;
			float4 _Centre;

			// Based on approach described in "Shader-Based Wireframe Drawing", http://cgg-journal.com/2008-2/06/index.html

			struct v2g
			{
				float4 viewPos : SV_POSITION;
			};

			v2g vert(appdata_base v)
			{
				v2g o;
				o.viewPos = UnityObjectToClipPos(v.vertex);
				//float3 normal = UnityObjectToViewPos(v.normal);
				//float3 lightPos = unity_LightPosition[0].xyz;
				//half nl = max(0, dot(normal, lightPos));
				//o.diff = nl;// *_LightColor0;
				return o;
			}

			// inverseW is to counter-act the effect of perspective-correct interpolation so that the lines look the same thickness
			// regardless of their depth in the scene.
			struct g2f
			{
				float4 viewPos : SV_POSITION;
				float inverseW : TEXCOORD0;
				float3 dist : TEXCOORD1;
				float4 diff : COLOR0;
			};

			[maxvertexcount(3)]
			void geom(triangle v2g i[3], inout TriangleStream<g2f> triStream)
			{
				// Calculate the vectors that define the triangle from the input points.
				float2 point0 = i[0].viewPos.xy / i[0].viewPos.w;
				float2 point1 = i[1].viewPos.xy / i[1].viewPos.w;
				float2 point2 = i[2].viewPos.xy / i[2].viewPos.w;

				// Calculate the area of the triangle.
				float2 vector0 = point2 - point1;
				float2 vector1 = point2 - point0;
				float2 vector2 = point1 - point0;
				float area = abs(vector1.x * vector2.y - vector1.y * vector2.x);

				float wireScale = 800 - _WireThickness;

				// Output each original vertex with its distance to the opposing line defined
				// by the other two vertices.

				g2f o;
				float r = _Radius;
				float blend = 0.25f;

        float4 centrePos = UnityObjectToClipPos(_Centre);
//        float4 centrePos = _WorldSpaceCameraPos;
				float lp = length(centrePos.xyz- i[0].viewPos);
				float d = abs(lp - r)*blend;
				float vl = max(0, 1 - d); //min(1,r/(lp*lp));
				o.diff = float4(vl,vl,vl,vl);
				o.viewPos = i[0].viewPos;
				o.inverseW = 1.0 / o.viewPos.w;
				o.dist = float3(area / length(vector0), 0, 0) * o.viewPos.w * wireScale;
				triStream.Append(o);

				lp = length(centrePos.xyz - i[1].viewPos);
				d = abs(lp - r)*blend;
				vl = max(0, 1 - d); //min(1,r/(lp*lp));
				o.diff = float4(vl,vl,vl,vl);
				o.viewPos = i[1].viewPos;
				o.inverseW = 1.0 / o.viewPos.w;
				o.dist = float3(0, area / length(vector1), 0) * o.viewPos.w * wireScale;
				triStream.Append(o);

				lp = length(centrePos.xyz - i[2].viewPos);
				d = abs(lp - r)*blend;
				vl = max(0, 1 - d); //min(1,r/(lp*lp));
				o.diff = float4(vl,vl,vl,vl);
				o.viewPos = i[2].viewPos;
				o.inverseW = 1.0 / o.viewPos.w;
				o.dist = float3(0, 0, area / length(vector2)) * o.viewPos.w * wireScale;
				triStream.Append(o);
			}

			float4 frag(g2f i) : COLOR
			{
				// Calculate  minimum distance to one of the triangle lines, making sure to correct
				// for perspective-correct interpolation.
				float dist = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.inverseW;

				// Make the intensity of the line very bright along the triangle edges but fall-off very
				// quickly.
				float I = exp2(-2 * dist * dist);

				// Fade out the alpha but not the color so we don't get any weird halo effects from
				// a fade to a different color.
				float4 color = I * _WireColor + (1 - I) * _BaseColor * _LightColor;
				//color.a = I * _WireColor.a + (1 - I) * _BaseColor.a;
				color *= i.diff;
				return color;
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}