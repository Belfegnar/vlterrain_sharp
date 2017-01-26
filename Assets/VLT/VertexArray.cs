using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class VertexArray : IDisposable {

		HeightMapMesher m_mesher;

		MemoryBlock m_index_block;
		MemoryBlock m_vertex_block;
		MemoryBlock m_displacement_block;

		int m_index_count;

		public VertexArray(HeightMapMesher mesher, MemoryBlock index_block, int index_count, MemoryBlock vertex_block, MemoryBlock displacement_block)
		{
			m_mesher = mesher;
			m_index_block = index_block;
			m_index_count = index_count;
			m_vertex_block = vertex_block;
			m_displacement_block = displacement_block;
		}

		public virtual void Dispose()
		{
			m_mesher.destroy_vertex_array(this);
		}

		public void bind(UnityEngine.Mesh mesh)
		{
			m_vertex_block.bind(mesh);
			m_displacement_block.bind(mesh);
			m_index_block.bind(mesh);
		}

		public MemoryBlock get_displacement_block()
		{
			return m_displacement_block;
		}

		public int get_index_count()
		{
			return m_index_count;
		}
	}
}