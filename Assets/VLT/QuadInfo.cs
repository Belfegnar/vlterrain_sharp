using System.Collections;
using System.IO;
using CGLA;

namespace VLT
{
	[System.Serializable]
	public class QuadInfo {

		[UnityEngine.SerializeField]int _id;
		[UnityEngine.SerializeField]int[] _child_ids = new int[4];
		[UnityEngine.SerializeField]float _error;
		[UnityEngine.SerializeField]float _skirt_depth;
		
		[UnityEngine.SerializeField]Vec2f _texture_origin;
		[UnityEngine.SerializeField]float _texture_size;
		
		[UnityEngine.SerializeField]BoundingBox _bounding_box;
		[UnityEngine.SerializeField]DualHeightMap _heightmap = new DualHeightMap();//TODO???
		[UnityEngine.SerializeField]Image _image;

		public QuadInfo(int id)
		{
			_id = id;
			_error = 0.0f;
			_skirt_depth = 0.0f;
			_texture_size = 0.0f;
			_image = null;
			_bounding_box = null;
			for (int c = 0; c < 4; c++)
			{
				_child_ids[c] = -1;
			}
		}

		public QuadInfo(int id, BinaryReader stream)
		{
			_id = id;
			_error = stream.ReadSingle ();
			_skirt_depth = stream.ReadSingle ();
			_child_ids = new int[]{stream.ReadInt32 (), stream.ReadInt32 (), stream.ReadInt32 (), stream.ReadInt32 ()};

			_bounding_box = new BoundingBox (stream);
			
			_heightmap.set_base_map (HeightMap.Create (stream));
			
			_image = Image.Create (stream);
			
			_texture_origin = _heightmap.get_origin ().ToVec2f();
			_texture_size = (_heightmap.get_size () - 1) * _heightmap.get_spacing ();
		}

		public QuadInfo(int id, float error, float skirt_depth, 
		         Vec2f texture_origin, float texture_size, 
		         HeightMap base_map, HeightMap detail_map, Image image, 
		         BoundingBox bounding_box)
		{
			_id = id;
			_error = error;
			_skirt_depth = skirt_depth;
			_image = image;
			_bounding_box = bounding_box;
			_texture_origin = texture_origin;
			_texture_size = texture_size;
			_heightmap.set_base_map(base_map);
			_heightmap.set_detail_map(detail_map);
			
			for (int c = 0; c < 4; c++)
			{
				_child_ids[c] = -1;
			}
		}
		
		public virtual void Dispose()
		{
			_bounding_box.Dispose ();
			_image.Dispose();
		}
		
		public void serialize(BinaryWriter stream)
		{
			stream.Write (_error);
			stream.Write (_skirt_depth);
			stream.Write (_child_ids [0]); stream.Write (_child_ids [1]); stream.Write (_child_ids [2]); stream.Write (_child_ids [3]);
			
			_bounding_box.serialize(stream);
			_heightmap.get_base_map().serialize(stream);
			_image.serialize(stream);
		}
		
		public void set_child_id(int child, int id)
		{
			_child_ids[child] = id;
		}

		public void set_error(float error){_error = error;}

		public void set_skirt_depth(float skirt_depth){_skirt_depth = skirt_depth;}

		public void set_bounding_box(BoundingBox bounding_box)
		{
			_bounding_box = bounding_box;
		}

		public void set_base_map(HeightMap base_map){_heightmap.set_base_map(base_map);}

		public void set_detail_map(HeightMap detail_map){_heightmap.set_detail_map(detail_map);}

		public void set_image(Image image)
		{
			_image = image;
		}
		
		public void replace_image(Image image)
		{
			_image = image;
		}
		
		public int get_id(){return _id;}

		public int get_child_id(int child)
		{
			return _child_ids[child];
		}
		
		public float get_error(){return _error;}
		public float get_skirt_depth(){return _skirt_depth;}
		public Vec2f get_texture_origin(){return _texture_origin;}
		public float get_texture_size(){return _texture_size;}
		public BoundingBox get_bounding_box(){return _bounding_box;}
		
		public HeightMap get_base_map(){return _heightmap.get_base_map();}
		public HeightMap get_detail_map(){return _heightmap.get_detail_map();}
		public DualHeightMap get_heightmap(){return _heightmap;}
		
		public Image get_image(){return _image;}

		public void draw_gizmos()
		{
			_bounding_box.draw_gizmos();
		}
	}
}