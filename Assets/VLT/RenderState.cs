using System.Collections;
using System.Collections.Generic;
using System;

namespace VLT
{
	public abstract class RenderState : IDisposable {
		public enum State
		{
			TerrainState,
			TextureState,
			SkyboxState,
			UIState,    
		};
		public abstract void Dispose();
		public abstract UnityEngine.Material get_unity_material ();
		public abstract UnityEngine.MaterialPropertyBlock get_unity_material_parameters ();

		public abstract void set_diffuse_map(Texture texture);

//		public abstract int get_pass_count();
//		public abstract void set_pass(int number);
//
//		public abstract void enable();
//		public abstract void disable();
	}
}