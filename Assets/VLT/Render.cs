using System.Collections;
using System.Collections.Generic;
using CGLA;
using System;

namespace VLT
{
	[System.Serializable]
	public class Render : IDisposable {
		
		public enum RenderTarget
		{
			FrameBuffer,
			OffscreenBuffer
		}
			
		Vec3f m_translation = new Vec3f ();
		Vec3f m_scale = new Vec3f ();
		RenderState m_render_state;
		RenderTarget m_target;

		[UnityEngine.SerializeField]TextureManager m_texture_manager;
		[UnityEngine.SerializeField]UnityEngine.Camera m_ortho_cam;
		[UnityEngine.SerializeField]int m_offscreen_buffer_size;
		[UnityEngine.SerializeField]UnityEngine.RenderTexture m_offscreen_texture;
		UnityEngine.MeshFilter m_offscreen_mesh_filter;
		UnityEngine.MeshRenderer m_offscreen_mesh_render;

		void create_offscreen_buffer()
		{
			m_offscreen_buffer_size = Configuration.get_int("dynamic_texture_size");

			m_offscreen_texture = UnityEngine.RenderTexture.GetTemporary (m_offscreen_buffer_size, m_offscreen_buffer_size);

			UnityEngine.GameObject cameraGO = new UnityEngine.GameObject ("offscreen_camera");
			cameraGO.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
			m_ortho_cam = cameraGO.AddComponent<UnityEngine.Camera> ();
			//m_ortho_cam.CopyFrom (UnityEngine.Camera.main);
			m_ortho_cam.depthTextureMode = UnityEngine.DepthTextureMode.None;
			m_ortho_cam.cullingMask = 1 << 31;
			UnityEngine.Camera.main.cullingMask |= ~(1 << 31);
			m_ortho_cam.aspect = 1;
			m_ortho_cam.orthographicSize = 0.5f;
			m_ortho_cam.orthographic = true;
			m_ortho_cam.farClipPlane = 1f;
			m_ortho_cam.nearClipPlane = 0.1f;
			m_ortho_cam.transform.position = new UnityEngine.Vector3 (0.5f, 0, 0.5f);
			m_ortho_cam.clearFlags = UnityEngine.CameraClearFlags.Color;
			m_ortho_cam.backgroundColor = UnityEngine.Color.black;
			m_ortho_cam.transform.forward = UnityEngine.Vector3.down;
			m_ortho_cam.targetTexture = m_offscreen_texture;
			m_ortho_cam.enabled = false;
		}

		void destroy_offscreen_buffer()
		{
			if (m_ortho_cam != null) {
				m_ortho_cam.targetTexture = null;
				UnityEngine.GameObject.DestroyImmediate (m_ortho_cam.gameObject);
			}
			UnityEngine.RenderTexture.ReleaseTemporary (m_offscreen_texture);
		}

		public Render(Engine engine, int width, int height, bool fullscreen)
		{
			create_offscreen_buffer();
			m_target = RenderTarget.FrameBuffer;
		}

		public void Dispose()
		{
			destroy_offscreen_buffer ();
			m_texture_manager.Dispose();
		}

		public void run()
		{

		}

		public void initialize_textures(int count, int dynamic_size, int static_size, bool static_compressed)
		{
			m_texture_manager = new TextureManager(count, dynamic_size, static_size, static_compressed);

		}

		public Texture get_texture(TextureType type)
		{
			if (type == TextureType.StaticTexture)
			{
				return m_texture_manager.get_static_texture();
			}
			else if (type == TextureType.DynamicTexture)
			{
				return m_texture_manager.get_dynamic_texture();
			}
			else
			{
				return m_texture_manager.get_custom_texture();
			}
		}

		public Texture get_texture(Image image, TextureType type)
		{
			Texture texture = get_texture(type);

			texture.set_image(image);

			return texture;
		}

		public void draw(TerrainMesh mesh)
		{
			mesh.bind();

			draw_implementation(mesh);
		}

		public void draw(TerrainMesh mesh, MemoryBlock light_block)
		{
			mesh.bind(light_block);

			draw(mesh);
		}

//		public GenericMesh create_mesh(List<Vertex> vertices, Image image)
//		{
//			return create_mesh_implementation(vertices, image);
//		}

		public TerrainMesh create_mesh(VertexArray vertex_array, Image image)
		{
			return create_mesh(vertex_array, get_texture(image, TextureType.StaticTexture));
		}

		public TerrainMesh create_mesh(VertexArray vertex_array, Texture diffuse_texture)
		{   
			return new TerrainMesh(vertex_array, diffuse_texture, get_texture(TextureType.DynamicTexture));
		}

		uint lastFrame = 1000000;
		public void copy_texture(Texture texture)
		{
			if (lastFrame == Program.instance.currentFrame) {
				UnityEngine.Debug.LogError ("Same frame " + lastFrame);
			}
			lastFrame = Program.instance.currentFrame;
//			(texture as UnityTexture).get_unity_texture ().ReadPixels (new UnityEngine.Rect (0, 0, 128, 128), 0, 0);
//			(texture as UnityTexture).get_unity_texture ().Apply ();
			UnityEngine.Graphics.CopyTexture (m_offscreen_texture, (texture as UnityTexture).get_unity_texture ());
			if (!Program.instance.images.Contains ((texture as UnityTexture).get_unity_texture ())) {
				Program.instance.images.Add ((texture as UnityTexture).get_unity_texture ());
			}
		}

//		public void draw(GenericMesh mesh)
//		{
//			draw_implementation(mesh);
//		}

