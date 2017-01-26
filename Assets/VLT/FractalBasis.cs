using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public abstract class FractalBasis : IDisposable {

		public abstract void Dispose();

		public abstract float get_value(Vec3f point);
	}
}