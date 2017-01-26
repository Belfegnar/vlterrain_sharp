using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VLT
{
	[System.Serializable]
	public class QuadCache {

		[SerializeField]int _cache_size;
		[SerializeField]bool _create_detail_nodes;
		[SerializeField]QuadNodeGenerator _node_generator;
		[SerializeField]int _max_nodes_per_frame;
		[SerializeField]float _living_age;
		[SerializeField]int _living_frames;
		
		[SerializeField]List<TimeNodePair> _lru_list;
		[SerializeField]List<QuadNode> _delete_list = new List<QuadNode>(50);
		[SerializeField]List<QuadNode> _request_list = new List<QuadNode>(50);
		Dictionary<int, QuadNode> _allNodes;
		int _current_frame;
		
		void add_node_to_lru(QuadNode node)
		{
			int nodeId = node.get_info ().get_id ();
			_lru_list.Insert(0, new TimeNodePair(/*Time.time*/_current_frame, nodeId));
			if (!_allNodes.ContainsKey (nodeId))
				_allNodes.Add (nodeId, node);

			node.set_cache_id (_lru_list [0]);
		}

		void remove_children_from_lru(QuadNode node)
		{

			if (node.are_children_resident())
			{
				for (int c = 0; c < 4; c++)
				{
					QuadNode child = node.get_child(c);
					remove_children_from_lru(child);

					TimeNodePair tnp = child.get_cache_id ();
					for (int i = 0; i < _lru_list.Count; i++) //_lru_list.Remove(child.get_cache_id());
					{
						if (_lru_list [i] == tnp) {
							_lru_list.RemoveAt (i);
							break;
						}
					}
					child.set_cache_id(new TimeNodePair());//QuadCacheId());

					int index = _request_list.IndexOf(child);
					if(index >= 0)
					{
						_request_list.RemoveAt(index);
					}
					
					_delete_list.Add(child);
				}
				
				node.clear_children();
			}
		}
		
		void add_new_nodes()
		{
			int new_count = 0;
			
			QuadNode list_it;
			for(int i = 0 ; i < _request_list.Count && new_count < 1/*_max_nodes_per_frame - 4*/; i++)// FIXME
			{
				list_it = _request_list[i];
				for (int c = 0; c < 4; c++)
				{
					QuadNode node = _node_generator.get_node(list_it.get_child_id(c), list_it);
					if (node != null)
					{
						list_it.set_child(c, node);
						
						new_count++;
					}
				}
				
				if (list_it.are_children_resident())
				{
					for (int c = 0; c < 4; c++)
					{
						add_node_to_lru(list_it.get_child(c));
					}

					_request_list.RemoveAt(i);
					--i;//TODO ????
				}
				else
				{
					//++i;//TODO
				}
			}
		}

		void delete_old_nodes()
		{
			int it = _lru_list.Count;
			if (it == 0)
				return;
			
			--it;
			
			float time_now = Time.time;
			
			while ((_lru_list.Count != 0) &&
			       //(_lru_list.Count > _cache_size / 10 && _lru_list[it].time < time_now - _living_age) ||
				(_lru_list.Count > _cache_size / 10 && _lru_list[it].frame < _current_frame - _living_frames) ||
			       (_lru_list.Count > _cache_size - _max_nodes_per_frame * 2))
			{
				remove_children_from_lru(_allNodes[_lru_list[it].node].get_parent());
				
				it = _lru_list.Count;
				--it;
			}
			
			int delete_count = 0;
			while ((_delete_list.Count != 0) && delete_count < _max_nodes_per_frame)
			{
				_allNodes.Remove (_delete_list [0].get_info ().get_id ());
				_delete_list[0].Dispose();
				_delete_list.RemoveAt(0);
				
				delete_count++;
			}
		}

		public QuadCache(string filename, Render render)
		{
			_max_nodes_per_frame = 5;
			_living_age = 10.0f;
			_living_frames = (int)(60.0f * _living_age);

			_cache_size = Configuration.get_int("quad_cache_size");
			_create_detail_nodes = Configuration.get_bool("detail_create_new_nodes");

			_lru_list = new List<TimeNodePair> (_cache_size);
			_allNodes = new Dictionary<int, QuadNode> (_cache_size);
			
			_node_generator = new QuadNodeGenerator(filename, render);
			
			if (Configuration.get_bool("video_mode"))
			{
				_living_age = 1000.0f;
				_living_frames = (int)(60.0f * _living_age);
			}
		}

		public void Dispose()
		{
			_node_generator.Dispose();
		}
		
		public void node_used(QuadNode node)
		{
			if (node.get_parent () != null) {
				TimeNodePair tnp = node.get_cache_id ();
				for (int i = 0; i < _lru_list.Count; i++) //_lru_list.Remove (node.get_cache_id ());
				{
					if (_lru_list [i] == tnp) {
						_lru_list.RemoveAt (i);
						break;
					}
				}

				add_node_to_lru (node);
			}
		}
		
		public void update(int currentFrame)
		{
			_current_frame = currentFrame;

			delete_old_nodes ();
			
			_node_generator.update ();
			
			add_new_nodes ();
		}
		
		public QuadNode create_root(){return _node_generator.create_root();}

		public bool request_children(QuadNode parent)
		{
			if (!_create_detail_nodes && !parent.has_static_children())
			{
				return true;
			}
			
			if (_request_list.Count < 2) // FIXME
			{
				_request_list.Add(parent);
				
				_node_generator.request_children(parent);
				
				return true;
			}
			else
			{
				return false;
			}
		}
			
	}

	public struct TimeNodePair
	{
		public int frame;
		public int node;

		public TimeNodePair(int t, int n)
		{
			frame = t;
			node = n;
		}

//		public override bool Equals(System.Object obj)
//		{
//			if (!(obj is TimeNodePair))
//				return false;
//
//			TimeNodePair mys = (TimeNodePair) obj;
//
//			return (frame == mys.frame) && (node == mys.node);
//		}

		public bool Equals(TimeNodePair p)
		{
			return (frame == p.frame) && (node == p.node);
		}

		public static bool operator ==(TimeNodePair a, TimeNodePair b) {
			return a.frame == b.frame && a.node == b.node;
		}

		public static bool operator !=(TimeNodePair a, TimeNodePair b) {
			return a.frame != b.frame || a.node != b.node;
		}

		public override int GetHashCode()
		{
			return frame ^ node << 2;
		}
	}
}