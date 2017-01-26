using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class TerrainRenderState : RenderState {
		
		bool m_wireframe;
		UnityEngine.MaterialPropertyBlock m_property_block;
		UnityEngine.Material m_material;

		float m_frame_time;

		int m_morph_factor_parameter;
		int m_height_offset_scale_parameter;
		int m_parent_offset_parameter;


		int m_diffuse_map_parameter;
		int m_parent_diffuse_map_parameter;

		protected bool get_wireframe()
		{
			return m_wireframe;
		}

		public TerrainRenderState()
		{
			m_wireframe = false;
			m_material = new UnityEngine.Material(UnityEngine.Shader.Find("TerrainShader"));
			m_property_block = new UnityEngine.MaterialPropertyBlock ();

			m_morph_factor_parameter = UnityEngine.Shader.PropertyToID("morph_factor");
			m_height_offset_scale_parameter = UnityEngine.Shader.PropertyToID("height_offset_scale");
			m_parent_offset_parameter = UnityEngine.Shader.PropertyToID("parent_offset");
			m_diffuse_map_parameter = UnityEngine.Shader.PropertyToID("diffuse_map");
			m_parent_diffuse_map_parameter = UnityEngine.Shader.PropertyToID("parent_diffuse_map");
		}

		public override void Dispose()
		{
			if (m_property_block != null)
				m_property_block.Clear ();
			m_property_block = null;
			UnityEngine.GameObject.DestroyImmediate (m_material);
		}

		public override UnityEngine.Material get_unity_material ()
		{
			return m_material;
		}

		public override UnityEngine.MaterialPropertyBlock get_unity_material_parameters ()
		{
			return m_property_block;
		}

		public virtual void set_morph_factor(float morph_factor)
		{
			m_property_block.SetFloat (m_morph_factor_parameter, morph_factor);
		}
		public virtual void set_height_offset_scale(float offset, float scale)
		{
			m_property_block.SetVector(m_height_offset_scale_parameter, new UnityEngine.Vector4(offset, scale, 0, 0));
		}
		public override void set_diffuse_map(Texture texture)
		{
			m_property_block.SetTexture (m_diffuse_map_parameter, (texture as UnityTexture).get_unity_texture());
		}
		public virtual void set_parent_diffuse_map(Texture parent_diffuse_map)
		{
			m_property_block.SetTexture (m_parent_diffuse_map_parameter, (parent_diffuse_map as UnityTexture).get_unity_texture ());
		}
		public virtual void set_parent_offset(Vec2f offset)
		{
			m_property_block.SetVector(m_parent_offset_parameter, offset.ToVector2());
		} 

		public int get_pass_count()
		{
			return get_wireframe() ? 2 : 1;
		}

		public void set_wireframe(bool enable)
		{
			m_wireframe = enable;
		}

	}
}