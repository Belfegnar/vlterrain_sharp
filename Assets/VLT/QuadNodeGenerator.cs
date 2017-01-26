using System.Collections;
using System.Collections.Generic;
using CGLA;

namespace VLT
{
	[System.Serializable]
	public class QuadNodeGenerator : System.IDisposable {

		class Request
		{
			QuadNode m_parent;
			bool m_static;
			/*QuadId*/int m_child_id;
			int m_child_no;

			public Request(QuadNode parent, bool is_static, int child_id, int child_no)
			{
				m_parent = parent;
				m_static = is_static;
				m_child_id = child_id;
				m_child_no = child_no;
			}

			public bool is_static() { return m_static; }
			public int get_child_id() { return m_child_id; }
			public int get_child_no() { return m_child_no; }
			public QuadNode get_parent() { return m_parent; }
		};

		class Communicator : System.IDisposable
		{
//			HANDLE m_incoming_mutex;
//			HANDLE m_outgoing_mutex;
//			HANDLE m_not_empty_mutex;

			List<Request> m_incoming_requests = new List<Request>();
			List<QuadInfo> m_outgoing_responses = new List<QuadInfo>();

			public Communicator()
			{
//				m_incoming_mutex = CreateMutex(NULL, FALSE, NULL);
//				assert(m_incoming_mutex != 0);
//
//				m_outgoing_mutex = CreateMutex(NULL, FALSE, NULL);
//				assert(m_outgoing_mutex != 0);
//
//				m_not_empty_mutex = CreateMutex(NULL, FALSE, NULL);
//				assert(m_not_empty_mutex != 0);
			}
			public void Dispose()
			{
//				CloseHandle(m_incoming_mutex);
//				m_incoming_mutex = 0;
//
//				CloseHandle(m_outgoing_mutex);
//				m_outgoing_mutex = 0;
//
//				CloseHandle(m_not_empty_mutex);
//				m_not_empty_mutex = 0;
			}

			public void add_incoming_request(Request request)
			{
//				WaitForSingleObject(m_incoming_mutex, INFINITE);

				bool was_empty = m_incoming_requests.Count == 0;

				m_incoming_requests.Add(request);

//				ReleaseMutex(m_incoming_mutex);

				if (was_empty)
				{
//					ReleaseMutex(m_not_empty_mutex);
				}
			}

			public Request get_incoming_request()
			{
//				WaitForSingleObject(m_incoming_mutex, INFINITE);

//				assert(!m_incoming_requests.empty());
				Request request = null;
				if (m_incoming_requests.Count != 0) {
					request = m_incoming_requests [0];
					m_incoming_requests.RemoveAt (0);
				}

//				ReleaseMutex(m_incoming_mutex);

				return request;    
			}

			public void wait_for_incoming_request()
			{
//				while (m_incoming_requests.Count == 0)
				{
//					WaitForSingleObject(m_not_empty_mutex, INFINITE);
				}
			}

			public void add_outgoing_response(QuadInfo info)
			{
//				WaitForSingleObject(m_outgoing_mutex, INFINITE);

				m_outgoing_responses.Add(info);

//				ReleaseMutex(m_outgoing_mutex);
			}

			public QuadInfo get_outgoing_response()
			{
//				WaitForSingleObject(m_outgoing_mutex, INFINITE);

				QuadInfo info;

				if (m_outgoing_responses.Count != 0)
				{
					info = m_outgoing_responses[0];
					m_outgoing_responses.RemoveAt(0);
				}
				else
				{
					info = null;
				}    

//				ReleaseMutex(m_outgoing_mutex);

				return info;
			}
		};


		Communicator m_communicator = new Communicator();
		static QuadNodeGenerator m_instance;

		volatile bool m_running;
//		HANDLE m_thread_handle;

		public static void thread_proc(object parameter)
		{
			// FIXME: deadlock if no request is ever issued!! :-/
			m_instance.m_communicator.wait_for_incoming_request();
			Request request = null;
			//do 
			{
				request = m_instance.m_communicator.get_incoming_request();

				if (request == null)
					return;
				QuadInfo info = null;
				if (request.is_static())
				{
					info = m_instance.m_reader.create_info(request.get_child_id());
				}
				else
				{
					info = m_instance.m_synthesizer.create_info(request.get_parent(), request.get_child_no());
				}

				m_instance.m_communicator.add_outgoing_response(m_instance.calculate_info(info, request.get_parent()));

				m_instance.m_communicator.wait_for_incoming_request();
			}// while (request != null);// m_instance.m_running); TODO

//			return 0;
		}

