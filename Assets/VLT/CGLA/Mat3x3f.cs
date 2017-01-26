using UnityEngine;
using System.Collections;
using System;

namespace CGLA
{
	public struct Mat3x3f {

		public float m00;
		public float m01;
		public float m02;

		public float m10;
		public float m11;
		public float m12;

		public float m20;
		public float m21;
		public float m22;
		
		public static Mat3x3f New()
		{
			Mat3x3f m;
			m.m00 = 0;
			m.m01 = 0;
			m.m02 = 0;
			m.m10 = 0;
			m.m11 = 0;
			m.m12 = 0;
			m.m20 = 0;
			m.m21 = 0;
			m.m22 = 0;
			return m;
		}
			
		public Mat3x3f(Vec3f a1, Vec3f a2, Vec3f a3)
		{
			m00 = a1.x;
			m01 = a1.y;
			m02 = a1.z;
			m10 = a2.x;
			m11 = a2.y;
			m12 = a2.z;
			m20 = a3.x;
			m21 = a3.y;
			m22 = a3.z;
		}

		public float this[int row, int column]
		{
			get
			{
				return this[row * 3 + column];
			}
			set
			{
				this[row * 3 + column] = value;
			}
		}

		public float this[int index]
		{
			get
			{
				float result;
				switch (index)
				{
				case 0:
					result = this.m00;
					break;
				case 1:
					result = this.m01;
					break;
				case 2:
					result = this.m02;
					break;
				case 3:
					result = this.m10;
					break;
				case 4:
					result = this.m11;
					break;
				case 5:
					result = this.m12;
					break;
				case 6:
					result = this.m20;
					break;
				case 7:
					result = this.m21;
					break;
				case 8:
					result = this.m22;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
				return result;
			}
			set
			{
				switch (index)
				{
				case 0:
					this.m00 = value;
					break;
				case 1:
					this.m01 = value;
					break;
				case 2:
					this.m02 = value;
					break;
				case 3:
					this.m10 = value;
					break;
				case 4:
					this.m11 = value;
					break;
				case 5:
					this.m12 = value;
					break;
				case 6:
					this.m20 = value;
					break;
				case 7:
					this.m21 = value;
					break;
				case 8:
					this.m22 = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid matrix index!");
				}
			}
		}
		
		//MATRIX MULTIPLICATION
		public static Mat3x3f operator *(Mat3x3f m1, Mat3x3f m2){
			Mat3x3f n;
			n.m00 = m1.m00 * m2.m00;
			n.m00 += m1.m01 * m2.m10;
			n.m00 += m1.m02 * m2.m20;

			n.m01 = m1.m00 * m2.m01;
			n.m01 += m1.m01 * m2.m11;
			n.m01 += m1.m02 * m2.m21;

			n.m02 = m1.m00 * m2.m02;
			n.m02 += m1.m01 * m2.m12;
			n.m02 += m1.m02 * m2.m22;


			n.m10 = m1.m10 * m2.m00;
			n.m10 += m1.m11 * m2.m10;
			n.m10 += m1.m12 * m2.m20;

			n.m11 = m1.m10 * m2.m01;
			n.m11 += m1.m11 * m2.m11;
			n.m11 += m1.m12 * m2.m21;

			n.m12 = m1.m10 * m2.m02;
			n.m12 += m1.m11 * m2.m12;
			n.m12 += m1.m12 * m2.m22;


			n.m20 = m1.m20 * m2.m00;
			n.m20 += m1.m21 * m2.m10;
			n.m20 += m1.m22 * m2.m20;

			n.m21 = m1.m20 * m2.m01;
			n.m21 += m1.m21 * m2.m11;
			n.m21 += m1.m22 * m2.m21;

			n.m22 = m1.m20 * m2.m02;
			n.m22 += m1.m21 * m2.m12;
			n.m22 += m1.m22 * m2.m22;

			return n;
		}
		
		//float MULTIPLICATION
		public static Mat3x3f operator *(Mat3x3f m, float f)
		{
			m.m00 *= f;
			m.m01 *= f;
			m.m02 *= f;
			m.m10 *= f;
			m.m11 *= f;
			m.m12 *= f;
			m.m20 *= f;
			m.m21 *= f;
			m.m22 *= f;
			return m;
		}

		public static Mat3x3f operator *(float f, Mat3x3f m)
		{
			m.m00 *= f;
			m.m01 *= f;
			m.m02 *= f;
			m.m10 *= f;
			m.m11 *= f;
			m.m12 *= f;
			m.m20 *= f;
			m.m21 *= f;
			m.m22 *= f;
			return m;
		}

		//Vector MULTIPLICATION
		public static Vec3f operator *(Mat3x3f m, Vec3f v)
		{
			Vec3f v2;
			v2.x = m.m00 * v.x + m.m01 * v.y + m.m02 * v.z;
			v2.y = m.m10 * v.x + m.m11 * v.y + m.m12 * v.z;
			v2.z = m.m20 * v.x + m.m21 * v.y + m.m22 * v.z;
			return v2;
		}
		
		//ToSTRING OVERRIDE
		public override string ToString()
		{
			string str = "[";
//			for(int i = 0; i < 3; i++){
//				for(int j = 0; j < 3; j++){
//					str += matrix[j][i].ToString();
//					str += i*j == 4 ? "]" : ", ";
//				}
//			}
			return str;
		}
		
		public Vec3f MultiplyVector(Vec3f vec){
			Vec3f v2;
			v2.x = m00 * vec.x + m01 * vec.y + m02 * vec.z;
			v2.y = m10 * vec.x + m11 * vec.y + m12 * vec.z;
			v2.z = m20 * vec.x + m21 * vec.y + m22 * vec.z;
			return v2;
		}

		public static Mat3x3f Rotation_Mat3x3f(Axis axis, float angle)
		{
			Mat3x3f m = Mat3x3f.New();

			switch(axis)
			{
			case Axis.XAXIS:
				m.m00 = 1.0f;
				m.m11 = Mathf.Cos(angle);
				m.m12 = Mathf.Sin(angle);
				m.m21 = -Mathf.Sin(angle);
				m.m22 = Mathf.Cos(angle);
				break;
			case Axis.YAXIS:
				m.m00 = Mathf.Cos(angle);
				m.m02 = -Mathf.Sin(angle);
				m.m20 = Mathf.Sin(angle);
				m.m22 = Mathf.Cos(angle);
				m.m11 = 1.0f;
				break;
			case Axis.ZAXIS:
				m.m00 = Mathf.Cos(angle);
				m.m01 = Mathf.Sin(angle);
				m.m10 = -Mathf.Sin(angle);
				m.m11 = Mathf.Cos(angle);
				m.m22 = 1.0f;
				break;
			}

			return m;
		}
		
		public Mat3x3f Transpose(){
			Mat3x3f newM;

			newM.m00 = m00;
			newM.m01 = m10;
			newM.m02 = m20;

			newM.m10 = m01;
			newM.m11 = m11;
			newM.m12 = m21;

			newM.m20 = m02;
			newM.m21 = m12;
			newM.m22 = m22;

			return newM;
		}

	}
}