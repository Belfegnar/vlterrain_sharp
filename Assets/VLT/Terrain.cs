using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	[System.Serializable]
	public class Terrain : IDisposable {

		[UnityEngine.SerializeField]Render m_render;

		[UnityEngine.SerializeField]QuadNode m_quadtree;
		[UnityEngine.SerializeField]QuadCache m_cache;

		[UnityEngine.SerializeField]TerrainRenderState m_terrain_state;

		[UnityEngine.SerializeField]bool m_wireframe;

		[UnityEngine.SerializeField]float m_error_to_distance;

		[UnityEngine.SerializeField]float m_minimum_spacing;
		[UnityEngine.SerializeField]float m_minimum_request_spacing;
		[UnityEngine.SerializeField]bool m_video_mode;

		[UnityEngine.SerializeField]int m_drawn_tris;

		int m_current_frame = 0;

		float recursive_get_height(QuadNode node, float x, float y)
		{
			if(node.are_children_resident())
			{
				for(int	c =	0; c < 4; c++)
				{
					QuadNode child	= node.get_child(c);
					if(child.contains(x, y))
					{
						return recursive_get_height(child, x, y);
					}
				}
			}

			return node.get_height(x, y);
		}

		void recursive_sorted_draw(QuadNode node, Camera camera)
		{

			float[] child_distance = new float[4];
			for (int c = 0; c < 4; c++)
			{
				child_distance[c] = (node.get_child(c).get_center() - new Vec3f(camera.get_eye())).magnitude();//TODO sqrt????
			}

			for (int l = 0; l < 4; l++)
			{
				float min_distance = child_distance[0];
				int min_child = 0;
				for (int c = 1; c < 4; c++)
				{
					if (child_distance[c] < min_distance)
					{
						min_distance = child_distance[c];
						min_child = c;
					}
				}

				recursive_draw(node.get_child(min_child), camera);

				child_distance[min_child] = 1.0e20f;
			}
		}

		void recursive_draw(QuadNode node, Camera camera)
		{

			m_cache.node_used(node);

			node.set_visible(node.get_parent() == null || (node.get_parent().is_visible() && camera.can_see(node)));

			if (should_subdivide(node, camera))
			{
				if (node.are_children_resident())
				{
					recursive_sorted_draw(node, camera);
				}
				else
				{
					if (should_request(node))
					{
						node.request_resident_children(m_cache);
					}

					draw_node(node,	calculate_morph_factor(node, camera), camera);
				}
			}
			else
			{
				draw_node(node,	calculate_morph_factor(node, camera), camera);
			}
		}


		void draw_node(QuadNode node, float morph_factor, Camera camera)  // FIXME ?
		{
			if (node.is_visible())
			{
				float scale = (node.get_size() - 1) * node.get_spacing();

//				m_render.push_transformation();
				m_render.translate(new Vec3d(node.get_origin()));
				m_render.scale(new Vec3d(scale, scale, 1.0));

				m_terrain_state.set_morph_factor(morph_factor);
				m_terrain_state.set_height_offset_scale(node.get_heightmap().get_min_height() - node.get_skirt_depth(),
					node.get_heightmap().get_height_range() + node.get_skirt_depth());
				m_terrain_state.set_diffuse_map (node.get_mesh ().get_dynamic_texture());

				QuadNode parent = node.get_parent();
				if (parent != null)
				{
					m_terrain_state.set_parent_diffuse_map(parent.get_mesh().get_dynamic_texture());
					m_terrain_state.set_parent_offset((node.get_origin() - parent.get_origin()).ToVec2f() / ((parent.get_size() - 1) * parent.get_spacing())); 
				}
				else
				{
					m_terrain_state.set_parent_diffuse_map (node.get_mesh ().get_dynamic_texture());
					m_terrain_state.set_parent_offset(new Vec2f(0,0)); 
				}

//				for (int p = 0; p < m_terrain_state.get_pass_count(); p++)
//				{
//					m_terrain_state.set_pass(p);

					m_render.set_state (m_terrain_state);
					m_render.draw(node.get_mesh());
					//UnityEngine.Debug.Log ("Draw");
//				}
//				m_render.pop_transformation();
				
				m_drawn_tris += node.get_size() * node.get_size() * 2;
			}
		}

		bool should_subdivide(QuadNode node, Camera camera)
		{
			return camera.get_distance(node.get_bounding_box()) < node.get_error() * m_error_to_distance;
		}

		bool should_request(QuadNode node)
		{
			return node.get_spacing() > m_minimum_request_spacing;
		}

		float calculate_morph_factor(QuadNode node, Camera camera)
		{
			if (node.get_parent() != null)
			{
				float camera_distance =	camera.get_distance(node.get_bounding_box());
				float parent_distance =	node.get_parent().get_error() * m_error_to_distance;
				float node_distance	= node.get_error()	* m_error_to_distance;

				return UnityEngine.Mathf.Clamp(1.0f -	(parent_distance - camera_distance)	/ (parent_distance - node_distance), 0.0f, 1.0f);
			}
			else
			{
				return 0.0f;
			}
		}

		public Terrain(Render render, string filename)
		{
			m_render = render;
			m_wireframe = false;
			m_drawn_tris = 0;
			m_video_mode = false;

			m_terrain_state	= m_render.create_state(RenderState.State.TerrainState) as TerrainRenderState;

			m_render.set_state (m_terrain_state);//TODO
			m_cache	= new QuadCache(filename, m_render);

			m_quadtree = m_cache.create_root();

			m_minimum_spacing = Configuration.get_float("lod_minimum_spacing");
			set_error(Configuration.get_float("lod_error"));
			m_video_mode = Configuration.get_bool("video_mode");

			set_camera_velocity(0.0f);
		}

		public void Dispose()
		{
			m_terrain_state.Dispose();
			m_quadtree.Dispose();
			m_cache.Dispose();
		}

		public RenderState get_terrain_render_state()
		{
			return m_terrain_state;
		}

		public void set_error(float max_error)
		{
			float screen_height	= 600.0f; // FIXME!
			float fov =	55.0f; // FIXME

			m_error_to_distance	= screen_height	/ ((float)Math.Abs(Math.Tan(deg_to_rad(fov / 2.0f))) * 2.0f *	max_error);
		}

		float deg_to_rad(float deg)
		{
			return deg * MathExt.M_PI / 180.0f;
		}

		public void set_sun_direction(CGLA.Vec3f direction)
		{
			//m_terrain_state.set_sun_drection(direction); FIXME!!!
		}

		public void set_camera_velocity(float velocity)
		{
			m_minimum_request_spacing = (float)Math.Max(0.01f * velocity, m_minimum_spacing);
		}

		public float get_height(float x, float y)
		{
			if (m_quadtree.contains(x,	y))
			{
				return recursive_get_height(m_quadtree,	x, y);
			}
			else
			{
				return -1.0e4f;
			}
		}

		public void update(float frame_time, UserInterface user_interface)
		{
			user_interface.write(3, "tris/frame:");
			user_interface.append(3, m_drawn_tris);

			user_interface.write(4, "tris/s:");
			user_interface.append(4, m_drawn_tris / frame_time);

			m_drawn_tris = 0;
		}

		public void draw(Camera camera)
		{
			m_cache.update(m_current_frame);
//			m_render.clear_depth_buffer();
			m_render.set_camera(camera);
			m_render.set_state (m_terrain_state);//TODO
//			m_terrain_state.enable();
			recursive_draw(m_quadtree, camera);
//			m_terrain_state.disable();
			m_current_frame++;
		}

		public void toggle_wireframe()
		{
			m_wireframe = !m_wireframe;

			m_terrain_state.set_wireframe(m_wireframe);
		}

		public void draw_gizmos()
		{
			if(m_quadtree != null)
			m_quadtree.draw_gizmos ();
		}
	}
}