using UnityEngine;
using System.Collections;
using System;

namespace CGLA
{
	public struct Vec2i {

		public int x;
		public int y;
		
		public int this[int index] {
			get {
				switch (index) {
				case 0:
					return this.x;
				case 1:
					return this.y;
				default:
					throw new IndexOutOfRangeException("Invalid Vec2i index!");
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
					throw new IndexOutOfRangeException("Invalid Vec2i index!");
				}
			}
		}
		
		public static Vec2i zero {
			get {
				return new Vec2i(0, 0);
			}
		}
		
		public static Vec2i one {
			get {
				return new Vec2i(1, 1);
			}
		}
		
		public static Vec2i up {
			get {
				return new Vec2i(0, 1);
			}
		}
		
		public static Vec2i right {
			get {
				return new Vec2i(1, 0);
			}
		}
		
		public Vec2i(int x, int y) {
			this.x = x;
			this.y = y;
		}
		
		public static Vec2i operator +(Vec2i a, Vec2i b) {
			return new Vec2i(a.x + b.x, a.y + b.y);
		}
		
		public static Vec2i operator -(Vec2i a, Vec2i b) {
			return new Vec2i(a.x - b.x, a.y - b.y);
		}
		
		public static Vec2i operator -(Vec2i a) {
			return new Vec2i(-a.x, -a.y);
		}
		
		public static Vec2i operator *(Vec2i a, int d) {
			return new Vec2i(a.x * d, a.y * d);
		}
		
		public static Vec2i operator *(int d, Vec2i a) {
			return new Vec2i(a.x * d, a.y * d);
		}
		
		public static Vec2i operator /(Vec2i a, int d) {
			return new Vec2i(a.x / d, a.y / d);
		}
		
		public static bool operator ==(Vec2i a, Vec2i b) {
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Vec2i a, Vec2i b) {
			return a.x != b.x || a.y != b.y;
		}
		
		public override string ToString() {
			return "[" + x + ", " + y + "]";
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
			return x ^ y << 2;
		}
		
		public override bool Equals(object other) {
			if (!(other is Vec2i))
				return false;
			Vec2i vector2d = (Vec2i)other;

			return x == vector2d.x && y == vector2d.y;
		}

		public bool Equals(Vec2i p)
		{
			return (x == p.x) && (y == p.y);
		}
	}
}