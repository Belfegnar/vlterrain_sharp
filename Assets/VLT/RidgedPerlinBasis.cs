using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class RidgedPerlinBasis : PerlinBasis {

		float m_sharpness;

		public RidgedPerlinBasis(float offset, float sharpness) : base(offset)
		{
			m_sharpness = sharpness;
		}

		public override void Dispose()
		{

		}

		public override float get_value(Vec3f point)
		{
			return (float)Math.Pow((1.0f - Math.Abs(base.get_value(point))), m_sharpness) * 2.0f - 1.0f;
		}
	}
}