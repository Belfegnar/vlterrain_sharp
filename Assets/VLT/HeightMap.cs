using CGLA;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VLT
{
	public class HeightMap {

		int _size;
		int _allocation_size;
		float _spacing;
		Vec2d _origin;
		float _minimum;
		float _maximum;
		
//		union
//		{
		ushort[] _short_heights;
		float[] _float_heights;
//		}
		bool _compacted;

		static byte[] _convertor;
		static Stack<HeightMap> _free_heightmaps = new Stack<HeightMap> ();

		public static HeightMap Create(int size, float spacing, Vec2d origin)
		{
			if (_free_heightmaps.Count == 0)
				return new HeightMap (size, spacing, origin);
			else {
				HeightMap result = _free_heightmaps.Pop ();
				result.Reuse (size, spacing, origin);
				return result;
			}
		}

		public static HeightMap Create(BinaryReader stream)
		{
			if (_free_heightmaps.Count == 0)
				return new HeightMap (stream);
			else {
				HeightMap result = _free_heightmaps.Pop ();
				result.Reuse (stream);
				return result;
			}
		}

		HeightMap()
		{
			_size = 0;
			_allocation_size = 0;
			_spacing = 0;
			_maximum = float.MinValue;
			_minimum = float.MaxValue;
			_float_heights = null;
			_compacted = false;
		}

		public HeightMap(int size, float spacing, Vec2d origin)
		{
			Reuse (size, spacing, origin);
		}

		public HeightMap(BinaryReader stream)
		{
			Reuse (stream);
		}

		public void Reuse(int size, float spacing, Vec2d origin)
		{
			_size = size;
			_allocation_size = size + 2;
			_spacing = spacing;
			_maximum = float.MinValue;
			_minimum = float.MaxValue;
			_origin = origin;
			_compacted = false;

			if(_float_heights == null || _float_heights.Length != _allocation_size * _allocation_size)
				_float_heights = new float[_allocation_size * _allocation_size];
		}

		public void Reuse(BinaryReader stream)
		{
			_compacted = true;
			_size = stream.ReadInt32 ();
			_spacing = stream.ReadSingle ();
			_origin.x = stream.ReadDouble ();
			_origin.y = stream.ReadDouble ();

			_allocation_size = _size + 2;

			_minimum = stream.ReadSingle ();
			_maximum = stream.ReadSingle ();
			if(_short_heights == null || _short_heights.Length != _allocation_size * _allocation_size)
				_short_heights = new ushort[_allocation_size * _allocation_size];

			if (_convertor == null || _convertor.Length < _short_heights.Length * sizeof(ushort))
				_convertor = new byte[_short_heights.Length * sizeof(ushort)];

			stream.Read(_convertor, 0, _short_heights.Length * sizeof(ushort));
			System.Buffer.BlockCopy (_convertor, 0, _short_heights, 0, _short_heights.Length * sizeof(ushort));
		}

		public virtual void Dispose()
		{
			_free_heightmaps.Push (this);
//			_float_heights = null;
//			_short_heights = null;
		}
		
		public void serialize(BinaryWriter stream)
		{
			stream.Write (_size);
			stream.Write (_spacing);
			stream.Write (_origin.x);
			stream.Write (_origin.y);
			
			if (!_compacted)
			{
				compact();
			}
			
			stream.Write (_minimum);
			stream.Write (_maximum);

			if (_convertor == null || _convertor.Length != _short_heights.Length * sizeof(ushort))
				_convertor = new byte[_short_heights.Length * sizeof(ushort)];

			System.Buffer.BlockCopy (_short_heights, 0, _convertor, 0, _short_heights.Length * sizeof(ushort));
			stream.Write (_convertor);
		}
		
		public int get_size(){return _size;}
		public float get_spacing(){return _spacing;}
		
		public float get_min_height(){return _minimum;}
		public float get_height_range(){return _maximum - _minimum;}
		
		public float get_height(int x, int y)
		{
			if (x < -1)
			{
				x = -1;
			}
			else if (x > _size)
			{
				x = _size;
			}
			
			if (y < -1)
			{
				y = -1;
			}
			else if (y > _size)
			{
				y = _size;
			}
			
			if (!_compacted)
			{
				return _float_heights[(y + 1) * _allocation_size + x + 1];
			}
			else
			{
				return (_short_heights[(y + 1) * _allocation_size + x + 1] * get_height_range()) / 65535.0f + _minimum;
			}
		}

		public float get_height(float x, float y)
		{
			int x_0 = (int)(x);
			int x_1 = x_0 + 1; if(x_1 > _size) x_1 = _size;
			int y_0 = (int)(y);
			int y_1 = y_0 + 1; if(y_1 > _size) y_1 = _size;
			
			return bi_lerp(get_height(x_0, y_0), get_height(x_1, y_0),
			               get_height(x_0, y_1), get_height(x_1, y_1), 
			               x - x_0, y - y_0);
		}

		public float get_extrapolated_height(int x, int y)
		{
			
			if (x >= -1 && x <= _size && y >= -1 && y <= _size)
			{
				return get_height(x, y);
			}
			else if (x >= -1 && x <= _size)
			{
				if (y == -2)
				{
					return 2.0f * get_height(x, -1) - get_height(x, 0);
				}
				else 
				{
					return 2.0f * get_height(x, _size) - get_height(x, _size - 1);
				}
			}
			else if (y >= -1 && y <= _size)
			{
				if (x == -2)
				{
					return 2.0f * get_height(-1, y) - get_height(0, y);
				}
				else 
				{
					return 2.0f * get_height(_size, y) - get_height(_size - 1, y);
				}
			}
			else
			{
				if (x == -2 && y == -2)
				{
					return 2.0f * get_height(-1, -1) - get_height(0, 0);
				}
				else if (x == -2)
				{
					return 2.0f * get_height(-1, _size) - get_height(0, _size - 1);
				}
				else if (y == -2)
				{
					return 2.0f * get_height(_size, -1) - get_height(_size - 1, 0);
				}
				else
				{
					return 2.0f * get_height(_size, _size) - get_height(_size - 1, _size - 1);
				}
			}
		}
		
		public Vec2d get_origin(){return _origin;}

		public Vec3f get_point(int x, int y)
		{
			Vec3f result;
			result.x = x * _spacing;
			result.y = y * _spacing;
			result.z = get_height(x, y);
			return result;
		}
		
		public void set_height(float height, int x, int y)
		{
			if(height < _minimum)
			{
				_minimum = height;
			}
			else if (height > _maximum)
			{
				_maximum = height;
			}
			
			_float_heights[(y + 1) * _allocation_size + x + 1] = height;
		}
		
		public void compact()
		{
			_minimum = get_height(-1, -1);
			_maximum = get_height(-1, -1);
			
			for (int x = -1; x <= _size; x++)
			{
				for (int y = -1; y <= _size; y++)
				{
					float height = get_height(x, y);
					
					if (_minimum > height)
					{
						_minimum = height;
					}
					else if (_maximum < height)
					{
						_maximum = height;
					}
				}
			}
			
			float range = _maximum - _minimum;
			
			ushort[] short_heights = new ushort[_allocation_size * _allocation_size];
			for (int x = 0; x < _allocation_size; x++)
			{
				for (int y = 0; y < _allocation_size; y++)
				{
					float scaled_height = (_float_heights[y * _allocation_size + x] - _minimum) * 65535.0f / range;
					
					short_heights[y * _allocation_size + x] = (ushort)(scaled_height);
				}
			}
			
			_float_heights = null;
			
			_short_heights = short_heights;
			
			_compacted = true;
		}

		float lerp(float start_value, float end_value, float t)
		{
			return start_value + (float)((end_value - start_value) * t);
		}

		float bi_lerp(float u, float v, float q, float r, float s, float t)
		{
			return lerp(lerp(u, v, s), lerp(q, r, s), t);
		}
	}
}