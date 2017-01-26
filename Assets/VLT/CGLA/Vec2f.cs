using UnityEngine;
using System.Collections;
using System;

namespace CGLA
{
	public struct Vec2f {
		public const float kEpsilon = 1E-05f;
		public float x;
		public float y;
		
		public float this[int index] {
			get {
				switch (index) {
				case 0:
					return this.x;
				case 1:
					return this.y;
				default:
					throw new IndexOutOfRangeException("Invalid Vec2f index!");
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
					throw new IndexOutOfRangeException("Invalid Vec2f index!");
				}
			}
		}

		public float length()
		{
			return (float)Math.Sqrt (x * x + y * y);
		}
		
		public float magnitude() {
				return /*Mathf.Sqrt*/(this.x * this.x + this.y * this.y);
		}

		public void normalize()
		{
			float l = length ();
			x /= l;
			y /= l;
		}

		public static Vec2f normalize(Vec2f v) 
		{
			return v/v.length();
		}
		
		public static Vec2f zero {
			get {
				return new Vec2f(0.0f, 0.0f);
			}
		}
		
		public static Vec2f one {
			get {
				return new Vec2f(1f, 1f);
			}
		}
		
		public static Vec2f up {
			get {
				return new Vec2f(0.0f, 1f);
			}
		}
		
		public static Vec2f right {
			get {
				return new Vec2f(1f, 0.0f);
			}
		}
		
		public Vec2f(float x, float y) {
			this.x = x;
			this.y = y;
		}

//		public Vec2f(float x) {
//			this.x = x;
//			this.y = x;
//		}

		public Vec2d ToVec2d()
		{
			return new Vec2d ((double)x, (double)y);
		}

		public UnityEngine.Vector2 ToVector2()
		{
			return new UnityEngine.Vector2 (x, y);
		}
		
		public static implicit operator Vec2f(Vec3d v) {
			return new Vec2f((float)v.x, (float)v.y);
		}
		
		public static implicit operator Vec3d(Vec2f v) {
			return new Vec3d(v.x, v.y, 0.0d);
		}

		public static implicit operator Vec3f(Vec2f v) {
			return new Vec3f(v.x, v.y, 0.0f);
		}
		
		public static Vec2f operator +(Vec2f a, Vec2f b) {
			return new Vec2f(a.x + b.x, a.y + b.y);
		}
		
		public static Vec2f operator -(Vec2f a, Vec2f b) {
			return new Vec2f(a.x - b.x, a.y - b.y);
		}
		
		public static Vec2f operator -(Vec2f a) {
			return new Vec2f(-a.x, -a.y);
		}
		
		public static Vec2f operator *(Vec2f a, float d) {
			return new Vec2f(a.x * d, a.y * d);
		}
		
		public static Vec2f operator *(float d, Vec2f a) {
			return new Vec2f(a.x * d, a.y * d);
		}
		
		public static Vec2f operator /(Vec2f a, float d) {
			return new Vec2f(a.x / d, a.y / d);
		}
		
		public static bool operator ==(Vec2f lhs, Vec2f rhs) {
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}
		
		public static bool operator !=(Vec2f lhs, Vec2f rhs) {
			return lhs.x != rhs.x || lhs.y != rhs.y;
		}
		
		public override string ToString() {
			/*
      string fmt = "({0:D1}, {1:D1})";
      object[] objArray = new object[2];
      int index1 = 0;
      // ISSUE: variable of a boxed type
      __Boxed<float> local1 = (ValueType) this.x;
      objArray[index1] = (object) local1;
      int index2 = 1;
      // ISSUE: variable of a boxed type
      __Boxed<float> local2 = (ValueType) this.y;
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
		
		public override int GetHashCode() {
			return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
		}
		
		public override bool Equals(object other) {
			if (!(other is Vec2f))
				return false;
			Vec2f vector2d = (Vec2f)other;
			if (this.x.Equals(vector2d.x))
				return this.y.Equals(vector2d.y);
			else
				return false;
		}
	}
}