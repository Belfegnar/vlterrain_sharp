using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class PerlinBasis : FractalBasis {

		float m_offset;

		public PerlinBasis(float offset)
		{
			m_offset = offset;
		}

		public override void Dispose(){}

		public override float get_value(Vec3f point)
		{
			return PerlinNoise.noise (point + new Vec3f (m_offset));
		}
	}
}