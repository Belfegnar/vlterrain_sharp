using System.Collections;
using System.Collections.Generic;
using System;

namespace VLT
{
	[System.Serializable]
	public abstract class QuadTreeReader : IDisposable {

		public QuadTreeReader()
		{

		}

		public abstract void Dispose ();

		public abstract QuadInfo create_info (int id);

		public static QuadTreeReader create_reader(string filename)
		{
			//string reader = Configuration.get_string("reader");
			return new StreamQuadTreeReader(filename);
		}
	}
}