using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using CGLA;

namespace VLT
{
	public class Preprocessor : MonoBehaviour {

		int subdivision_size = 64;
		int sub_image_size = 32;
		bool compress_images = true;

		int Preprocess(int argc, string[] argv)
		{
//			SetPriorityClass(GetCurrentProcess(), BELOW_NORMAL_PRIORITY_CLASS);

			PreprocessorInputParser input = new PreprocessorInputParser(argc, argv);

			if (input.fail())
			{
				return 1;
			}

			Debug.Log("Processing: " + input.get_heightmap_filename() + " -> "
				+ input.get_output_filename() + "...");

			Debug.Log("Reading input file...");
			HeightMap heightmap = HeightMapGenerator.create_heightmap(input.get_heightmap_filename(), 
				new Vec2f(input.get_horizontal_spacing(), input.get_vertical_spacing()));

//			Image image = null;
//			if (input.has_texture_filename())
//			{
//				Debug.Log("Reading texture...");
//				image = Image.create_image(input.get_texture_filename());
//			}
//			else
//			{
//				Debug.Log("Generating texture...");
//				TextureGenerator texgen;
//				image = texgen.create_image(heightmap);
//			}
//
//			Image weight_image = null;
//			if (input.has_weigth_filename())
//			{
//				Debug.Log("Reading weight file...");
//				weight_image = Image.create_image(input.get_weight_filename());
//			}
//
//			WeightMap weightmap = new WeitMap(weight_image, heightmap.get_size(), subdivision_size);
//
//			ImageCompressor compressor;
//			QuadTreeSaver saver = new QuadTreeSaver(input.get_output_filename(), compressor, compress_images);
//
//			Debug.Log("Generating Heightmap...");
//			QuadTreeGenerator generator = new QuadTreeGenerator(heightmap, &weightmap, image, &saver, subdivision_size, sub_image_size);
//
//			weight_image.Dispose();
//			image.Dispose();
//			heightmap.Dispose();
//
			return 0;
		}
	}
}