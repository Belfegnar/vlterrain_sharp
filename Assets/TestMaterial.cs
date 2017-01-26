using System.Collections;
using System.Collections.Generic;
using System;

namespace VLT
{
	public class TestMaterial : Material {

		Fractalizer m_fractalizer_0;
		Fractalizer m_fractalizer_1;

		protected override float calculate_displacement(float x, float y)
		{
			return Math.Min(m_fractalizer_0.get_displacement(x, y), m_fractalizer_1.get_displacement(x, y));
		}

		public TestMaterial(float amplitude_scale, float frequency_scale) : base("TestMaterial", 1, 2048, 0.05f)
		{
			m_fractalizer_0 = new Fractalizer(new RidgedPerlinBasis(0.2f, 4.0f), 2.0f, 1.7f, 12, amplitude_scale, frequency_scale * 0.05f);
			m_fractalizer_1 = new Fractalizer(new PerlinBasis(0.7f), 2.0f, 1.2f, 12, amplitude_scale * 0.66f, frequency_scale * 0.06f);

			init_table(128);
		}
		public override void Dispose ()
		{
			m_fractalizer_0.Dispose ();
			m_fractalizer_1.Dispose ();
		}
	}
}