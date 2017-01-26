using System.Collections;
using System.Collections.Generic;
using System;

namespace VLT
{
	public class PreprocessorInputParser : IDisposable {

		bool m_fail;

		string m_heightmap_filename;
		string m_output_filename;

		bool m_has_texture_filename;
		string m_texture_filename;

		bool m_has_weight_filename;
		string m_weight_filename;

		float m_horizontal_spacing;
		float m_vertical_spacing;


		public PreprocessorInputParser(int argc, string[] argv)
		{
			m_has_texture_filename = false;
			m_fail = false;
			m_has_weight_filename = false;
			m_horizontal_spacing = 1.0f;
			m_vertical_spacing = 1.0f;

			parse(argc, argv);
		}
		public void Dispose(){}

		public bool fail()
		{
			return m_fail;
		}

		public bool has_texture_filename()
		{
			return m_has_texture_filename;
		}
		public string get_texture_filename()
		{
			return m_texture_filename;
		}

		public bool has_weigth_filename()
		{
			return m_has_weight_filename;
		}
		public string get_weight_filename()
		{
			return m_weight_filename;
		}

		public string get_heightmap_filename()
		{
			return m_heightmap_filename;
		}
		public string get_output_filename()
		{
			return m_output_filename;
		}

		public float get_horizontal_spacing()
		{
			return m_horizontal_spacing;
		}
		public float get_vertical_spacing()
		{
			return m_vertical_spacing;
		}

		public void parse(int argc, string[] argv)
		{
			if (argc > 1)
			{
				m_heightmap_filename = argv[1];
				m_output_filename = m_heightmap_filename.Substring(0, m_heightmap_filename.IndexOf(".")) + ".map";

				int argument = 2;
				int remaining_arguments = argc - 2;
				while (remaining_arguments > 0)
				{
					if (argv[argument][0] == '-')
					{
						char option = argv[argument++][1];
						remaining_arguments--;
						if (remaining_arguments == 0)
						{
							return;
						}

						switch (option)
						{	
						case 't':
							m_texture_filename = argv[argument++];
							m_has_texture_filename = true;
							break;

						case 'w':
							m_weight_filename = argv[argument++];
							m_has_weight_filename = true;
							break;

						case 'h':
							m_horizontal_spacing = (float)(float.Parse(argv[argument++]));
							break;

						case 'v':
							m_vertical_spacing = (float)(float.Parse(argv[argument++]));
							break;

						case 'o':
							m_output_filename = argv[argument++];
							break;

						default:
							UnityEngine.Debug.LogError("Unknown option: " + option);
							usage();
							m_fail = true; 
							return;
						}
						remaining_arguments--;
					}
					else
					{
						usage();
						m_fail = true;

						return;
					}
				}
			}
			else
			{
				usage();
				m_fail = true;
			}
		}
		public static void usage()
		{
			UnityEngine.Debug.Log("Usage: prep <heightmap> [-t texture] [-h horizontal_spacing] [-v vertical_spacing] [-o output]");
		}
	}
}