		void create_thread()
		{
			m_running = true;

//			DWORD thread_id;
//			m_thread_handle = CreateThread(NULL, 0, thread_proc, 0, 0, &thread_id);
//			assert(m_thread_handle != 0);

			/* Thread priorities:
		    THREAD_PRIORITY_TIME_CRITICAL 
		    THREAD_PRIORITY_HIGHEST 
		    THREAD_PRIORITY_ABOVE_NORMAL 
		    THREAD_PRIORITY_NORMAL 
		    THREAD_PRIORITY_BELOW_NORMAL 
		    THREAD_PRIORITY_LOWEST 
		    THREAD_PRIORITY_IDLE
		    */
//			SetThreadPriority(m_thread_handle, THREAD_PRIORITY_NORMAL);
		}
		void destroy_thread()
		{
			m_running = false;

			m_communicator.add_incoming_request(null);
//			WaitForSingleObject(m_thread_handle, INFINITE);
//
//			CloseHandle(m_thread_handle);
//			m_thread_handle = 0;
		}



		[UnityEngine.SerializeField]Render m_render;

		[UnityEngine.SerializeField]QuadTreeReader m_reader;
		QuadSynthesizer m_synthesizer;
		HeightMapMesher m_mesher;

		NormalMapGenerator m_normal_map_generator;
		RenderState m_texture_render_state;

		Dictionary<int, QuadInfo> m_ready_map = new Dictionary<int, QuadInfo>(50);

		QuadInfo calculate_info(QuadInfo info, QuadNode parent)
		{
			info.set_detail_map(m_synthesizer.create_detail_map(info.get_base_map()));

			if (info.get_bounding_box() == null)
			{
				info.set_bounding_box(new BoundingBox(info.get_heightmap()));
			}

			// calculate "displacements array"
			// calculate "light array"

			//m_heightmap_mesher->create_vertex_array(heightmap, skirt_depth)

			return info;
		}

		static byte[] light_array = new byte[64000];
		QuadNode create_node(QuadInfo info, QuadNode parent)
		{
			VertexArray vertex_array = m_mesher.create_vertex_array(info.get_heightmap(), info.get_skirt_depth());

			TerrainMesh mesh = null;
			if (info.get_image() != null)
			{
				mesh = m_render.create_mesh(vertex_array, info.get_image());
			}
			else
			{
				mesh = m_render.create_mesh(vertex_array, parent.get_mesh().get_static_texture());
			}

			QuadNode node = new QuadNode(parent, info, mesh);

			m_normal_map_generator.calculate_light(node.get_heightmap(), light_array);

			m_render.set_render_target(Render.RenderTarget.OffscreenBuffer);
			m_render.set_state (m_texture_render_state);
//			m_texture_render_state.enable();
//
			TextureRenderState texture_render_state = m_texture_render_state as TextureRenderState;

			float invsize = 1.0f / (node.get_heightmap().get_spacing() * (node.get_heightmap().get_size() - 1));

			texture_render_state.set_diffuse_map(node.get_mesh().get_static_texture());
//			texture_render_state.set_material_maps (m_synthesizer.get_material());

			Vec2f texture_origin = node.get_texture_origin();
			Vec3d chunk_origin = node.get_heightmap().get_origin();

			texture_origin.x -= (float)(chunk_origin.x);
			texture_origin.y -= (float)(chunk_origin.y);

			float texture_size = node.get_texture_size() * invsize;

			texture_origin = texture_origin * invsize;

			texture_render_state.set_texture_origin_and_size(texture_origin, texture_size);

			MemoryBlock light_block = m_mesher.create_light_array(light_array);

			m_render.draw(node.get_mesh(), light_block);

			m_render.copy_texture(node.get_mesh().get_dynamic_texture());
			m_render.set_render_target(Render.RenderTarget.FrameBuffer);

//			m_texture_render_state.disable();

			return node;
		}

		public QuadNodeGenerator(string filename, Render render)
		{
			m_render = render;
			m_instance = this;

			m_synthesizer = new QuadSynthesizer();
			m_reader = QuadTreeReader.create_reader(filename);

			m_mesher = new HeightMapMesher();

			m_normal_map_generator = new NormalMapGenerator(256, 0.0625f);
			m_texture_render_state = m_render.create_state(RenderState.State.TextureState);

			create_thread();
		}

		public void Dispose()
		{
			destroy_thread();

			m_mesher.Dispose();

			m_reader.Dispose();
			m_synthesizer.Dispose();

			m_texture_render_state.Dispose();
			m_normal_map_generator.Dispose();

			m_instance = null;
		}

		public QuadNode create_root()
		{
			return create_node(calculate_info(m_reader.create_info(0), null), null);
		}

		public void request_children(QuadNode parent)
		{
			for (int c = 0; c < 4; c++)
			{
				m_communicator.add_incoming_request(new Request(parent, parent.has_static_children(), parent.get_child_id(c), c));
			}
		}

		public void update()
		{
			QuadInfo info = m_communicator.get_outgoing_response();

			while (info != null)
			{
				m_ready_map.Add(info.get_id(), info);

				info = null;//m_communicator.get_outgoing_response();
			}
		}

		public QuadNode get_node(int id, QuadNode parent)
		{
			if (m_ready_map.Count != 0) {
				QuadInfo info;
				if (m_ready_map.TryGetValue (id, out info)) {

					m_ready_map.Remove(id);

					return create_node(info, parent);
				}
			}
			return null;
		}
	}
}