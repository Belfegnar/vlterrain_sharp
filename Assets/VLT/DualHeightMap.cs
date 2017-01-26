using UnityEngine;
using System.Collections;
using CGLA;

namespace VLT
{
	public class DualHeightMap {
		
		HeightMap _base_map;
		HeightMap _detail_map;

		public DualHeightMap()
		{
			_base_map = null;
			_detail_map = null;
		}
		public void Dispose()
		{
			if(_base_map != null) _base_map.Dispose();
			if(_detail_map != null) _detail_map.Dispose();
		}
		
		public void set_base_map(HeightMap base_map)
		{
			_base_map = base_map;
		}

		public void set_detail_map(HeightMap detail_map)
		{
			_detail_map = detail_map;
		}
		
		public HeightMap get_base_map()
		{
			return _base_map;
		}

		public HeightMap get_detail_map()
		{
			return _detail_map;
		}
		
		public int get_size()
		{
			return _base_map.get_size();
		}

		public float get_spacing()
		{
			return _base_map.get_spacing ();
		}
		
		public float get_min_height()
		{
			return _base_map.get_min_height () + _detail_map.get_min_height ();
		}

		public float get_height_range()
		{
			//assert(_base_map != null);
			//assert(_detail_map != null);
			
			return _base_map.get_height_range() + _detail_map.get_height_range();
		}
		
		public float get_height(int x, int y)
		{
			return _base_map.get_height (x, y) + _detail_map.get_height (x, y);
		}

		public float get_height(float x, float y)
		{
			return _base_map.get_height(x, y) + _detail_map.get_height(x, y);
		}

		public float get_extrapolated_height(int x, int y)
		{
			return _base_map.get_extrapolated_height(x, y) + _detail_map.get_extrapolated_height(x, y);
		}
		
		public Vec2d get_origin()
		{
			return _base_map.get_origin ();
		}

		public Vec3f get_point(int x, int y)
		{
			return _base_map.get_point(x, y) + new Vec3f(0.0f, 0.0f, _detail_map.get_height(x, y));
		}
		
		public float get_morph_delta(int x, int y)
		{
			bool x_even = ((x & 1) == 0);
			bool y_even = ((y & 1) == 0);
			
			float height_0;
			float height_1;
			
			if (x_even && y_even) {
				return 0;
			} else if (!x_even && y_even) {
				height_0 = get_height (x - 1, y);
				height_1 = get_height (x + 1, y);
			} else if (x_even && !y_even) {
				height_0 = get_height (x, y - 1);
				height_1 = get_height (x, y + 1);
			} else {
				height_0 = get_height (x - 1, y - 1);
				height_1 = get_height (x + 1, y + 1);
			}
			
			float target = (height_0 + height_1) * 0.5f;
			return target - get_height (x, y);
		}
	}
}