Shader "Custom/GrayScale"
{
	Properties
	{
		[PerRendererData] _MainTex("Texture", 2D) = "white" {}
		_Intensity("Intensity",Range(0.0,1)) = 1.0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#pragma shader_feature
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Intensity;

			struct VertexInput
			{
				float4 pos: POSITION;
				float4 color : COLOR;
				float2 uv: TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 sv_pos: SV_POSITION;
				fixed4 color : COLOR;
				float2 uv: TEXCOORD0;
			};

			VertexOutput vert(VertexInput input)
			{
				VertexOutput output;
				output.sv_pos = UnityObjectToClipPos(input.pos);
				output.uv = input.uv;
				output.color = input.color;

				return output;
			}

			fixed4 frag (VertexOutput output) : SV_Target
			{
				half4 tex = tex2D(_MainTex, output.uv) * output.color;
				tex.rgb = dot(tex.rgb, float3(0.3, 0.59, 0.11)) * _Intensity;
 				return tex;
			}
			ENDCG
		}
	}
}
