using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class Fractalizer : IDisposable {

		FractalBasis m_basis;
		float m_lacunarity;
		float m_h;
		int m_octaves;
		float m_amplitude_scale;
		float m_frequency_scale;
		float[] m_exponents = new float[16];

		float fbm(Vec3f point)
		{
			float value = 0.0f;

			for(int o = 0; o < m_octaves; o++)
			{
				value += m_basis.get_value(point) * m_exponents[o];
				point *= m_lacunarity;
			}

			return value;
		}

		public Fractalizer(FractalBasis basis, float lacunarity, float h, int octaves,	float amplitude_scale, float frequency_scale)
		{
			m_basis = basis;
			m_lacunarity = lacunarity;
			m_h = h;
			m_octaves = octaves;
			m_amplitude_scale = amplitude_scale;
			m_frequency_scale = frequency_scale;

			for (int o = 0; o < 16; o++)
			{
				m_exponents[o] = (float)Math.Pow(m_lacunarity, -o * m_h);
			}
		}

		public void Dispose()
		{
			m_basis.Dispose();
		}

		public float get_displacement(float x, float y)
		{
			return m_amplitude_scale * fbm(new Vec3f(x, y, 0.0f) * m_frequency_scale);
		}
	}
}