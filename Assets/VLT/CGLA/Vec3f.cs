using System;
namespace CGLA {

    /** 3D float vector.
    Class Vec3f is the vector typically used in 3D computer graphics. 
    The class has many constructors since we may need to convert from
    other vector types. Most of these are explicit to avoid automatic
    conversion. 
    */
	[System.Serializable]
    public struct Vec3f
    {
        public float x;
		public float y;
		public float z;
		public float this[int index]
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

        public static Vec3f operator +(Vec3f a, Vec3f b)
		{
			Vec3f v;
			v.x = a.x + b.x;
			v.y = a.y + b.y;
			v.z = a.z + b.z;
			return v;
			//return new Vec3f(a.x + b.x, a.y + b.y, a.z + b.z);
		}
		public static Vec3f operator -(Vec3f a, Vec3f b)
		{
			Vec3f v;
			v.x = a.x - b.x;
			v.y = a.y - b.y;
			v.z = a.z - b.z;
			return v;
			//return new Vec3f(a.x - b.x, a.y - b.y, a.z - b.z);
		}
		public static Vec3f operator -(Vec3f a)
		{
			Vec3f v;
			v.x = -a.x;
			v.y = -a.y;
			v.z = -a.z;
			return v;
			//return new Vec3f(-a.x, -a.y, -a.z);
		}
		public static Vec3f operator *(Vec3f a, float d)
		{
			Vec3f v;
			v.x = a.x * d;
			v.y = a.y * d;
			v.z = a.z * d;
			return v;
			//return new Vec3f(a.x * d, a.y * d, a.z * d);
		}
		public static Vec3f operator *(float d, Vec3f a)
		{
			Vec3f v;
			v.x = a.x * d;
			v.y = a.y * d;
			v.z = a.z * d;
			return v;
			//return new Vec3f(a.x * d, a.y * d, a.z * d);
		}
		public static Vec3f operator /(Vec3f a, float d)
		{
			Vec3f v;
			v.x = a.x / d;
			v.y = a.y / d;
			v.z = a.z / d;
			return v;
			//return new Vec3f(a.x / d, a.y / d, a.z / d);
		}
		public static bool operator ==(Vec3f lhs, Vec3f rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}
		public static bool operator !=(Vec3f lhs, Vec3f rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

        /// Construct 0 vector.
//        public Vec3f()
//        {
//            x = 0;
//            y = 0;
//            z = 0;
//        }

        /// Construct a 3D float vector.
        public Vec3f(float a, float b, float c)
        {
            x = a;
            y = b;
            z = c;
        }//: ArithVec<float,Vec3f,3>(a,b,c) {}

        /// Construct a vector with 3 identical coordinates.
        public Vec3f(float a)
        {
            x = a;
            y = a;
            z = a;
        }// ArithVec<float,Vec3f,3>(a,a,a) {}
			
		/// Construct from a 3D double vector.
		public Vec3f(Vec3d v)
		{
			x = (float)v.x;
			y = (float)v.y;
			z = (float)v.z;
		}

		public UnityEngine.Vector3 ToVector3()
		{
			UnityEngine.Vector3 result;
			result.x = x;
			result.y = z;//TODO OGL to Unity
			result.z = y;
			return result;
		}

        /// Compute Euclidean length.
        public float length() 
        {
            return (float)System.Math.Sqrt(x*x+y*y+z*z);
        }

        public float magnitude() 
        {
			return /*(float)Math.Sqrt*/(x * x + y * y + z * z);//TODO
        }

        /// Normalize vector.
        public void normalize() 
        {
            float l = length();
            x /= l;
            y /= l;
            z /= l;
        }

        /** Get the vector in spherical coordinates.
        The first argument (theta) is inclination from the vertical axis.
        The second argument (phi) is the angle of rotation about the vertical 
        axis. The third argument (r) is the length of the vector. */
        //public void get_spherical( float, float, float );

        /** Assign the vector in spherical coordinates.
        The first argument (theta) is inclination from the vertical axis.
        The second argument (phi) is the angle of rotation about the vertical 
        axis. The third argument (r) is the length of the vector. */
        //public void set_spherical( float, float, float);

        /// Returns normalized vector
        public static Vec3f normalize(Vec3f v) 
        {
            return v/v.length();
        }

        /// Returns cross product of arguments
        public static Vec3f cross( Vec3f x, Vec3f y ) 
        {
//			Vec3f result;
//			result.x = x [1] * y [2] - x [2] * y [1];
//			result.y = x[2] * y[0] - x[0] * y[2];
//			result.z = x[0] * y[1] - x[1] * y[0];
//			return result;
            return new Vec3f( x[1] * y[2] - x[2] * y[1], 
                x[2] * y[0] - x[0] * y[2], 
                x[0] * y[1] - x[1] * y[0] );
        }

        /// Returns (component-wise) floor of argument
        public static Vec3f floor(Vec3f x) 
        {
            return new Vec3f((float)System.Math.Floor(x[0]), (float)System.Math.Floor(x[1]), (float)System.Math.Floor(x[2]));
        }

        /** Compute basis of orthogonal plane.
        Given a vector Compute two vectors that are orothogonal to it and 
        to each other. */
        //public static void orthogonal(Vec3f a, Vec3f b, Vec3f c);

		/** Dot product for two vectors. The `*' operator is 
			reserved for coordinatewise	multiplication of vectors. */
		public static float dot(Vec3f v0, Vec3f v1)
		{
			return v0.x * v1.x + v0.y * v1.y + v0.z * v1.z;
		}

		public static Vec3f Min(Vec3f a, Vec3f b)
		{
			Vec3f v;
			v.x = a.x < b.x ? a.x : b.x;
			v.y = a.y < b.y ? a.y : b.y;
			v.z = a.z < b.z ? a.z : b.z;
			return v;
			//return new Vec3f (Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
		}

		public static Vec3f Max(Vec3f a, Vec3f b)
		{
			Vec3f v;
			v.x = a.x > b.x ? a.x : b.x;
			v.y = a.y > b.y ? a.y : b.y;
			v.z = a.z > b.z ? a.z : b.z;
			return v;
			//return new Vec3f (Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
		}
    }
}