Shader "TextureShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MainTex2 ("Texture2", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 position: POSITION;
			    float2 displacements: TEXCOORD0;
			    float4 light: COLOR0;
			};

			struct v2f
			{
				float4 position: POSITION;
			    float2 diffuse_texcoord: TEXCOORD0;
			    float2 material_texcoord_0: TEXCOORD1;
			    float2 material_texcoord_1: TEXCOORD2;  
			    float4 material_weights: COLOR0;
			    float4 light: COLOR1;
			};

			sampler2D _MainTex;
			sampler2D _MainTex2;
			float4 _MainTex_ST;

			uniform float4 tex_origin_and_size;
			uniform sampler2D diffuse_map;
			uniform sampler2D material_map_0;
			uniform sampler2D material_map_1;
			uniform sampler2D material_map_2;
			
			v2f vert (appdata input)
			{
				v2f output;

				input.position.w = 1.0f;

			    output.position = UnityObjectToClipPos(input.position.xzyw);
			    output.diffuse_texcoord.xy = (input.position.xy - tex_origin_and_size.xy) * (1.0f / tex_origin_and_size.zw);

				output.material_texcoord_0.xy = (input.position.xy - tex_origin_and_size.xy) * (500.0f / tex_origin_and_size.zw);
				output.material_texcoord_1.xy = output.material_texcoord_0.xy;

				output.material_weights = float4(0.55f, 0.0f, 0.45f, 0.0f);

			    output.light = input.light;
				return output;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//return i.light;
				float4 diffuse_color = tex2D(diffuse_map, i.diffuse_texcoord);
    			float4 material_0_color = tex2D(material_map_0, i.material_texcoord_0);
    			float4 material_1_color = tex2D(material_map_1, i.material_texcoord_1);

			    return saturate(diffuse_color * i.material_weights.z  + 
								material_0_color * i.material_weights.x +
								material_1_color * i.material_weights.y) * i.light;
			}
			ENDCG
		}
	}
}
