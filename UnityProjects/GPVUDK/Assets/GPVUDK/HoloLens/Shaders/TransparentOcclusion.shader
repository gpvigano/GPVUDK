Shader "GPVUDK/TransparentOcclusion"
{
	Properties
	{
		_BaseColor("Base color", Color) = (0.0, 0.0, 0.0, 0.5)
	}
		SubShader
	{
		LOD 100
		Cull Back
		Tags{ "RenderType" = "Fade" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

			float4 _BaseColor;

			struct v2g
			{
				float4 viewPos : SV_POSITION;
			};

			v2g vert(appdata_base v)
			{
				v2g o;
				o.viewPos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

			float4 frag(v2f i) : COLOR
			{
				return _BaseColor;
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}