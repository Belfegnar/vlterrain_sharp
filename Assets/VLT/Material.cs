using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using CGLA;

namespace VLT
{
	public class Material : IDisposable {

		string m_name;
		int m_version;

		int m_size;
		int m_table_size;
		float[] m_table;

		float m_inv_scale;

		float m_estimated_error;
		float m_estimation_spacing;

		float get_table_value(int x, int y)
		{
			return m_table[y * m_table_size + x];
		}

		void set_table_value(int x, int y, float value)
		{
			m_table[y * m_table_size + x] = value;
		}

		float recursive_error(Vec3f start_point, Vec3f end_point)
		{
			Vec3f mid_point = (start_point + end_point) * 0.5f;
			float displaced_z = get_displacement(mid_point[0], mid_point[1]);
			float error = Math.Abs(mid_point[2] - displaced_z);

			if (error > 0.01f)
			{
				float e1 = recursive_error(start_point, new Vec3f(mid_point[0], mid_point[1], displaced_z));
				float e2 = recursive_error(new Vec3f(mid_point[0], mid_point[1], displaced_z), end_point);
				return Math.Max(e1, e2) + error;
			}
			else
			{
				return error;
			}
		}

		bool load_table()
		{
			using(FileStream file = new FileStream(m_name, FileMode.Open))
			{

				if (!file.CanRead || file.ReadByte() != m_version)
				{
					return false;
				}

				using(BinaryReader reader = new BinaryReader(file))
				{
					for (int y = 0; y < m_table_size; y++)
					{
						for (int x = 0; x < m_table_size; x++)
						{
							set_table_value(x, y, reader.ReadSingle());
						}
					}
				}
			}

			return true;
		}

		void save_table()
		{
			using (FileStream file = new FileStream (m_name, FileMode.OpenOrCreate))
			{
				file.WriteByte ((byte)m_version);

				using(BinaryWriter writer = new BinaryWriter(file))
				{
					for (int y = 0; y < m_table_size; y++) {
						for (int x = 0; x < m_table_size; x++) {
							writer.Write (get_table_value (x, y));
						}
					}
				}
			}
		}

		void calculate_table(int border_size)
		{
			UnityEngine.Vector4[][] weights = new UnityEngine.Vector4[][]{
				new UnityEngine.Vector4[]{
					new UnityEngine.Vector4(0.25f, 0.25f, 0.25f, 0.25f),
					new UnityEngine.Vector4(0.50f, 0.50f, 0.0f, 0.0f),
					new UnityEngine.Vector4(0.50f, 0.50f, 0.0f, 0.0f),
					new UnityEngine.Vector4(0.25f, 0.25f, 0.25f, 0.25f)
				},
				new UnityEngine.Vector4[]{
					new UnityEngine.Vector4(0.50f, 0.0f, 0.0f, 0.50f),
					new UnityEngine.Vector4(1.0f, 0.0f, 0.0f, 0.0f),
					new UnityEngine.Vector4(1.0f, 0.0f, 0.0f, 0.0f),
					new UnityEngine.Vector4(0.50f, 0.0f, 0.0f, 0.50f)
				},
				new UnityEngine.Vector4[]{
					new UnityEngine.Vector4(0.50f, 0.0f, 0.0f, 0.50f),
					new UnityEngine.Vector4(1.0f, 0.0f, 0.0f, 0.0f),           
					new UnityEngine.Vector4(1.0f, 0.0f, 0.0f, 0.0f),
					new UnityEngine.Vector4(0.50f, 0.0f, 0.0f, 0.50f)
				},
				new UnityEngine.Vector4[]{
					new UnityEngine.Vector4(0.25f, 0.25f, 0.25f, 0.25f),
					new UnityEngine.Vector4(0.50f, 0.50f, 0.0f, 0.0f),
					new UnityEngine.Vector4(0.50f, 0.50f, 0.0f, 0.0f),
					new UnityEngine.Vector4(0.25f, 0.25f, 0.25f, 0.25f)
				}
			};

			float displacement;

			for (int x = 0; x < m_size; x++)
			{
				for (int y = 0; y < m_size; y++)
				{
					int y_blend_index = 1;
					int x_blend_index = 1;
					float y_fraction = 0.0f;
					float x_fraction = 0.0f;
					float x_size = 0;
					float y_size = 0;

					int last = m_size - 1;

					if (y < border_size)
					{
						y_blend_index = 0;
						y_fraction = (float)(y + 1) / border_size;
						y_size = m_size / m_inv_scale;
					}
					else if (y >= last - border_size)
					{
						y_blend_index = 2;
						y_fraction = 1.0f - (float)(last - y) / border_size;
						y_size = -(m_size) / m_inv_scale;
					}

					if (x < border_size)
					{
						x_blend_index = 0;
						x_fraction = (float)(x + 1) / border_size;
						x_size = m_size / m_inv_scale;
					}
					else if (x >= last - border_size)
					{
						x_blend_index = 2;
						x_fraction = 1.0f - (float)(last - x) / border_size;
						x_size = -(m_size) / m_inv_scale;
					}

					float x_scaled = x / m_inv_scale;
					float y_scaled = y / m_inv_scale;

					if (x_blend_index != 1 || y_blend_index != 1)
					{
						float d0 = calculate_displacement(x_scaled, y_scaled);
						float d1 = calculate_displacement(x_scaled + x_size, y_scaled);
						float d2 = calculate_displacement(x_scaled + x_size, y_scaled + y_size);
						float d3 = calculate_displacement(x_scaled, y_scaled + y_size);

						UnityEngine.Vector4 weight = MathExt.bi_lerp(weights[x_blend_index][y_blend_index],
							weights[x_blend_index + 1][y_blend_index],
							weights[x_blend_index][y_blend_index + 1],
							weights[x_blend_index + 1][y_blend_index + 1],
							x_fraction, y_fraction);

						displacement = d0 * weight[0] + d1 * weight[1] + d2 * weight[2] + d3 * weight[3];
					}
					else
					{
						displacement = calculate_displacement(x_scaled, y_scaled);
					}

					set_table_value(x, y, displacement);
				}
			}

			for (int x = 0; x < m_table_size; x++)
			{
				set_table_value(x, m_size, get_table_value(x, 0));
			}

			for (int y = 0; y < m_table_size; y++)
			{
				set_table_value(m_size, y, get_table_value(0, y));
			}
		}

