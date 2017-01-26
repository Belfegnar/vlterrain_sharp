using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.InteropServices;

namespace VLT
{
	[System.Serializable]
	public class StreamQuadTreeReader : QuadTreeReader 
	{

		[UnityEngine.SerializeField]int[] m_directory;

		byte[] _bytes;

		BinaryReader m_stream;

		[UnityEngine.SerializeField]QuadTreeHeader m_header;

		void read_directory(int directory_position)
		{
			m_stream.BaseStream.Position = directory_position;

			uint directory_size = m_stream.ReadUInt32();

			m_directory = new int[directory_size];
			for (int i = 0; i < directory_size; i++)
			{
				m_directory[i] = m_stream.ReadInt32 ();
			}
		
		}

		public StreamQuadTreeReader(string filename)
		{
			m_stream = new BinaryReader (File.Open (filename, FileMode.Open));

			m_header = new QuadTreeHeader(m_stream);

			read_directory(m_header.get_directory_position());
		}

		public override void Dispose()
		{
			m_header.Dispose();

			m_stream.Close();m_stream = null;

			m_directory = null;
		}

		public override QuadInfo create_info(int id)
		{
			m_stream.BaseStream.Position = m_directory[id];

			return new QuadInfo(id, m_stream);
		}
	}
}