using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace VLT
{
	[System.Serializable]
	public class QuadTreeHeader : IDisposable {

		static int m_current_version = 2;
		static string m_current_magic = "QUADTREEMAP";

		[UnityEngine.SerializeField]bool m_ok;

		[UnityEngine.SerializeField]int m_directory_position;

		public QuadTreeHeader(int directory_position)
		{
			m_ok = true;
			m_directory_position = directory_position;
		}

		public QuadTreeHeader(BinaryReader stream)
		{
			int header_size = sizeof(/*m_current_version*/int) + m_current_magic.Length+1 + sizeof(/*m_directory_position*/int);

			byte[] magic = new byte[m_current_magic.Length+1];
			int version;

			stream.BaseStream.Position = stream.BaseStream.Length - header_size;
			stream.Read (magic, 0, magic.Length);

			version = stream.ReadInt32 ();

			m_ok = System.Text.Encoding.ASCII.GetString(magic).Contains(m_current_magic) &&	version == m_current_version;

			m_directory_position = stream.ReadInt32 ();
			UnityEngine.Debug.Log ("magic = " + System.Text.Encoding.ASCII.GetString (magic).ToString ().Substring(0, 11) + ", ver = " + version + ", ok = " + m_ok + ", pos = " + m_directory_position);
		}

		public virtual void Dispose()
		{

		}

		public void serialize(BinaryWriter stream)
		{
			stream.Write (m_current_magic);
			stream.Write (m_current_version);
			stream.Write (m_directory_position);
		}

		public bool is_ok()
		{
			return m_ok;
		}

		public int get_directory_position()
		{
			return m_directory_position;
		}
	}
}