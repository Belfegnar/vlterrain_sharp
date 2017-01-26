using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	[Serializable]
	public class TextureManager : IDisposable
	{
		[UnityEngine.SerializeField]List<Texture> m_all_static_textures;
		[UnityEngine.SerializeField]Stack<Texture> m_free_static_textures = new Stack<Texture>();

		[UnityEngine.SerializeField]List<Texture> m_all_dynamic_textures;
		[UnityEngine.SerializeField]Stack<Texture> m_free_dynamic_textures = new Stack<Texture>();

		public TextureManager(int count, int dynamic_size, int static_size, bool static_compressed)
		{
			float static_bpp = 4.0f / (static_compressed ? 8 : 2);
			int dynamic_bpp = 2;
			int memory_size = (int)(dynamic_size * dynamic_size * count * dynamic_bpp +
				static_size * static_size * count * static_bpp);

			UnityEngine.Debug.Log("Allocating " + count + " dynamic textures of " + 
				dynamic_size + "x" +  dynamic_size);
			UnityEngine.Debug.Log("Allocation " + count + " static textures of " +
				static_size + "x" +  static_size);
			UnityEngine.Debug.Log("Allocation total is " + (memory_size / (1024 * 1024)) + " mb");

			Image static_image = new Image(static_size, static_compressed);
			m_all_static_textures = new List<Texture>(count);
			for (int t = 0; t < count; t++)
			{
				Texture texture = Texture.create_texture(this, TextureType.StaticTexture);
				texture.set_image(static_image);

				m_all_static_textures.Add(texture);
				m_free_static_textures.Push(texture);
			}

			Image dynamic_image = new Image(dynamic_size, false);
			m_all_dynamic_textures = new List<Texture>(count);
			for (int t = 0; t < count; t++)
			{
				Texture texture = Texture.create_texture(this, TextureType.DynamicTexture);
				texture.set_image(dynamic_image);

				m_all_dynamic_textures.Add(texture);
				m_free_dynamic_textures.Push(texture);
			}
		}

		public void Dispose()
		{
			for (int t = 0; t < m_all_static_textures.Count; t++)
			{
				m_all_static_textures[t].Dispose();
			}
			m_all_static_textures.Clear ();
			for (int t = 0; t < m_all_dynamic_textures.Count; t++)
			{
				m_all_dynamic_textures[t].Dispose();
			}
			m_all_dynamic_textures.Clear ();
		}

		public static TextureManager create_manager(int count, int dynamic_size, int static_size, bool static_compressed)
		{
			return new TextureManager (count, dynamic_size, static_size, static_compressed);
		}

		public Texture get_static_texture()
		{
			Texture texture = m_free_static_textures.Pop();//TODO
			return texture;
		}

		public Texture get_dynamic_texture()
		{
			Texture texture = m_free_dynamic_textures.Pop();
			return texture;
		}

		public Texture get_custom_texture()
		{
			return Texture.create_texture(this, TextureType.CustomTexture);
		}

		public void release_texture(Texture texture)
		{
			if (texture.get_type() == TextureType.StaticTexture)
			{
				m_free_static_textures.Push(texture);
			}
			else if (texture.get_type() == TextureType.DynamicTexture)
			{
				m_free_dynamic_textures.Push(texture);
			}
			else 
			{
				texture.Dispose();
			}
		}
	}
}

