// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GPVUDK/DoubleSidedSimpleDiffuse"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 300
		Pass
		{
			Cull Front
			CGPROGRAM
			float4 _Color;

			#pragma vertex vert             
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			#include "Lighting.cginc"
			struct vertInput
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct vertOutput
			{
				fixed4 diff : COLOR0; // diffuse lighting color
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
				LIGHTING_COORDS(7,8)
			};

			vertOutput vert(vertInput input)
			{
				vertOutput o;
				o.pos = UnityObjectToClipPos(input.pos);
				o.normal = input.normal * -1;
				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(input.normal);
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				o.diff = nl * _LightColor0;
				return o;
			}

			half4 frag(vertOutput output) : COLOR
			{
				return output.diff*_Color;
			}

			ENDCG
		}
		Pass
		{
			Cull Back
			CGPROGRAM
			float4 _Color;

			#pragma vertex vert             
			#pragma fragment frag
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
			struct vertInput
			{
				float4 pos : POSITION;
				float3 normal : NORMAL;
			};

			struct vertOutput
			{
				fixed4 diff : COLOR0; // diffuse lighting color
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
                LIGHTING_COORDS(7,8)
			};

			vertOutput vert(vertInput input)
			{
				vertOutput o;
				o.pos = UnityObjectToClipPos(input.pos);
				o.normal = input.normal;
                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(input.normal);
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                o.diff = nl * _LightColor0;
				return o;
			}

			half4 frag(vertOutput output) : COLOR
			{
				return output.diff*_Color; 
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
