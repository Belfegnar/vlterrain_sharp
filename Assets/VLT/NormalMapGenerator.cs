using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class NormalMapGenerator : IDisposable {

		byte[] m_height_to_index_table;

		int m_index_table_size;

		int m_lookup_size;
		int m_lookup_radius;

		int m_table_to_lookup_factor;

		float[] m_lookup_table;

		float m_scale;
		float m_inv_scale;

		byte get_index(float delta_height)
		{
			delta_height *= m_inv_scale;

			int index_radius = m_lookup_radius * m_table_to_lookup_factor;
			int quantized_height = (int)(MathExt.clamp(delta_height * m_table_to_lookup_factor, 
				-(float)(index_radius), (float)(index_radius)));

			return m_height_to_index_table[index_radius + quantized_height];
		}

		ushort calculate_normal(float left, float right, 
			float up, float down, float center)
		{
			byte x_index = (byte)((get_index(center - left) + get_index(right - center)) / 2);
			byte y_index = (byte)((get_index(center - down) + get_index(up - center)) / 2);

			return (ushort)((y_index << 8) | x_index);
		}

		ushort calculate_normal2(float left_down, float right_down, 
			float right_up, float left_up)
		{
			byte x_index = (byte)((get_index(right_down - left_down) + get_index(right_up - left_up)) / 2);
			byte y_index = (byte)((get_index(left_up - left_down) + get_index(right_up - right_down)) / 2);

			return (ushort)((y_index << 16) | x_index);
		}


		public NormalMapGenerator(int lookup_size, float scale)
		{
			m_index_table_size=2048;
			m_lookup_size = lookup_size;
			m_lookup_radius = (lookup_size - 1) / 2;
			m_scale = scale;
			m_inv_scale = 1.0f / scale;

			m_height_to_index_table = new byte[m_index_table_size];
			m_table_to_lookup_factor = m_index_table_size / m_lookup_size;

			float atan_min = (float)Math.Atan(-m_lookup_radius * m_scale) + MathExt.M_PI_2;
			float atan_max = (float)Math.Atan(m_lookup_radius * m_scale) + MathExt.M_PI_2;
			for (int i = 0; i < m_index_table_size; i++)
			{
				float delta_height = ((i - m_index_table_size / 2) / m_table_to_lookup_factor) * m_scale;
				float angle = (float)Math.Atan(delta_height) + MathExt.M_PI_2;

				m_height_to_index_table[i] = (byte)(((m_lookup_size - 1) * (angle - atan_min)) / (atan_max - atan_min));
			}

			init_lookup_map();
		}
		public void Dispose()
		{
			m_height_to_index_table = null;
		}

		public void calculate_light(DualHeightMap heightmap, byte[] light_array)
		{
			float inv_spacing = 1.0f / heightmap.get_spacing();

			for (int y = 0; y < heightmap.get_size(); y++)
			{
				for (int x = 0; x < heightmap.get_size(); x++)
				{
					uint normal_index = calculate_normal(heightmap.get_extrapolated_height(x - 1, y) * inv_spacing, 
						heightmap.get_extrapolated_height(x + 1, y) * inv_spacing,
						heightmap.get_extrapolated_height(x, y + 1) * inv_spacing, 
						heightmap.get_extrapolated_height(x, y - 1) * inv_spacing,
						heightmap.get_extrapolated_height(x, y) * inv_spacing);
						/*
						float h_ld = heightmap.get_extrapolated_height(x - 1, y - 1) * inv_spacing;
						float h_cd = heightmap.get_extrapolated_height(x, y - 1) * inv_spacing;
						float h_cc = heightmap.get_extrapolated_height(x, y) * inv_spacing;
						float h_lc = heightmap.get_extrapolated_height(x - 1, y) * inv_spacing;
						float h_rd = heightmap.get_extrapolated_height(x + 1, y - 1) * inv_spacing;
						float h_rc = heightmap.get_extrapolated_height(x + 1, y) * inv_spacing;
						float h_ru = heightmap.get_extrapolated_height(x + 1, y + 1) * inv_spacing;
						float h_cu = heightmap.get_extrapolated_height(x, y + 1) * inv_spacing;
						float h_lu = heightmap.get_extrapolated_height(x - 1, y + 1) * inv_spacing;

						unsigned int normal_index2 = 0;
						normal_index2 += calculate_normal2(h_ld, h_cd, h_cu, h_lu);
						normal_index2 += calculate_normal2(h_cd, h_rd, h_rc, h_cc);
						normal_index2 += calculate_normal2(h_cc, h_rc, h_ru, h_cu);
						normal_index2 += calculate_normal2(h_lc, h_cc, h_cu, h_lu);

						unsigned int normal_index = ((normal_index2 >> 16) / 4) << 8;
						normal_index |= (normal_index2 & 0xffff) / 4;
  						*/          
					byte color = (byte)(255 * m_lookup_table[(normal_index >> 8) * m_lookup_size + (normal_index & 0xFF)]);

					light_array[(y * heightmap.get_size() + x) * 4 + 0] = color;
					light_array[(y * heightmap.get_size() + x) * 4 + 1] = color;
					light_array[(y * heightmap.get_size() + x) * 4 + 2] = color;
				}
			}
		}
		public void init_lookup_map()
		{
			m_lookup_table = new float[m_lookup_size * m_lookup_size];

			float atan_min = (float)Math.Atan(-m_lookup_radius * m_scale) + MathExt.M_PI_2;
			float atan_max = (float)Math.Atan(m_lookup_radius * m_scale) + MathExt.M_PI_2;

			for (int y = 0; y < m_lookup_size; ++y)
			{
				for (int x = 0; x < m_lookup_size; ++x)
				{
					float x_normalized = x / (float)(m_lookup_size - 1);
					float x_angle = atan_min + x_normalized * (atan_max - atan_min);

					float y_normalized = y / (float)(m_lookup_size - 1);
					float y_angle = atan_min + y_normalized * (atan_max - atan_min);

					Vec3f x_normal = new Vec3f((float)Math.Cos(x_angle), 0.0f, (float)Math.Sin(x_angle));
					Vec3f y_normal = new Vec3f(0.0f, (float)Math.Cos(y_angle), (float)Math.Sin(y_angle));

					Vec3f normal = Vec3f.normalize(x_normal + y_normal);

					m_lookup_table[y * m_lookup_size + x] = MathExt.clamp(Vec3f.dot(normal, Vec3f.normalize(new Vec3f(0.8f, 0.8f, 0.8f))), 0.0f, 1.0f);
				}
			}
		}
	}
}