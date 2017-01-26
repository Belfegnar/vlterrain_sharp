using System.Collections;
using System.Collections.Generic;
using CGLA;

namespace VLT
{
	[System.Serializable]
	public class QuadNode {

		QuadNode _parent;
		QuadNode[] _children = new QuadNode[4];
		
		[UnityEngine.SerializeField]QuadInfo _info;
		
		[UnityEngine.SerializeField]bool _has_static_children;
		[UnityEngine.SerializeField]bool _are_children_resident;
		[UnityEngine.SerializeField]bool _has_requested;
		
		[UnityEngine.SerializeField]TerrainMesh _mesh;
		
		[UnityEngine.SerializeField]bool _is_visible;
		
		[UnityEngine.SerializeField]TimeNodePair _cache_id;

		public override bool Equals (object obj)//TODO
		{
			QuadNode other = obj as QuadNode;
			if (other == null)
				return false;
			return _info.get_id() == other._info.get_id();
		}

		public QuadNode(QuadNode parent, QuadInfo info, TerrainMesh mesh)
		{
			_parent = parent;
			_info = info;
			_mesh = mesh;
			_are_children_resident = false;
			_has_requested = false;
			_is_visible = true;

			for (int c = 0; c < 4; c++)
			{
				_children[c] = null;
			}
			
			_has_static_children = _info.get_child_id(0) != -1 && 
				_info.get_child_id(1) != -1 && 
				_info.get_child_id(2) != -1 && 
				_info.get_child_id(3) != -1;
		}

		public void Dispose()
		{
			for (int c = 0; c < 4 ; c++)
			{
				_children[c] = null;
			}
			
#if !PREP
			_mesh.Dispose();
			_mesh = null;
#endif
			_info = null;
		}
		
		public void set_child(int child_number, QuadNode child)
		{
			_children[child_number] = child;
			
			_are_children_resident = (_children[0] != null) && (_children[1] != null) && (_children[2] != null) && (_children[3] != null);
			if (_has_requested && _are_children_resident)
			{
				_has_requested = false;
			}
		}

		public void set_child_id(int child_number, int id)
		{
			_info.set_child_id (child_number, id);
		}
		
		public QuadNode get_parent(){return _parent;}

		public QuadNode get_child(int child_number)
		{
			return _children [child_number];
		}

		public int get_child_id(int child_number)
		{
			return _info.get_child_id(child_number);
		}

		public QuadInfo get_info()//TODO
		{
			return _info;
		}

		public DualHeightMap get_heightmap(){return _info.get_heightmap();}
		
		public Image get_image(){return _info.get_image();}
		
		public float get_error(){return _info.get_error();}
		
		public Vec2d get_origin(){return _info.get_heightmap().get_origin();}
		public Vec3f get_center(){return get_bounding_box().get_center();}
		
		public int get_size(){return _info.get_heightmap().get_size();}
		public float get_spacing(){return _info.get_heightmap().get_spacing();}
		public float get_height(float x, float y)
		{
			Vec2f local_point = (new Vec2d (x, y) - get_origin ()).ToVec2f() * (1.0f / get_spacing ());
			
			return _info.get_heightmap ().get_height (local_point [0], local_point [1]);
		}
		public float get_skirt_depth(){return _info.get_skirt_depth();}
		
		public TerrainMesh get_mesh(){return _mesh;}

		public Vec2f get_texture_origin(){return _info.get_texture_origin();}
		public float get_texture_size(){return _info.get_texture_size();}

		public BoundingBox get_bounding_box(){return _info.get_bounding_box();}
		
		public bool contains(float x, float y)
		{
			return (get_origin () [0] <= x) && (get_origin () [1] <= y) &&
				(get_origin () [0] + (get_size () - 1) * get_spacing () > x) &&
				(get_origin () [1] + (get_size () - 1) * get_spacing () > y);
		}
		
		public bool has_static_children(){return _has_static_children;}
		public bool are_children_resident(){return _are_children_resident;}
		public void request_resident_children(QuadCache cache)
		{
			if (!_has_requested)
			{
#if !PREP
				_has_requested = cache.request_children(this);
#endif
			}
		}
		
		public void clear_children()
		{
			for (int c = 0; c < 4 ; c++)
			{
				set_child(c, null);
			}
		}
		
		public void set_visible(bool visible){_is_visible = visible;}
		public bool is_visible(){return _is_visible;}
		
		public void set_cache_id(TimeNodePair cache_id){_cache_id = cache_id;}
		public TimeNodePair get_cache_id(){return _cache_id;}

		public void draw_gizmos()
		{
			if (is_visible ())
				UnityEngine.Gizmos.color = UnityEngine.Color.white;
			else
				UnityEngine.Gizmos.color = UnityEngine.Color.red;
			_info.draw_gizmos ();

			UnityEngine.Gizmos.color = UnityEngine.Color.white;
			for (int i = 0; i < 4; i++) {
				if (_children [i] != null && _children [i]._is_visible) {
					_children [i].draw_gizmos ();
				}
			}
		}
	}
}
