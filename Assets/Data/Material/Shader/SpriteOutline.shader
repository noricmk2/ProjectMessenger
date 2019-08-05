// Draws an HDR outline over the sprite borders. 
// Based on Sprites/Default shader from Unity 2017.3.

Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[Toggle(OUTLINE_OUTSIDE)] _OutlineOutside("Outside", Float) = 0
		[MaterialToggle] _IsOutlineEnabled("Enable Outline", float) = 0
		[HDR] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineSize("Outline Size", Range(1, 10)) = 1
		_AlphaThreshold("Alpha Threshold", Range(0, 1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex ComputeVertex
            #pragma fragment ComputeFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ OUTLINE_OUTSIDE 
			#pragma multi_compile _ SHUT_OFF 

            #ifndef SAMPLE_DEPTH_LIMIT
            #define SAMPLE_DEPTH_LIMIT 10
            #endif

            fixed4 _OutlineColor;
            float _IsOutlineEnabled, _OutlineSize, _AlphaThreshold;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;

            struct VertexInput
            {
                float4 Vertex : POSITION;
                float4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 Vertex : SV_POSITION;
                fixed4 Color : COLOR;
                float2 TexCoord : TEXCOORD0;
            };

            VertexOutput ComputeVertex(VertexInput vertexInput)
            {
                VertexOutput vertexOutput;

                vertexOutput.Vertex = UnityObjectToClipPos(vertexInput.Vertex);
                vertexOutput.TexCoord = vertexInput.TexCoord;
                vertexOutput.Color = vertexInput.Color * _Color;

                #ifdef PIXELSNAP_ON
                vertexOutput.Vertex = UnityPixelSnap(vertexOutput.Vertex);
                #endif

                return vertexOutput;
            }

            int ShouldDrawOutlineInside (fixed4 sampledColor, float2 texCoord, int isOutlineEnabled, int outlineSize, float alphaThreshold)
            {
                if (isOutlineEnabled * outlineSize * sampledColor.a == 0) return 0;

                float2 texDdx = ddx(texCoord);
                float2 texDdy = ddy(texCoord);

                for (int i = 1; i <= SAMPLE_DEPTH_LIMIT; i++)
                {
					float2 pixelUpTexCoord = texCoord + float2(0, i * _MainTex_TexelSize.y);
					fixed pixelUpAlpha = pixelUpTexCoord.y > 1.0 ? 0.0 : tex2D(_MainTex, pixelUpTexCoord).a;
					if (pixelUpAlpha <= alphaThreshold) return i;

					float2 pixelDownTexCoord = texCoord - float2(0, i * _MainTex_TexelSize.y);
					fixed pixelDownAlpha = pixelDownTexCoord.y < 0.0 ? 0.0 : tex2D(_MainTex, pixelDownTexCoord).a;
					if (pixelDownAlpha <= alphaThreshold) return i;

					float2 pixelRightTexCoord = texCoord + float2(i * _MainTex_TexelSize.x, 0);
					fixed pixelRightAlpha = pixelRightTexCoord.x > 1.0 ? 0.0 : tex2D(_MainTex, pixelRightTexCoord).a;
					if (pixelRightAlpha <= alphaThreshold) return i;

					float2 pixelLeftTexCoord = texCoord - float2(i * _MainTex_TexelSize.x, 0);
					fixed pixelLeftAlpha = pixelLeftTexCoord.x < 0.0 ? 0.0 : tex2D(_MainTex, pixelLeftTexCoord).a;
					if (pixelLeftAlpha <= alphaThreshold) return i;

                    if (i > outlineSize) break;
                }

                return 0;
            }

            // Determines whether _OutlineColor should replace sampledColor at the given texCoord when drawing outside the sprite borders.
            // Will return 1 when the test is positive (should draw outline), 0 otherwise.
            int ShouldDrawOutlineOutside (fixed4 sampledColor, float2 texCoord, int isOutlineEnabled, int outlineSize, float alphaThreshold)
            {
                // Won't draw if effect is disabled, outline size is zero or sampled fragment is above alpha threshold.
                if (isOutlineEnabled * outlineSize == 0) return 0;
                if (sampledColor.a > alphaThreshold) return 0;

                float2 texDdx = ddx(texCoord);
                float2 texDdy = ddy(texCoord);

                // Looking for an opaque pixel (sprite border from outise) around computed fragment with given depth (_OutlineSize).
                for (int i = 1; i <= SAMPLE_DEPTH_LIMIT; i++)
                {
                    float2 pixelUpTexCoord = texCoord + float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelUpAlpha = tex2Dgrad(_MainTex, pixelUpTexCoord, texDdx, texDdy).a;
                    if (pixelUpAlpha > alphaThreshold) return 1;

                    float2 pixelDownTexCoord = texCoord - float2(0, i * _MainTex_TexelSize.y);
                    fixed pixelDownAlpha = tex2Dgrad(_MainTex, pixelDownTexCoord, texDdx, texDdy).a;
                    if (pixelDownAlpha > alphaThreshold) return 1;

                    float2 pixelRightTexCoord = texCoord + float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelRightAlpha = tex2Dgrad(_MainTex, pixelRightTexCoord, texDdx, texDdy).a;
                    if (pixelRightAlpha > alphaThreshold) return 1;

                    float2 pixelLeftTexCoord = texCoord - float2(i * _MainTex_TexelSize.x, 0);
                    fixed pixelLeftAlpha = tex2Dgrad(_MainTex, pixelLeftTexCoord, texDdx, texDdy).a;
                    if (pixelLeftAlpha > alphaThreshold) return 1;

                    if (i > outlineSize) break;
                }

                return 0;
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);
                return color;
            }

			float _ShutOff;

            fixed4 ComputeFragment(VertexOutput vertexOutput) : SV_Target
            {
				fixed4 color = SampleSpriteTexture(vertexOutput.TexCoord) * vertexOutput.Color;
				color.rgb *= color.a;

				#ifdef OUTLINE_OUTSIDE
				int shouldDrawOutline = ShouldDrawOutlineOutside(color, vertexOutput.TexCoord, _IsOutlineEnabled, _OutlineSize, _AlphaThreshold);
				#else
				int shouldDrawOutline = ShouldDrawOutlineInside(color, vertexOutput.TexCoord, _IsOutlineEnabled, _OutlineSize, _AlphaThreshold);
				#endif

				#ifdef SHUT_OFF
					float delta = float(shouldDrawOutline) / float(SAMPLE_DEPTH_LIMIT);
					delta = delta == 0 ? 1 : delta;
					color.rgb = lerp(_OutlineColor.rgb, _OutlineColor.rgb * 0.7f, 1 - delta) * color.a;
					color.rgb = LinearToGammaSpace(color.rgb);
					color.rgb *= pow(2.0, -2);
					color.rgb = GammaToLinearSpace(color.rgb);
					color.rgb *= 0.7;
				#else
					color.rgb = lerp(color.rgb, _OutlineColor.rgb * _OutlineColor.a, shouldDrawOutline >= 1 ? 1 : 0);
				#endif
                return color;
            }

            ENDCG
        }
    }
}
