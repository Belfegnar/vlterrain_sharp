using UnityEngine;
using System.Collections;
using System.IO;
using CGLA;

namespace VLT
{
	[System.Serializable]
	public class BoundingBox {

		[SerializeField]Vec3f[] _extrema = new Vec3f[2];
		
		[SerializeField]Vec3f _center;
		[SerializeField]float _radius;
		
		void calc_sphere()
		{
			Vec3f diag = _extrema [1] - _extrema [0];
			
			_center = _extrema [0] + diag * 0.5f;
			_radius = diag.length() * 0.5f;
		}
		
		//BoundingBox();

		public enum Corner
		{
			left_top_front      = 0,
			right_top_front     = 1,
			right_top_back      = 2,
			left_top_back       = 3,
			left_bottom_front   = 4,
			right_bottom_front  = 5,
			right_bottom_back   = 6,
			left_bottom_back    = 7
		};
		
		public BoundingBox(BoundingBox bounding_box)
		{
			_extrema [0] = bounding_box._extrema [0];
			_extrema [1] = bounding_box._extrema [1];
			_center = bounding_box._center;
			_radius = bounding_box._radius;
		}
		public BoundingBox(DualHeightMap heightmap)
		{
			Vec3f origin = heightmap.get_origin().ToVec2f();
			
			_extrema[0] = _extrema[1] = heightmap.get_point(0, 0) + origin;
			
			for (int x = 0; x < heightmap.get_size(); x++)
			{
				for (int y = 0; y < heightmap.get_size(); y++)
				{
					Vec3f point = heightmap.get_point(x, y) + origin;
					_extrema[0] = Vec3f.Min(_extrema[0], point);
					_extrema[1] = Vec3f.Max(_extrema[1], point);
				}
			}
			
			calc_sphere();
		}

		public BoundingBox(BinaryReader stream)
		{
			_extrema [0] = new Vec3f (stream.ReadSingle (), stream.ReadSingle (), stream.ReadSingle ());
			_extrema [1] = new Vec3f (stream.ReadSingle (), stream.ReadSingle (), stream.ReadSingle ());

			calc_sphere ();
		}

		public virtual void Dispose()
		{

		}
		
		public void serialize(BinaryWriter stream)
		{
			stream.Write (_extrema [0].x);
			stream.Write (_extrema [0].y);
			stream.Write (_extrema [0].z);
			stream.Write (_extrema [1].x);
			stream.Write (_extrema [1].y);
			stream.Write (_extrema [1].z);
		}

		static byte[][] corner_to_extrema = new byte[][]
		{
			new byte[]{ 0, 1, 1 }, // left_top_front
			new byte[]{ 1, 1, 1 }, // right_top_front
			new byte[]{ 1, 1, 0 }, // right_top_back
			new byte[]{ 0, 1, 0 }, // left_top_back
			new byte[]{ 0, 0, 1 }, // left_bottom_front
			new byte[]{ 1, 0, 1 }, // right_bottom_front
			new byte[]{ 1, 0, 0 }, // right_bottom_back
			new byte[]{ 0, 0, 0 }  // left_bottom_back
		};
		public Vec3f get_corner(Corner corner)
		{
			return new Vec3f(_extrema[corner_to_extrema[(int)corner][0]][0],
			                   _extrema[corner_to_extrema[(int)corner][1]][1],
			                   _extrema[corner_to_extrema[(int)corner][2]][2]);
		}

		public Vec3f get_center(){return _center;}
		public float get_radius(){return _radius;}

		public void get_extremas(out Vec3f extrema0, out Vec3f extrema1)
		{
			extrema0 = _extrema [0];
			extrema1 = _extrema [1];
		}
		
		public float get_distance(Vec3f point)
		{
			Vec3f distance;
			distance.x = 0;
			distance.y = 0;
			distance.z = 0;
			
			for (int e = 0; e < 3; e++)
			{
				distance[e] = System.Math.Min(System.Math.Min(point[e] - _extrema[0][e], _extrema[1][e] - point[e]), 0.0f);
			}
			
			return distance.length();
		}
		
		public bool is_above(Plane plane)
		{
			float sphere_distance = plane.distance_to_point(_center);
			if (sphere_distance < -_radius)
			{
				return false;
			} 
			else if (sphere_distance >= _radius)
			{
				return true;
			}
			
			for (int c = 0; c < 8; c++)
			{
				if (plane.is_point_above(get_corner((Corner)c)))
				{
					return true;
				}
			}
			
			return false;
		}

		public void draw_gizmos()
		{
			UnityEngine.Vector3 ex0 = _extrema[0].ToVector3();
			UnityEngine.Vector3 ex1 = _extrema[1].ToVector3();
			UnityEngine.Vector3 center = _center.ToVector3();

			UnityEngine.Gizmos.DrawWireCube (center, (ex1 - ex0));
		}
	}
}