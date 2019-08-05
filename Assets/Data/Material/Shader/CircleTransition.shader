Shader "Custom/CircleTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ArchSize("ArchSize", Float) = 240
		[HideInInspector] _CurAngle("CurAngle", Float) = 0
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
			float _CurAngle;
			float _ArchSize;
			fixed4 _Color;
			float4 _MainTex_TexelSize;

			float calAngle(half2 vec1, half2 vec2, half2 orgPos)
			{
				float angle = degrees(acos(dot(vec1, vec2)));
				half2 ver = half2(vec1.y, -vec1.x);
				float angle2 = degrees(acos(dot(ver, vec2)));

				if (angle2 > 90)
					angle = 360 - angle;

				return angle;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				half2 center = half2(0.5, 0.5);
				half2 pixelPos = i.uv;
				half2 topPos = half2(0.5, 1);
				half2 axis = topPos - center;
				half2 curDir = pixelPos - center;

				axis = normalize(axis);
				curDir = normalize(curDir);

				float angle = calAngle(axis, curDir, i.uv);
				fixed4 final = _Color;

				float stage = _CurAngle / 5;
				float curStage = floor(angle / stage);
				curStage /= 5;

#ifdef REVERSE_ON
				fixed4 org = tex2D(_MainTex, i.uv);
				org.a = 1 - curStage;
                fixed4 col = angle <= _CurAngle ? org : final;
#else
				final.a = 1 - curStage;
				fixed4 col = angle > _CurAngle ? tex2D(_MainTex, i.uv) : final;
#endif
                return col;
            }
            ENDCG
        }
    }
}
