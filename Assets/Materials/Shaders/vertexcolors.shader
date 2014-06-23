// THIS IS EXTERNAL SHADER, ORIGINAL SOURCE:
// http://vertexcolors.com/shared/AEVertexGI.shader

Shader "AE/AE Vertex GI" {
	
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Intensity ("Vertex LM Intensity", Range(1, 16)) = 8
	}
	
	SubShader {
		
		Tags {
			"RenderType" = "Opaque"
		}
		
		Pass {
			CGPROGRAM
			#pragma fragment frag
			#pragma vertex vert
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Intensity;
			half4 _Color;
			
			#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
			
			struct appdata_color {
				float4 vertex   : POSITION;
				half4  color    : COLOR;
				float3 normal   : NORMAL;
				float4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos       : SV_POSITION;
				float2 uv        : TEXCOORD0;
				half4  color     : TEXCOORD2;
			};
			
			
			v2f vert (appdata_color v) {
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.color = v.color;
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    half4 texcol = tex2D (_MainTex, i.uv);
			    half4 lm = i.color.rgba * i.color.a * _Intensity;
			    lm.a = 1;
			    return texcol * lm * _Color;
			}
			
			ENDCG
		}
		
	}
	
	FallBack "Diffuse"
	
}
