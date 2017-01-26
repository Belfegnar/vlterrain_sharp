using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public enum TextureType {
		StaticTexture,
		DynamicTexture,
		CustomTexture
	}

	[Serializable]
	public abstract class Texture : IDisposable
	{
		
		[UnityEngine.SerializeField]TextureType m_type;

		[UnityEngine.SerializeField]int m_reference_count;
		TextureManager m_manager;

		[UnityEngine.SerializeField]protected bool m_initialized;

		protected abstract void initialize_image (Image image);
		protected abstract void replace_image (Image image);

		protected Texture(TextureManager manager, TextureType type)
		{
			m_manager = manager;
			m_type = type;
			m_reference_count = 0;
			m_initialized = false;
		}

		public abstract void Dispose ();

		public static Texture create_texture(TextureManager manager, TextureType type)
		{
			//string render = Configuration.get_string("render");

			return new UnityTexture(manager, type);
		}

		public void add_reference()
		{
			m_reference_count++;
		}

		public void remove_reference()
		{
			if (--m_reference_count == 0)
			{
				m_manager.release_texture(this);
			}
		}

		public void set_image(Image image)
		{
			if (!m_initialized)
			{
				initialize_image(image);

				m_initialized = true;
			}
			else
			{
				replace_image(image);
			}
		}

		public abstract void set_image(string asset_name);

		public TextureType get_type()
		{
			return m_type;
		}
	}
}