		public void set_camera(Camera camera)
		{
//			glMatrixMode(GL_PROJECTION);
//			glLoadIdentity();
//			gluPerspective(camera.get_fov(), camera.get_aspect(), 
//				camera.get_z_near(), camera.get_z_far());
//
//			Vec3d eye = camera.get_eye();
//			Vec3d center = camera.get_center();
//			Vec3d up = camera.get_up();
//
//			glMatrixMode(GL_MODELVIEW);
//			glLoadIdentity();
//
//			gluLookAt(eye[0], eye[1], eye[2],
//				center[0], center[1], center[2],
//				up[0], up[1], up[2]);
		}

		public static Render create_render(Engine engine, int width, int height, bool fullscreen)
		{
			return new Render (engine, width, height, fullscreen);
		}

		public RenderState create_state(RenderState.State state)
		{
			switch (state)
			{
			case RenderState.State.TerrainState:
				return new TerrainRenderState();

			case RenderState.State.TextureState:
				return new TextureRenderState();
//
//			case RenderState::SkyboxState:
//				return new GLSkyboxRenderState(m_cg_context);
//
//			case RenderState::UIState:
//				return new GLUIRenderState(m_cg_context);
			}

			return null;
		}

		public void translate(Vec3d translation)
		{
			m_translation = translation.ToVec3f();
		}

		public void scale(Vec3d scale)
		{
			m_scale = scale.ToVec3f();
		}

		public void set_state(RenderState state)
		{
			m_render_state = state;
		}

		public void set_render_target(RenderTarget target)
		{
			if (target == RenderTarget.OffscreenBuffer) {
				//UnityEngine.Graphics.SetRenderTarget (m_offscreen_texture);
			} else {
				//UnityEngine.Graphics.SetRenderTarget (null);
			}
			m_target = target;
		}

		void draw_implementation(TerrainMesh mesh)
		{
			if (m_target == RenderTarget.FrameBuffer) {
				UnityEngine.Matrix4x4 m = UnityEngine.Matrix4x4.TRS (m_translation.ToVector3 (), UnityEngine.Quaternion.identity, m_scale.ToVector3 ());

				if (mesh.get_mesh () == null)
					UnityEngine.Debug.LogError ("null mesh");

				UnityEngine.Graphics.DrawMesh (mesh.get_mesh (), m, m_render_state.get_unity_material (), 0, UnityEngine.Camera.main, 0, m_render_state.get_unity_material_parameters ());
			} else {
//				m_offscreen_texture.DiscardContents ();//TODO
//				//UnityEngine.GL.Clear(true, true, UnityEngine.Color.black);
//				UnityEngine.GL.PushMatrix(); // copy current camera matrix settings
//				UnityEngine.GL.LoadOrtho(); // build ortho camera
//				m_render_state.get_unity_material().SetPass(0);
//				UnityEngine.Quaternion last_rotation = UnityEngine.Camera.main.transform.rotation;
//				UnityEngine.Vector3 last_position = UnityEngine.Camera.main.transform.position;
//				UnityEngine.Camera.main.transform.forward = UnityEngine.Vector3.down;
//				UnityEngine.Camera.main.transform.position = UnityEngine.Vector3.zero;
//				UnityEngine.Graphics.DrawMeshNow(mesh.get_mesh (),UnityEngine.Matrix4x4.TRS(UnityEngine.Camera.main.transform.position + UnityEngine.Camera.main.transform.forward*0.5f/* + new UnityEngine.Vector3(.25f,0,.25f)*/, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one /**.5f*/));
//
//				UnityEngine.Camera.main.transform.rotation = last_rotation;
//				UnityEngine.Camera.main.transform.position = last_position;
//				UnityEngine.GL.PopMatrix ();// Restore camera
				if (m_offscreen_mesh_filter == null) {
					UnityEngine.GameObject go = new UnityEngine.GameObject ("offscreen_mesh");
					go.layer = 31;
					m_offscreen_mesh_filter = go.AddComponent<UnityEngine.MeshFilter> ();
					m_offscreen_mesh_render = go.AddComponent<UnityEngine.MeshRenderer> ();
					m_offscreen_mesh_render.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//TODO
					m_offscreen_mesh_render.receiveShadows = false;
					m_offscreen_mesh_render.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
					go.transform.position = UnityEngine.Vector3.zero + m_ortho_cam.transform.forward * 0.5f;
					//go.transform.localScale = new UnityEngine.Vector3 (0.99609375f, 1f, 0.99609375f);
					go.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
				}
				m_offscreen_mesh_filter.sharedMesh = mesh.get_mesh ();
				m_offscreen_mesh_render.sharedMaterial = m_render_state.get_unity_material ();
				m_offscreen_mesh_filter.gameObject.SetActive (true);
				m_ortho_cam.Render ();
				m_offscreen_mesh_filter.gameObject.SetActive (false);
			}
		}
			
	}
}