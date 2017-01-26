using UnityEngine;
using System.Collections;
using System;

namespace CGLA
{
	public struct Vec2d {
		public const double kEpsilon = 1E-05d;
		public double x;
		public double y;
		
		public double this[int index] {
			get {
				switch (index) {
				case 0:
					return this.x;
				case 1:
					return this.y;
				default:
					throw new IndexOutOfRangeException("Invalid Vec2d index!");
				}
			}
			set {
				switch (index) {
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Vec2d index!");
				}
			}
		}
		
		public double length()
		{
			return Math.Sqrt (x * x + y * y);
		}

		/// Normalize vector.
		public void normalize() 
		{
			double l = length();
			x /= l;
			y /= l;
		}

		public static Vec2d normalize(Vec2d v) 
		{
			return v/v.length();
		}
		
		public double magnitude {
			get {
				return /*Math.Sqrt*/(this.x * this.x + this.y * this.y);
			}
		}
		
		public Vec2d(double x, double y) {
			this.x = x;
			this.y = y;
		}

		public Vec2f ToVec2f()
		{
			Vec2f v;
			v.x = (float)x;
			v.y = (float)y;
			return v;
		}

		public Vec3f ToVec3f()
		{
			Vec3f v;
			v.x = (float)x;
			v.y = (float)y;
			v.z = 0;
			return v;
		}
		
		public static implicit operator Vec2d(Vec3d v) {
			Vec2d vv;
			vv.x = v.x;
			vv.y = v.y;
			return vv;
		}
		
		public static implicit operator Vec3d(Vec2d v) {
			Vec3d vv;
			vv.x = v.x;
			vv.y = v.y;
			vv.z = 0.0d;
			return vv;
		}
		
		public static Vec2d operator +(Vec2d a, Vec2d b) {
			return new Vec2d(a.x + b.x, a.y + b.y);
		}
		
		public static Vec2d operator -(Vec2d a, Vec2d b) {
			return new Vec2d(a.x - b.x, a.y - b.y);
		}
		
		public static Vec2d operator -(Vec2d a) {
			return new Vec2d(-a.x, -a.y);
		}
		
		public static Vec2d operator *(Vec2d a, double d) {
			return new Vec2d(a.x * d, a.y * d);
		}
		
		public static Vec2d operator *(float d, Vec2d a) {
			return new Vec2d(a.x * d, a.y * d);
		}
		
		public static Vec2d operator /(Vec2d a, double d) {
			return new Vec2d(a.x / d, a.y / d);
		}
		
		public static bool operator ==(Vec2d lhs, Vec2d rhs) {
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}
		
		public static bool operator !=(Vec2d lhs, Vec2d rhs) {
			return lhs.x != rhs.x || lhs.y != rhs.y;
		}

		public override string ToString() {
			/*
      string fmt = "({0:D1}, {1:D1})";
      object[] objArray = new object[2];
      int index1 = 0;
      // ISSUE: variable of a boxed type
      __Boxed<double> local1 = (ValueType) this.x;
      objArray[index1] = (object) local1;
      int index2 = 1;
      // ISSUE: variable of a boxed type
      __Boxed<double> local2 = (ValueType) this.y;
      objArray[index2] = (object) local2;
      */
			return "not implemented";
		}
		
		public string ToString(string format) {
			/* TODO:
      string fmt = "({0}, {1})";
      object[] objArray = new object[2];
      int index1 = 0;
      string str1 = this.x.ToString(format);
      objArray[index1] = (object) str1;
      int index2 = 1;
      string str2 = this.y.ToString(format);
      objArray[index2] = (object) str2;
      */
			return "not implemented";
		}
		
		public override bool Equals(object other) {
			if (!(other is Vec2d))
				return false;
			Vec2d vector2d = (Vec2d)other;
			if (this.x.Equals(vector2d.x))
				return this.y.Equals(vector2d.y);
			else
				return false;
		}
		
		public static Vec2d Min(Vec2d lhs, Vec2d rhs) {
			return new Vec2d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y));
		}
		
		public static Vec2d Max(Vec2d lhs, Vec2d rhs) {
			return new Vec2d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y));
		}
	}
}