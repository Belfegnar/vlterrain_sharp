using System;
namespace CGLA {

    /** 3D int vector.
    Class Vec3i is the vector typically used in 3D computer graphics. 
    The class has many constructors since we may need to convert from
    other vector types. Most of these are explicit to avoid automatic
    conversion. 
    */
    public struct Vec3i
    {
        public int x;
		public int y;
		public int z;
		public int this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				default:
                    return 0;
					//throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					break;
				case 1:
					this.y = value;
					break;
				case 2:
					this.z = value;
					break;
				//default: throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
		}

		public Vec3i(int a, int b, int c)
		{
			x = a;
			y = b;
			z = c;
		}
		public Vec3i(int a)
		{
			x = a;
			y = a;
			z = a;
		}

        public static Vec3i operator +(Vec3i a, Vec3i b)
		{
			return new Vec3i(a.x + b.x, a.y + b.y, a.z + b.z);
		}
		public static Vec3i operator -(Vec3i a, Vec3i b)
		{
			return new Vec3i(a.x - b.x, a.y - b.y, a.z - b.z);
		}
		public static Vec3i operator -(Vec3i a)
		{
			return new Vec3i(-a.x, -a.y, -a.z);
		}
		public static Vec3i operator *(Vec3i a, int d)
		{
			return new Vec3i(a.x * d, a.y * d, a.z * d);
		}
		public static Vec3i operator *(int d, Vec3i a)
		{
			return new Vec3i(a.x * d, a.y * d, a.z * d);
		}
		public static Vec3i operator /(Vec3i a, int d)
		{
			return new Vec3i(a.x / d, a.y / d, a.z / d);
		}
		public static bool operator ==(Vec3i lhs, Vec3i rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}
		public static bool operator !=(Vec3i lhs, Vec3i rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

		public static Vec3i Min(Vec3i a, Vec3i b)
		{
			Vec3i v;
			v.x = a.x < b.x ? a.x : b.x;
			v.y = a.y < b.y ? a.y : b.y;
			v.z = a.z < b.z ? a.z : b.z;
			return v;
			//return new Vec3i (Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
		}

		public static Vec3i Max(Vec3i a, Vec3i b)
		{
			Vec3i v;
			v.x = a.x > b.x ? a.x : b.x;
			v.y = a.y > b.y ? a.y : b.y;
			v.z = a.z > b.z ? a.z : b.z;
			return v;
			//return new Vec3i (Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
		}
    }
}