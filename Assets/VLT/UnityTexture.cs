using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	[Serializable]
	public class UnityTexture : Texture
	{
		[UnityEngine.SerializeField]UnityEngine.Texture2D m_unity_texture;
		static byte[] converter;

		protected override void initialize_image(Image image)
		{
			if (image.is_compressed ()) {
				m_unity_texture = new UnityEngine.Texture2D (image.get_size (), image.get_size(), UnityEngine.TextureFormat.DXT1, false);
			} else {
				m_unity_texture = new UnityEngine.Texture2D (image.get_size (), image.get_size(), UnityEngine.TextureFormat.ARGB32, false);//Or RGBA32?
			}
			m_unity_texture.wrapMode = UnityEngine.TextureWrapMode.Clamp;
			uint[] tmpList = image.get_pixels ();
			if (converter == null || converter.Length != tmpList.Length * sizeof(uint))
				converter = new byte[tmpList.Length * sizeof(uint)];
			Buffer.BlockCopy (tmpList, 0, converter, 0, converter.Length);
			m_unity_texture.LoadRawTextureData (converter);
			m_unity_texture.Apply ();

//			if (!Program.instance.images.Contains (m_unity_texture))
//				Program.instance.images.Add (m_unity_texture);
		}
		protected override void replace_image(Image image)
		{
			if (m_unity_texture == null || m_unity_texture.width != image.get_size () || m_unity_texture.height != image.get_size ()) {
				UnityEngine.GameObject.DestroyImmediate (m_unity_texture);
				initialize_image (image);
			} else {
//				if (image.is_compressed ()) {
//					m_unity_texture = new UnityEngine.Texture2D (image.get_size (), image.get_size (), UnityEngine.TextureFormat.DXT1, false);
//				} else {
//					m_unity_texture = new UnityEngine.Texture2D (image.get_size (), image.get_size (), UnityEngine.TextureFormat.ARGB32, false);
//				}
				uint[] tmpList = image.get_pixels ();
				if (converter == null || converter.Length != tmpList.Length * sizeof(uint))
					converter = new byte[tmpList.Length * sizeof(uint)];
				Buffer.BlockCopy (tmpList, 0, converter, 0, converter.Length);
				m_unity_texture.LoadRawTextureData (converter);
				m_unity_texture.Apply ();
//				if (!Program.instance.images.Contains (m_unity_texture))
//					Program.instance.images.Add (m_unity_texture);
			}
		}

		public override void set_image(string asset_name)
		{
			UnityEngine.Texture2D newTexture = UnityEngine.Resources.Load<UnityEngine.Texture2D> (asset_name) as UnityEngine.Texture2D;
			//UnityEngine.GameObject.DestroyImmediate (m_unity_texture);//TODO
			m_unity_texture = newTexture;//UnityEngine.GameObject.Instantiate(newTexture) as UnityEngine.Texture2D;
			m_initialized = true;
//			if (!Program.instance.images.Contains (m_unity_texture))
//				Program.instance.images.Add (m_unity_texture);
		}

		public UnityTexture(TextureManager manager, TextureType type) : base(manager, type)
		{
			m_unity_texture = null;
		}

		public override void Dispose()
		{
			UnityEngine.GameObject.DestroyImmediate (m_unity_texture);
			m_unity_texture = null;
		}

		public UnityEngine.Texture2D get_unity_texture()
		{
			return m_unity_texture;
		}
	}
}

