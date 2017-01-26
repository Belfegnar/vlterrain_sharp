using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;
using System.IO;

namespace VLT
{
	[Serializable]
	public class TerragenHeightMapGenerator : HeightMapGenerator
	{
		BinaryReader m_input;

		int m_size;
		float m_scale;

		HeightMap m_heightmap;

		void parse()
		{
			if (read_magic())
			{
				read_segments();
			}
		}

		bool read_magic()
		{
			byte[] buffer = new byte[16];

			buffer = m_input.ReadBytes(16);

			return string.Compare(System.Text.Encoding.ASCII.GetString(buffer), "TERRAGENTERRAIN ") == 0;
		}

		void read_segments()
		{
			string marker;
			do
			{
				marker = read_marker();

				if (marker == "SIZE")
				{
					read_size();
				}
				else if (marker == "XPTS")
				{
					read_xpts();
				}
				else if (marker == "YPTS")
				{
					read_ypts();
				}
				else if (marker == "SCAL")
				{
					read_scal();
				}
				else if (marker == "CRAD")
				{
					read_crad();
				}
				else if (marker == "CRVM")
				{
					read_crvm();
				}
				else if (marker == "ALTW")
				{
					read_altw();
				}
			} while (marker != "ALTW");
		}

		string read_marker()
		{
			byte[] buffer = new byte[5];

			buffer = m_input.ReadBytes(4);
			buffer[4] = (byte)'\0';

			return System.Text.Encoding.ASCII.GetString(buffer);
		}

		void read_size()
		{
			m_size = read_short(true) + 1;
		}

		void read_xpts()
		{
			int x_points = read_short(true);
		}

		void read_ypts()
		{
			int y_points = read_short(true);
		}

		void read_scal()
		{
			m_scale = m_input.ReadSingle();
			m_input.BaseStream.Position += 8;
		}

		void read_crad()
		{
			m_input.BaseStream.Position += 4;
		}

		void read_crvm()
		{
			int crvm = m_input.ReadInt32();
		}

		void read_altw()
		{
			ushort height_scale = read_short(false);
			ushort base_height = read_short(false);

			m_heightmap = new HeightMap(m_size, m_scale, new Vec2d(0.0f, 0.0f));

			for (int y = 0; y < m_size; y++)
			{
				for (int x = 0; x < m_size; x++)
				{
					ushort elevation = read_short(false);

					float height = base_height + (float)(elevation * height_scale) / 65536.0f;

					m_heightmap.set_height(height * m_scale, x, y);
				}
			}
		}

		ushort read_short(bool skip_padding)
		{
			ushort s = m_input.ReadUInt16();

			if (skip_padding)
			{
				m_input.BaseStream.Position += 2;
			}

			return s;
		}

		public TerragenHeightMapGenerator(string filename)
		{
			m_input = new BinaryReader (File.Open (filename, FileMode.Open));
			m_size = 0;
			m_scale = 1.0f;
			m_heightmap = null;
		}

		public override void Dispose()
		{
			m_input.Close();
		}

		public override HeightMap create_map()
		{
			m_heightmap = null;

			parse();

			return m_heightmap;
		}
	}
}

