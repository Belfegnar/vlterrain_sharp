using System.Collections;
using System.Collections.Generic;
using System;

namespace VLT
{
	[System.Serializable]
	public class TerrainMesh : IDisposable {

		[UnityEngine.SerializeField]UnityEngine.Mesh m_mesh;
		[UnityEngine.SerializeField]VertexArray m_vertex_array;
		[UnityEngine.SerializeField]Texture m_static_texture;
		[UnityEngine.SerializeField]Texture m_dynamic_texture;

		public TerrainMesh(VertexArray vertex_array, Texture static_texture, Texture dynamic_texture)
		{
			m_vertex_array = vertex_array;
			m_static_texture = static_texture as UnityTexture;
			m_dynamic_texture = dynamic_texture as UnityTexture;

			m_static_texture.add_reference();
			m_dynamic_texture.add_reference();
		}
		public void Dispose()
		{
			m_static_texture.remove_reference();
			m_dynamic_texture.remove_reference();
			m_static_texture = null;
			m_dynamic_texture = null;
			m_vertex_array.Dispose();
			UnityEngine.GameObject.DestroyImmediate (m_mesh);
		}

		public void bind()
		{
			if (m_mesh == null) {
				m_mesh = new UnityEngine.Mesh ();
				m_mesh.name = "terrain mesh";
//			}
//			if (m_mesh != null && m_mesh.vertexCount == 0) {
				m_vertex_array.bind (m_mesh);
				m_mesh.bounds = new UnityEngine.Bounds (UnityEngine.Vector3.zero, UnityEngine.Vector3.one * float.MaxValue);
//				m_mesh.RecalculateBounds ();
				m_mesh.UploadMeshData (false);
			}
		}
		public void bind(MemoryBlock light)
		{
			bind ();
			light.bind(m_mesh);
			m_mesh.UploadMeshData (false);
		}
			
		public int get_index_count()
		{
			return m_vertex_array.get_index_count();
		}

		public UnityEngine.Mesh get_mesh()
		{
			return m_mesh;
		}

		public Texture get_static_texture()
		{
			return m_static_texture;
		}

		public Texture get_dynamic_texture()
		{
			return m_dynamic_texture;
		}
	}
}