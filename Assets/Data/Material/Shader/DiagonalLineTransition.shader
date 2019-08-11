Shader "Custom/DiagonalLineTransition"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DiagonalDegree("DiagonalDegree", Range(0,1)) = 0
		[HideInInspector] _Color("Color", Color) = (0,0,0,1)
	}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}

			Lighting Off
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#pragma multi_compile _ REVERSE_ON 

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
				fixed4 _Color;
				float4 _MainTex_TexelSize;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{


	#ifdef REVERSE_ON
					fixed4 col = tex2D(_MainTex, i.uv);
	#else
					fixed4 col = tex2D(_MainTex, i.uv);
	#endif
					return col;
				}
				ENDCG
			}
		}
}
