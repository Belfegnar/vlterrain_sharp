using System;
using CGLA;

namespace VLT
{
	public struct Plane
	{
		private Vec3f m_point;
		private Vec3f m_normal;

		public Plane(Vec3f point, Vec3f normal)
		{
			m_point  = point;
			m_normal = normal;
		}

		public float distance_to_point(Vec3f point)
		{
			Vec3f direction = point - m_point;

			return Vec3f.dot(direction, m_normal);//TODO dot?????????
		}

		public bool is_point_above(Vec3f point)
		{
			return distance_to_point(point) > 0.0f;
		}

		public Vec3f get_normal()
		{
			return m_normal;
		}

		public Vec3f get_point()
		{
			return m_point;
		}
	}
}

