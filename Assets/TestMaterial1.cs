using System.Collections;
using System.Collections.Generic;
using System;

namespace VLT
{
	public class TestMaterial1 : Material {

		Fractalizer m_fractalizer_0;
		Fractalizer m_fractalizer_1;

		protected override float calculate_displacement(float x, float y)
		{
			return m_fractalizer_1.get_displacement(x, y);
		}

		public TestMaterial1(float amplitude_scale, float frequency_scale) : base("TestMaterial1", 4, 2048, 0.05f)
		{
			m_fractalizer_1 = new Fractalizer(new PerlinBasis(0.7f), 2.0f, 1.2f, 12, amplitude_scale * 1.86f, frequency_scale * 0.03f);

			init_table(128);
		}
		public override void Dispose ()
		{
			m_fractalizer_1.Dispose ();
		}
	}
}