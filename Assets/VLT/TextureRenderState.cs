using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	[Serializable]
	public class TextureRenderState : RenderState {

		UnityEngine.MaterialPropertyBlock m_property_block;
		UnityEngine.Material m_material;

		int m_screen_width;
		int m_screen_height;

		int m_texture_size;

		int m_tex_origin_parameter;
		int m_diffuse_map_parameter;
		List<int> m_material_maps_parameter = new List<int>();

		public TextureRenderState()
		{
			m_material = new UnityEngine.Material(UnityEngine.Shader.Find("TextureShader"));
			m_material.SetTexture ("material_map_0", UnityEngine.Resources.Load<UnityEngine.Texture> ("texture0"));
			m_material.SetTexture ("material_map_1", UnityEngine.Resources.Load<UnityEngine.Texture> ("texture1"));
			m_property_block = new UnityEngine.MaterialPropertyBlock ();

			m_screen_width = Configuration.get_int("screen_width");
			m_screen_height = Configuration.get_int("screen_height");

			m_texture_size = Configuration.get_int("dynamic_texture_size");

			m_tex_origin_parameter = UnityEngine.Shader.PropertyToID("tex_origin_and_size");

			m_diffuse_map_parameter = UnityEngine.Shader.PropertyToID("diffuse_map");

			m_material_maps_parameter.Add(UnityEngine.Shader.PropertyToID("material_map_0"));

			m_material_maps_parameter.Add(UnityEngine.Shader.PropertyToID("material_map_1"));
		}
		public override void Dispose()
		{
			if (m_property_block != null)
				m_property_block.Clear ();
			m_property_block = null;
			UnityEngine.GameObject.DestroyImmediate (m_material);
		}

		public virtual void set_texture_origin_and_size(Vec2f origin, float size)
		{
			m_material.SetVector(m_tex_origin_parameter, new UnityEngine.Vector4(origin[0], origin[1], size, size));
		}
		public override void set_diffuse_map(Texture texture)
		{
			m_material.SetTexture(m_diffuse_map_parameter, (texture as UnityTexture).get_unity_texture());
		}
		public virtual void set_material_maps(Texture[] textures)//Material material)
		{
			// FIXME!!
//			extern Texture *tex_0, *tex_1;
//			Texture[] textures = new Texture[2];// {tex_0, tex_1};

			for (int p = 0; p < m_material_maps_parameter.Count; p++)
			{
				// FIXME!!
				m_material.SetTexture(m_material_maps_parameter[p], (textures[p] as UnityTexture).get_unity_texture());
			}
		}
		public override UnityEngine.Material get_unity_material ()
		{
			return m_material;
		}

		public override UnityEngine.MaterialPropertyBlock get_unity_material_parameters ()
		{
			return m_property_block;
		}

	}
}