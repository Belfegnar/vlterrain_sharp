using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;
using System.IO;

namespace VLT
{
	[Serializable]
	public class Image : IDisposable
	{
		[UnityEngine.SerializeField]int m_size;
		[UnityEngine.SerializeField]bool m_compressed;
		uint[] m_pixels;

		static byte[] _converter;
		static Stack<Image> _free_images = new Stack<Image> ();

		public static Image Create(int size)
		{
			if (_free_images.Count == 0)
				return new Image (size);
			else {
				Image result = _free_images.Pop ();
				result.Reuse (size);
				return result;
			}
		}

		public static Image Create(int size, bool compressed)
		{
			if (_free_images.Count == 0)
				return new Image (size, compressed);
			else {
				Image result = _free_images.Pop ();
				result.Reuse (size, compressed);
				return result;
			}
		}

		public static Image Create(BinaryReader stream)
		{
			if (_free_images.Count == 0)
				return new Image (stream);
			else {
				Image result = _free_images.Pop ();
				result.Reuse (stream);
				return result;
			}
		}

		uint color_lerp(uint color_0, uint color_1, float t)
		{
			uint r_0 = color_0 & 0xff;
			uint r_1 = color_1 & 0xff;

			uint g_0 = (color_0 >> 8) & 0xff;
			uint g_1 = (color_1 >> 8) & 0xff;

			uint b_0 = (color_0 >> 16) & 0xff;
			uint b_1 = (color_1 >> 16) & 0xff;

			return MathExt.lerp(r_0, r_1, t) | MathExt.lerp(g_0, g_1, t) << 8 | MathExt.lerp(b_0, b_1, t) << 16;
		}

		public Image(int size)
		{
			Reuse (size);
		}

		public Image(int size, bool compressed)
		{
			Reuse (size, compressed);
		}

//		public Image(int size, bool compressed, uint[] pixels)
//		{
//			m_size = size;
//			m_compressed = compressed;
//			int pixel_size = m_size * m_size / (m_compressed ? 8 : 1);
//
//			m_pixels = new uint[pixel_size];
//			Buffer.BlockCopy (pixels, 0, m_pixels, 0, pixel_size * sizeof(uint));
//		}

		public Image(BinaryReader stream)
		{
			Reuse (stream);
		}

		public void Reuse(int size)
		{
			m_size = size;
			m_compressed = false;

			if(m_pixels == null || m_pixels.Length != m_size * m_size)
				m_pixels = new uint[m_size * m_size];
		}

		public void Reuse(int size, bool compressed)
		{
			m_size = size;
			m_compressed = compressed;

			if(m_pixels == null || m_pixels.Length != (m_size * m_size / (m_compressed ? 8 : 1)))
				m_pixels = new uint[m_size * m_size / (m_compressed ? 8 : 1)];
		}

		public void Reuse(BinaryReader stream)
		{
			m_size = stream.ReadInt32 ();

			int compressed = stream.ReadInt32 ();
			m_compressed = (compressed != 0);

			int pixel_size = m_size * m_size / (m_compressed ? 8 : 1);
			if(m_pixels == null || m_pixels.Length != pixel_size)
				m_pixels = new uint[pixel_size];

			if (_converter == null || _converter.Length < pixel_size * sizeof(uint))
				_converter = new byte[pixel_size * sizeof(uint)];

			stream.Read (_converter, 0, pixel_size * sizeof(uint));

			Buffer.BlockCopy (_converter, 0, m_pixels, 0, pixel_size * sizeof(uint));
		}

		public virtual void Dispose()
		{
			_free_images.Push (this);
			//m_pixels = null;
		}

		public void serialize(BinaryWriter stream)
		{
			stream.Write (m_size);

			int compressed = m_compressed ? 1 : 0;
			stream.Write (compressed);

			int pixel_size = m_size * m_size / (m_compressed ? 8 : 1);

			if (_converter == null || _converter.Length != m_pixels.Length * sizeof(uint))
				_converter = new byte[m_pixels.Length * sizeof(uint)];
			Buffer.BlockCopy (m_pixels, 0, _converter, 0, m_pixels.Length * sizeof(uint));
			stream.Write(_converter);
		}
			
		public static Image create_combined_image(Image image_0, Image image_1,	Image image_2, Image image_3)
		{

			Image combined_image = Image.Create(image_0.get_size());

			int size = combined_image.get_size();
			int half_size = size / 2;
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					float double_x = x * 2 + 0.5f;
					float double_y = y * 2 + 0.5f;

					uint pixel;

					if (x < half_size)
					{
						if (y < half_size)
						{
							pixel = image_0.get_pixel(double_x, double_y);
						}
						else
						{
							pixel = image_3.get_pixel(double_x, double_y - size);
						}
					}
					else
					{
						if (y < half_size)
						{
							pixel = image_1.get_pixel(double_x - size, double_y);
						}
						else
						{
							pixel = image_2.get_pixel(double_x - size, double_y - size);
						}
					}

					combined_image.set_pixel(pixel, x, y);
				}
			}

			return combined_image;
		}

		public Image create_sub_image(Vec2f start, Vec2f end, int sub_image_size)
		{

			Image sub_image = new Image(sub_image_size);

			float step = (end[0] - start[0]) / (sub_image_size - 1);

			for (int x = 0; x < sub_image_size; x++)
			{
				for (int y = 0; y < sub_image_size; y++)
				{
					sub_image.set_pixel(get_pixel(start[0] + x * step, start[1] + y * step), x, y);
				}
			}

			return sub_image;
		}

		public int get_size()
		{
			return m_size;
		}

		public bool is_compressed()
		{
			return m_compressed;
		}

		public uint[] get_pixels()
		{
			return m_pixels;
		}

		public uint get_pixel(int x, int y)
		{
			return m_pixels[y * m_size + x];
		}
		public uint get_pixel(float x, float y)
		{

			x = MathExt.clamp(x, 0.0f, (float)(m_size - 1));
			y = MathExt.clamp(y, 0.0f, (float)(m_size - 1));

			int x_floor = (int)(x);
			int x_ceil = MathExt.clamp((int)(x + 1), 0, (int)(m_size - 1));

			int y_floor = (int)(y);
			int y_ceil = MathExt.clamp((int)(y + 1), 0, (int)(m_size - 1));

			float factor_x = x - x_floor;
			float factor_y = y - y_floor;

			uint color_0 = color_lerp(get_pixel(x_floor, y_floor), get_pixel(x_ceil, y_floor), factor_x);
			uint color_1 = color_lerp(get_pixel(x_floor, y_ceil), get_pixel(x_ceil, y_ceil), factor_x);

			return color_lerp(color_0, color_1, factor_y);
		}

		public void set_pixel(uint pixel, int x, int y)
		{
			m_pixels[y * m_size + x] = pixel;
		}

		public void set_row(uint[] pixel_row, int y)
		{
			Buffer.BlockCopy (pixel_row, 0, m_pixels, y * m_size, m_size * sizeof(uint));
		}
	}
}

