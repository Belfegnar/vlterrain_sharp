using System.Collections;
using System.Collections.Generic;
using System;

namespace CGLA
{
	public class MathExt {

		public const float M_PI = 3.14159265358979323846f;
		public const float M_PI_2 = M_PI * 2;
		public static float clamp(float value, float min_value, float max_value)
		{
			return value < min_value ? min_value : (value > max_value ? max_value : value);
		}
		public static int clamp(int value, int min_value, int max_value)
		{
			return value < min_value ? min_value : (value > max_value ? max_value : value);
		}
			
		public static float cubic_interpolate(float v0, float v1, float v2, float v3, float t)
		{
			float p = v3 - v2 - v0 + v1;
			float q = v0 - v1 - p;
			float r = v2 - v0;

			return (((p * t) + q) * t + r) * t + v1;
		}

		
		public static float cos_interpolate(float v0, float v1, float t)
		{
			float f = (1.0f - (float)Math.Cos(t * M_PI)) * 0.5f;

			return v0 * (1.0f - f) + v1 * f;
		}

		
		public static float lerp(float start_value, float end_value, float t)
		{
			return start_value + (float)((end_value - start_value) * t);
		}

		public static uint lerp(uint start_value, uint end_value, float t)
		{
			return (uint)(start_value + (float)((end_value - start_value) * t));
		}

		
		public static float bi_lerp(float u, float v, float q, float r, float s, float t)
		{
			return lerp(lerp(u, v, s), lerp(q, r, s), t);
		}

		public static UnityEngine.Vector4 lerp(UnityEngine.Vector4 start_value, UnityEngine.Vector4 end_value, float t)
		{
			return start_value + (UnityEngine.Vector4)((end_value - start_value) * t);
		}


		public static UnityEngine.Vector4 bi_lerp(UnityEngine.Vector4 u, UnityEngine.Vector4 v, UnityEngine.Vector4 q, UnityEngine.Vector4 r, float s, float t)
		{
			return lerp(lerp(u, v, s), lerp(q, r, s), t);
		}

		public static float bias(float value, float b)
		{
			return (float)Math.Pow(value, Math.Log(b) / Math.Log(0.5f));
		}

		public static float gain(float value, float g)
		{
			if (value < 0.5f)
			{
				return bias(1.0f - g, 2.0f * value) * 0.5f;
			}

			return 1.0f - bias(1.0f - g, 2.0f - 2.0f * value) * 0.5f;
		}
			
		public static float modulo(float value, float modus)
		{
			value -= modus * (int)(value / modus);

			if (value < 0)
			{
				value += modus;
			}

			return value;
		}

		public static double modulo(double value, double modus)
		{
			value -= modus * (int)(value / modus);

			if (value < 0)
			{
				value += modus;
			}

			return value;
		}

		public static int modulo(int value, int modus)
		{
			value -= modus * (int)(value / modus);

			if (value < 0)
			{
				value += modus;
			}

			return value;
		}

		public static float deg_to_rad(float deg)
		{
			return deg * M_PI / 180.0f;
		}

//		public static float random(float maximum)
//		{
//			return rand() * (maximum / RAND_MAX);
//		}
	}
}