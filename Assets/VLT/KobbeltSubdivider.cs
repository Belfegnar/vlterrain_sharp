using System.Collections;
using System.Collections.Generic;
using System;

namespace VLT
{
	public class KobbeltSubdivider : IDisposable {

		float m_omega;
		float m_alpha;
		float m_beta;
		float m_rho;
		float m_my;
		float m_ny;

		float subdivide_x(int first_x, int y, HeightMap heightmap)
		{
			// FIXME!!!
			//assert(first_x >= -1 && first_x <= heightmap.get_size() - 3);

			return m_alpha * heightmap.get_extrapolated_height(first_x, y) + 
				m_beta * heightmap.get_extrapolated_height(first_x + 1, y) +
				m_beta * heightmap.get_extrapolated_height(first_x + 2, y) +
				m_alpha * heightmap.get_extrapolated_height(first_x + 3, y);
		}
		float subdivide_y(int x, int first_y, HeightMap heightmap)
		{
			// FIXME!!!
			//assert(first_y >= -1 && first_y <= heightmap.get_size() - 3);

			return m_alpha * heightmap.get_extrapolated_height(x, first_y) +
				m_beta * heightmap.get_extrapolated_height(x, first_y + 1) +
				m_beta * heightmap.get_extrapolated_height(x, first_y + 2) +
				m_alpha * heightmap.get_extrapolated_height(x, first_y + 3);
		}
		float subdivide_xy(int first_x, int first_y, HeightMap heightmap)
		{
			// FIXME!!!
			//assert(first_x >= -1 && first_x <= heightmap.get_size() - 3);
			//assert(first_y >= -1 && first_y <= heightmap.get_size() - 3);

			return m_rho * heightmap.get_extrapolated_height(first_x, first_y) +
				m_my * heightmap.get_extrapolated_height(first_x, first_y + 1) +
				m_my * heightmap.get_extrapolated_height(first_x, first_y + 2) +
				m_rho * heightmap.get_extrapolated_height(first_x, first_y + 3) +

				m_my * heightmap.get_extrapolated_height(first_x + 1, first_y) +
				m_ny * heightmap.get_extrapolated_height(first_x + 1, first_y + 1) +
				m_ny * heightmap.get_extrapolated_height(first_x + 1, first_y + 2) +
				m_my * heightmap.get_extrapolated_height(first_x + 1, first_y + 3) +

				m_my * heightmap.get_extrapolated_height(first_x + 2, first_y) +
				m_ny * heightmap.get_extrapolated_height(first_x + 2, first_y + 1) +
				m_ny * heightmap.get_extrapolated_height(first_x + 2, first_y + 2) +
				m_my * heightmap.get_extrapolated_height(first_x + 2, first_y + 3) +

				m_rho * heightmap.get_extrapolated_height(first_x + 3, first_y) +
				m_my * heightmap.get_extrapolated_height(first_x + 3, first_y + 1) +
				m_my * heightmap.get_extrapolated_height(first_x + 3, first_y + 2) +
				m_rho * heightmap.get_extrapolated_height(first_x + 3, first_y + 3);
		}
			
		public KobbeltSubdivider(float omega)
		{
			m_omega = omega;
			m_alpha = -m_omega / 16.0f;
			m_beta = (8.0f + m_omega) / 16.0f;
			m_rho = m_alpha * m_alpha;
			m_my = m_alpha * m_beta;
			m_ny = m_beta * m_beta;
		}

		public float subdivide(int x, int y, HeightMap heightmap)
		{
			if ((x & 1) == 0 && (y & 1) == 0)
			{
				return heightmap.get_height(x / 2, y / 2);
			}
			else if ((x & 1) == 0)
			{
				return subdivide_y(x / 2, (y + 2) / 2 - 2, heightmap);
			}
			else if ((y & 1) == 0)
			{
				return subdivide_x((x + 2) / 2 - 2, y / 2, heightmap);
			}
			else
			{
				return subdivide_xy((x + 2) / 2 - 2, (y + 2) / 2 - 2, heightmap);
			}
		}

		public void Dispose(){}
	}
}