		protected virtual float calculate_displacement(float x, float y){
			return 0;
		}

		protected void init_table(int border_size)
		{
			UnityEngine.Debug.Log("Initialising material table...");

			if (!load_table())
			{
				UnityEngine.Debug.Log("calculating!");
				calculate_table(border_size);

				save_table();
			}
			else
			{
				UnityEngine.Debug.Log("cached!");
			}
		}


		public Material(string name, int version, int size, float scale)
		{
			m_name = name + ".mat";
			m_version = version;
			m_estimated_error = -1.0f;
			m_estimation_spacing = 0.0f;
			m_size = size;
			m_table_size = size + 1;
			m_inv_scale = 1.0f / scale;

			m_table = new float[m_table_size * m_table_size];
		}

		public virtual void Dispose(){}

		public float get_estimated_error(float spacing)
		{
			if (m_estimated_error < 0.0f || m_estimation_spacing != spacing)
			{
				UnityEngine.Debug.Log("Calculating estimated error for material...");

				m_estimation_spacing = spacing;

				for (int s = 0; s < 10000; s++)
				{
					Vec3f start_point = new Vec3f(UnityEngine.Random.Range(0.0f, 100.0f), UnityEngine.Random.Range(0.0f, 100.0f), 0.0f);
					Vec3f end_point = start_point + new Vec3f(spacing, spacing, 0.0f);

					start_point[2] = get_displacement(start_point[0], start_point[1]);
					end_point[2] = get_displacement(end_point[0], end_point[1]);

					m_estimated_error = Math.Max(m_estimated_error, recursive_error(start_point, end_point));
				}
			}

			return m_estimated_error;
		}

		public float get_displacement(double x, double y)
		{
			x = MathExt.modulo(x * m_inv_scale, (double)(m_size));
			y = MathExt.modulo(y * m_inv_scale, (double)(m_size));

			int x_0 = (int)(x);
			int x_1 = x_0 + 1;
			int y_0 = (int)(y);
			int y_1 = y_0 + 1;

			return MathExt.bi_lerp(get_table_value(x_0, y_0), get_table_value(x_1, y_0),
				get_table_value(x_0, y_1), get_table_value(x_1, y_1),
				(float)(x - x_0), (float)(y - y_0));
		}

		public void absolute()
		{
			for (int v = 0; v < m_size * m_size; v++)
			{
				m_table[v] = Math.Abs(m_table[v]);
			}
		}
		public void bias(float factor)
		{
			for (int v = 0; v < m_size * m_size; v++)
			{
				m_table[v] = MathExt.bias(m_table[v], factor);
			}
		}
		public void gain(float factor)
		{
			for (int v = 0; v < m_size * m_size; v++)
			{
				m_table[v] = MathExt.gain(m_table[v], factor);
			}
		}
		public void negate()
		{
			for (int v = 0; v < m_size * m_size; v++)
			{
				m_table[v] = -m_table[v];
			}
		}
		public void normalize()
		{
			float min_value = 1.0e12f;
			float max_value = -1.0e12f;
			for (int x = 0; x < m_size; x++)
			{
				for (int y = 0; y < m_size; y++)
				{
					if (get_table_value(x, y) < min_value)
					{
						min_value = get_table_value(x, y);
					}
					if (get_table_value(x, y) > max_value)
					{
						max_value = get_table_value(x, y);
					}
				}
			}
				
			float normalizer = (max_value - min_value) * 0.5f;
			for (int x2 = 0; x2 < m_size; x2++)
			{
				for (int y = 0; y < m_size; y++)
				{
					set_table_value(x2, y, (get_table_value(x2, y) - min_value) / normalizer - 1.0f);
				}
			}
		}
		public void power(float p)
		{
			for (int v = 0; v < m_size * m_size; v++)
			{
				m_table[v] = (float)Math.Pow(m_table[v], p);
			}
		}
		public void scale(float factor)
		{
			for (int v = 0; v < m_size * m_size; v++)
			{
				m_table[v] = m_table[v] * factor;
			}
		}
		public void translate(float distance)
		{
			for (int v = 0; v < m_size * m_size; v++)
			{
				m_table[v] = m_table[v] + distance;
			}
		}

		//void convolve(const std::vector<float> &kernel);
	}
}