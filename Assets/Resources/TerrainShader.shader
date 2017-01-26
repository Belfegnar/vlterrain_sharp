Shader "TerrainShader"
{
	Properties
	{
		diffuse_map ("diffuse_map", 2D) = "white" {}
		parent_diffuse_map ("parent_diffuse_map", 2D) = "white" {}
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
			};

			struct v2f
			{
				float4 position: POSITION;
			    float2 texcoords: TEXCOORD0;
			    float2 parent_texcoords: TEXCOORD1;
			    float4 morph_factor: TEXCOORD2;
				UNITY_FOG_COORDS(3)
			};

			sampler2D _MainTex;
			sampler2D _MainTex2;
			float4 _MainTex_ST;

			uniform float morph_factor; 
			uniform float2 height_offset_scale;
			uniform float2 parent_offset;
			uniform sampler2D diffuse_map;
			uniform sampler2D parent_diffuse_map;
			
			v2f vert (appdata input)
			{
				v2f output;
    
			    // FIXME ATI
			    float2 z = input.displacements.xy * height_offset_scale.y + height_offset_scale.x;
			    //float2 z = input.displacements.xy * (1.0f / 32767.0f) * height_offset_scale[1] + height_offset_scale[0];

			    float4 temp = float4(input.position.x, lerp(z.x, z.y, morph_factor), input.position.y, 1.0f);
			        
			    output.position = UnityObjectToClipPos(temp);

			    output.texcoords.xy = input.position.xy;
			    output.parent_texcoords.xy = input.position.xy * 0.5f + parent_offset;
			    output.morph_factor = morph_factor.xxxx;

			    UNITY_TRANSFER_FOG(o,o.position);
			    return output;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 color1 = tex2D(diffuse_map, i.texcoords);
			    fixed4 color2 = tex2D(parent_diffuse_map, i.parent_texcoords);

			    fixed4 col = lerp(color1, color2, i.morph_factor);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
