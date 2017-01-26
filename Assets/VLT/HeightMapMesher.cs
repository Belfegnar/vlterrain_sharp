using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class HeightMapMesher : IDisposable {

		MemoryManager m_memory_manager;

		int m_vertex_cache_size;
		int m_heightmap_size;

		List<Vec3i> m_index_map = new List<Vec3i>();

		// FIXME ATI
		UnityEngine.Vector2[] m_temp_displacements;
		UnityEngine.Color[] m_temp_lights;

		MemoryBlock m_vertex_block;
		MemoryBlock m_index_block;

		int m_index_count;

		int calc_index_count()
		{
			// FIXME!!
			int estimated_index_count = (m_heightmap_size + 2) * ((m_heightmap_size + 2) + 3 * 
				(m_heightmap_size + 2) / (m_vertex_cache_size / 2)) * 2 +
				m_heightmap_size * 4;

			return estimated_index_count;
		}
		int calc_vertex_count()
		{
			int extra = (m_heightmap_size + 1) / (m_vertex_cache_size / 2 - 1);
			if (MathExt.modulo(m_heightmap_size + 1, extra * (m_vertex_cache_size / 2 - 1)) == 0)
			{
				extra--;
			}

			return (m_heightmap_size + 2) * ((m_heightmap_size + 2) + extra);
		}

		void init_index_block()
		{
			List<int> indices = new List<int>();

			int vertex = 0;
			for (int x_start = 0; x_start < m_heightmap_size + 1; x_start += m_vertex_cache_size / 2 - 1)
			{
				int x_end = Math.Min(x_start + m_vertex_cache_size / 2, m_heightmap_size + 2);
				int x_delta = x_end - x_start;
				int vertex_start = vertex;

				for (int x = x_start; x < x_end; x++)
				{
					indices.Add(vertex++);
					indices.Add(vertex++);
				}

				indices.Add(vertex - 1);
				indices.Add(vertex_start + 1);

				for (int x = x_start; x < x_end; x++)
				{
					indices.Add(vertex_start + (x - x_start) * 2 + 1);
					indices.Add(vertex++);
				}

				for (int y = 2; y < m_heightmap_size; y++)
				{
					indices.Add(vertex - 1);
					indices.Add(vertex_start + y * x_delta);
					for (int x = x_start; x < x_end; x++)
					{
						indices.Add(vertex - x_delta);
						indices.Add(vertex++);
					}
				}

				indices.Add(vertex - 1);
				indices.Add(vertex_start + m_heightmap_size * x_delta);

				for (int x = x_start; x < x_end; x++)
				{
					indices.Add(vertex - x_delta);
					indices.Add(vertex++);
				}

				indices.Add(vertex - 1);
				indices.Add(vertex);
			}

			m_index_count = indices.Count - 2;

			m_index_block = m_memory_manager.get_index_block();
			m_index_block.set_data(indices.ToArray(), m_index_count * sizeof(short));
		}

		void init_index_map()
		{

			for (int x_start = 0; x_start < m_heightmap_size + 1; x_start += m_vertex_cache_size / 2 - 1)
			{
				int x_end = Math.Min(x_start + m_vertex_cache_size / 2, m_heightmap_size + 2);

				for (int x = x_start; x < x_end; x++)
				{
					m_index_map.Add(new Vec3i(Math.Min(Math.Max(x - 1, 0), m_heightmap_size - 1), m_heightmap_size - 1, 1));
					m_index_map.Add(new Vec3i(Math.Min(Math.Max(x - 1, 0), m_heightmap_size - 1), m_heightmap_size - 1, (x == 0 || x == m_heightmap_size + 1)? 1 : 0));
				}

				for (int y = m_heightmap_size - 2; y >= 0; y--)
				{
					for (int x = x_start; x < x_end; x++)
					{
						m_index_map.Add(new Vec3i(Math.Min(Math.Max(x - 1, 0), m_heightmap_size - 1), y, (x == 0 || x == m_heightmap_size + 1) ? 1 : 0));
					}
				}

				for (int x = x_start; x < x_end; x++)
				{
					m_index_map.Add(new Vec3i(Math.Min(Math.Max(x - 1, 0), m_heightmap_size - 1), 0, 1));
				}
			}
		}

		void init_vertex_block()
		{

			List<float> vertices = new List<float>();

			int vertex_count = calc_vertex_count();
			for (int v = 0; v < vertex_count; v++)
			{
				vertices.Add((float)(m_index_map[v][0]) / (m_heightmap_size - 1));
				vertices.Add((float)(m_index_map[v][1]) / (m_heightmap_size - 1));
				vertices.Add(0.0f);
			}

			m_vertex_block = m_memory_manager.get_vertex_block();
			m_vertex_block.set_data(vertices.ToArray(), vertices.Count * sizeof(float));
		}

		void fill_displacement_block(DualHeightMap heightmap, MemoryBlock displacement_block, float skirt_depth)
		{

			float min_height = heightmap.get_min_height() - skirt_depth;
			float height_range = heightmap.get_height_range() + skirt_depth;
			UnityEngine.Vector2 tmpVector;

			int vertex_count = calc_vertex_count();
			for (int d = 0; d < vertex_count; d++)
			{
				Vec3i mapping = m_index_map[d];
				float height = heightmap.get_height(mapping[0], mapping[1]) - skirt_depth * mapping[2];
				float normalized_height = (height - min_height) / height_range;
				float morphed_height = height + heightmap.get_morph_delta(mapping[0], mapping[1]);
				float normalized_morphed_height = (morphed_height - min_height) / height_range;

				// FIXME ATI
				tmpVector.x = normalized_height;
				tmpVector.y = normalized_morphed_height;
				m_temp_displacements[d] = tmpVector;

				//m_temp_displacements[d * 2 + 0] = (short)(normalized_height * 0x7FFF);
				//m_temp_displacements[d * 2 + 1] = (short)(normalized_morphed_height * 0x7FFF);
			}

			displacement_block.set_data(m_temp_displacements, m_temp_displacements.Length);
		}


		public HeightMapMesher()
		{
			m_heightmap_size = 0;
			m_vertex_block = null;//TODO Dispose
			m_index_block = null;
			m_index_count = 0;

			m_vertex_cache_size = Math.Max(Configuration.get_int("vertex_cache_size"), 6);

			m_memory_manager = MemoryManager.create_manager();
		}

		public void Dispose()
		{
			m_memory_manager.free_blocks();
			m_memory_manager.Dispose();
		}

		public void initialize(int heightmap_size)
		{
			m_heightmap_size = heightmap_size;

			int vertex_count = calc_vertex_count(); 
			int index_size = calc_index_count() * sizeof(int);
			int vertex_size = vertex_count * sizeof(float) * 3;
			int light_size = vertex_count * sizeof(float) * 4;  // FIXME ATI
			int displacement_size = vertex_count * sizeof(float) * 2; // FIXME ATI
			UnityEngine.Debug.Log("m_heightmap_size = " + m_heightmap_size);
			m_memory_manager.initialize_blocks(index_size, vertex_size, light_size, displacement_size, Configuration.get_int("quad_cache_size"));

			m_temp_displacements = new UnityEngine.Vector2[vertex_count];
			m_temp_lights = new UnityEngine.Color[vertex_count];

			init_index_block();
			init_index_map();
			init_vertex_block();
		}

		public VertexArray create_vertex_array(DualHeightMap heightmap, float skirt_depth)
		{
			if (m_index_count == 0)
			{
				initialize(heightmap.get_size());
			}

			// FIXME ATI
			MemoryBlock displacement_block = m_memory_manager.get_displacement_block();

			fill_displacement_block(heightmap, displacement_block, skirt_depth);

			return new VertexArray(this, m_index_block, m_index_count, m_vertex_block, displacement_block);
		}

		public MemoryBlock create_light_array(byte[] light_array)
		{
			UnityEngine.Color tmpColor;
			tmpColor.a = 0;

			int vertex_count = calc_vertex_count();
			for (int l = 0; l < vertex_count; l++)
			{
				tmpColor.r = light_array[(m_index_map[l][1] * m_heightmap_size + m_index_map[l][0]) * 4 + 0] * 1.0f / 255.0f;
				tmpColor.g = light_array[(m_index_map[l][1] * m_heightmap_size + m_index_map[l][0]) * 4 + 1] * 1.0f / 255.0f;
				tmpColor.b = light_array[(m_index_map[l][1] * m_heightmap_size + m_index_map[l][0]) * 4 + 2] * 1.0f / 255.0f;
				m_temp_lights [l] = tmpColor;

//				m_temp_lights[l * 4 + 0] = light_array[(m_index_map[l][1] * m_heightmap_size + m_index_map[l][0]) * 4 + 0] * 1.0f / 255.0f;
//				m_temp_lights[l * 4 + 1] = light_array[(m_index_map[l][1] * m_heightmap_size + m_index_map[l][0]) * 4 + 1] * 1.0f / 255.0f;
//				m_temp_lights[l * 4 + 2] = light_array[(m_index_map[l][1] * m_heightmap_size + m_index_map[l][0]) * 4 + 2] * 1.0f / 255.0f;
//				m_temp_lights[l * 4 + 3] = 0;
			}

			MemoryBlock light_block = m_memory_manager.get_light_block();
			light_block.set_data(m_temp_lights, m_temp_lights.Length/* * sizeof(m_temp_lights[0])*/);

			return light_block;
		}

		public void destroy_vertex_array(VertexArray vertex_array)
		{
			m_memory_manager.release_displacement_block(vertex_array.get_displacement_block());
		}
	}
}