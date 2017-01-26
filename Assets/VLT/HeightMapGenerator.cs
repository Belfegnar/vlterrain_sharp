using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;
using System.IO;

namespace VLT
{
	[Serializable]
	public abstract class HeightMapGenerator : IDisposable
	{
		public HeightMapGenerator(){}
		public abstract HeightMap create_map();

		static void initialize_border(HeightMap heightmap)
		{
			int size = heightmap.get_size();

			for (int x = 0; x < size; x++)
			{
				heightmap.set_height(heightmap.get_height(x, 0), x, -1);
				heightmap.set_height(heightmap.get_height(x, size - 1), x, size);
			}

			for (int y = -1; y <= size; y++)
			{
				heightmap.set_height(heightmap.get_height(0, y), -1, y);
				heightmap.set_height(heightmap.get_height(size - 1, y), size, y);
			}
		}

		public abstract void Dispose();

		public static HeightMap create_heightmap(string filename, Vec2f scale, int size = 129)
		{
			HeightMapGenerator generator = null;

			int dot_pos = filename.IndexOf (".");
			if (dot_pos >= 0)
			{
				string extension = filename.Substring(dot_pos + 1, filename.Length - dot_pos - 1);

				if (extension == "ter")
				{
					generator = new TerragenHeightMapGenerator(filename);
				}
//				else if (extension == "bt")
//				{
//					generator = new BinTerrainHeightMapGenerator(filename);
//				}
//				else if (extension == "png")
//				{
//					generator = new PNGHeightMapGenerator(filename, scale);
//				}
//				else
//				{
//					generator = new TestHeightMapGenerator(size, scale[0]);
//				}
			}

			HeightMap heightmap = generator.create_map();
			generator.Dispose();

			initialize_border(heightmap);

			return heightmap;
		}
	}
}

