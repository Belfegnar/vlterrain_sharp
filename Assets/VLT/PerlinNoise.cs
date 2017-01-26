using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class PerlinNoise {

		static int[] m_permutation = new int[]{
			151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 
			233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 
			10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 
			252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 
			56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 
			134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 
			60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 
			102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132,
			187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 
			86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 
			124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 
			206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 
			170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 
			155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 
			79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 
			251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 
			81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 
			199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 
			150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 
			141, 128, 195, 78, 66, 215, 61, 156, 180,
			151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 
			233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 
			10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 
			252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 
			56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 
			134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 
			60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 
			102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132,
			187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 
			86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 
			124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 
			206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 
			170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 
			155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 
			79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 
			251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 
			81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 
			199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 
			150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 
			141, 128, 195, 78, 66, 215, 61, 156, 180};

		static float fade(float t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}
		static float grad(int hash, float x, float y, float z)
		{
			hash = hash & 15;

			float u = (hash < 8 || hash == 12 || hash == 13) ? x : y;
			float v = (hash < 4 || hash == 12 || hash == 13) ? y : z;

			return ((hash & 1) == 0 ? u : -u) + ((hash & 2) == 0 ? v : -v);
		}

		public static float noise(Vec3f point)
		{
			Vec3i unit_cube;
			unit_cube.x = (int)(Math.Floor (point.x)) & 255; 
			unit_cube.y = (int)(Math.Floor (point.y)) & 255;
			unit_cube.z = (int)(Math.Floor (point.z)) & 255;

			point -= Vec3f.floor(point);

			float u = fade(point[0]);
			float v = fade(point[1]);
			float w = fade(point[2]);

			int A = m_permutation[unit_cube[0]] + unit_cube[1];
			int AA = m_permutation[A] + unit_cube[2];
			int AB = m_permutation[A + 1] + unit_cube[2];
			int B = m_permutation[unit_cube[0] + 1] + unit_cube[1];
			int BA = m_permutation[B] + unit_cube[2];
			int BB = m_permutation[B + 1] + unit_cube[2];

			return MathExt.lerp(MathExt.lerp(MathExt.lerp(grad(m_permutation[AA], point[0], point[1], point[2]),
				grad(m_permutation[BA], point[0] - 1, point[1], point[2]), u),
				MathExt.lerp(grad(m_permutation[AB], point[0], point[1] - 1, point[2]),
					grad(m_permutation[BB], point[0] - 1, point[1] - 1, point[2]), u), v),
				MathExt.lerp(MathExt.lerp(grad(m_permutation[AA + 1], point[0], point[1], point[2] - 1),
					grad(m_permutation[BA + 1], point[0] - 1, point[1], point[2] - 1), u),
					MathExt.lerp(grad(m_permutation[AB + 1], point[0], point[1] - 1, point[2] - 1),
						grad(m_permutation[BB + 1], point[0] - 1, point[1] - 1, point[2] - 1), u), v), w);
		}

		static Vec3f turb_const = new Vec3f(12.345f, 6.789f, 0.123f);
		public static float turbulence(Vec3f point, float low_freq, float high_freq)
		{
			Vec3f p = point + turb_const;

			float turb = 0.0f;
			for (float freq = low_freq; freq < high_freq;  freq *= 2.0f)
			{
				turb += Math.Abs(noise(freq * p)) / freq;
			}

			return turb;
		}
	}
}