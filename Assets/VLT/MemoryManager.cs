using System.Collections;
using System.Collections.Generic;
using CGLA;
using System;

namespace VLT
{
	public class MemoryManager : IDisposable {

		MemoryBlock m_index_block;
		MemoryBlock m_vertex_block;
		MemoryBlock m_light_block;

		List<MemoryBlock> m_all_displacement_blocks;
		List<MemoryBlock> m_free_displacement_blocks = new List<MemoryBlock>();

		int m_index_size;
		int m_vertex_size;
		int m_light_size;
		int m_displacement_size;
		int m_displacement_count;

		MemoryManager()
		{
			m_index_block = null;
			m_vertex_block = null;
			m_light_block = null;
			m_index_size = 0;
			m_vertex_size = 0;
			m_light_size = 0;
			m_displacement_size = 0;
			m_displacement_count = 0;
		}

		int get_index_size()
		{
			return m_index_size;
		}

		int get_vertex_size()
		{
			return m_vertex_size;
		}

		int get_light_size()
		{
			return m_light_size;
		}

		int get_displacement_size()
		{
			return m_displacement_size;
		}

		int get_displacement_count()
		{
			return m_displacement_count;
		}

		protected virtual MemoryBlock create_index_block()
		{
			return new MemoryBlock(MemoryBlock.BlockType.ElementBlock);
		}
		
		protected virtual MemoryBlock create_vertex_block()
		{
			return new MemoryBlock(MemoryBlock.BlockType.VertexBlock);
		}

		protected virtual MemoryBlock create_light_block()
		{
			return new MemoryBlock(MemoryBlock.BlockType.LightBlock);
		}
		
		protected virtual MemoryBlock create_displacement_block(int number)
		{
			return new MemoryBlock(MemoryBlock.BlockType.DisplacementBlock);
		}

		public virtual void Dispose()
		{
			m_index_block = null;
			m_vertex_block = null;
			m_light_block = null;

			for (int b = 0; b < m_all_displacement_blocks.Count; b++)
			{
				m_all_displacement_blocks[b] = null;
			}
			m_all_displacement_blocks.Clear ();
		}

		public static MemoryManager create_manager()
		{
//			string memory = Configuration.get_string("memory");
			return new MemoryManager ();
		}

		public void initialize_blocks(int index_size, int vertex_size, int light_size, 
			int displacement_size, int displacement_count)
		{
			int size = vertex_size + light_size + displacement_size * displacement_count;
			UnityEngine.Debug.Log("Allocating " + ((size + index_size) / (1024 * 1024)).ToString() + " mb of memory for geometry");

			m_index_size = index_size;
			m_vertex_size = vertex_size;
			m_light_size = light_size;
			m_displacement_size = displacement_size;
			m_displacement_count = displacement_count;

			m_index_block = create_index_block();
			m_vertex_block = create_vertex_block();
			m_light_block = create_light_block();

			m_all_displacement_blocks = new List<MemoryBlock>(displacement_count);
			for (int b = 0; b < displacement_count; b++)
			{
				MemoryBlock block = create_displacement_block(b);

				m_all_displacement_blocks.Add(block);
				m_free_displacement_blocks.Add(block);
			}
		}
		public virtual void free_blocks(){}

		public MemoryBlock get_index_block()
		{
			return m_index_block;
		}

		public MemoryBlock get_vertex_block()
		{
			return m_vertex_block;
		}

		public MemoryBlock get_light_block()
		{
			return m_light_block;
		}

		public MemoryBlock get_displacement_block()
		{
			MemoryBlock block = m_free_displacement_blocks[0];
			m_free_displacement_blocks.RemoveAt(0);
			return block;
		}

		public void release_displacement_block(MemoryBlock block)
		{
			m_free_displacement_blocks.Add(block);
		}
	}
}