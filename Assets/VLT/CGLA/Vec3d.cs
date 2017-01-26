namespace CGLA {
	public enum Axis {XAXIS=0,YAXIS=1,ZAXIS=2};
	/** A 3D double vector. Useful for high precision arithmetic. */

	[System.Serializable]
	public struct Vec3d
	{
        public double x, y, z;

        public static Vec3d operator +(Vec3d a, Vec3d b)
		{
			Vec3d v;
			v.x = a.x + b.x;
			v.y = a.y + b.y;
			v.z = a.z + b.z;
			return v;
		}

		public static Vec3d operator -(Vec3d a, Vec3d b)
		{
			Vec3d v;
			v.x = a.x - b.x;
			v.y = a.y - b.y;
			v.z = a.z - b.z;
			return v;
		}

		public static Vec3d operator -(Vec3d a)
		{
			Vec3d v;
			v.x = -a.x;
			v.y = -a.y;
			v.z = -a.z;
			return v;
		}

		public static Vec3d operator *(Vec3d a, double d)
		{
			Vec3d v;
			v.x = a.x * d;
			v.y = a.y * d;
			v.z = a.z * d;
			return v;
		}

		public static Vec3d operator *(double d, Vec3d a)
		{
			Vec3d v;
			v.x = a.x * d;
			v.y = a.y * d;
			v.z = a.z * d;
			return v;
		}

		public static Vec3d operator /(Vec3d a, double d)
		{
			Vec3d v;
			v.x = a.x / d;
			v.y = a.y / d;
			v.z = a.z / d;
			return v;
		}

		public static bool operator ==(Vec3d lhs, Vec3d rhs)
		{
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}

		public static bool operator !=(Vec3d lhs, Vec3d rhs)
		{
			return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
		}

        public double this[int index]
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
				default:	//throw new Exception("Invalid Vector3 index!");
                    return 0;
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
				//default: //throw new IndexOutOfRangeException("Invalid Vector3 index!");
				}
			}
		}

		/// Construct vector
		public Vec3d(double a, double b, double c)
        {
            x = a;
            y = b;
            z = c;
        }

		/// Construct vector where all coords = a 
		public Vec3d(double a)
        {
            x = a;
            y = a;
            z = a;
        }

		/// Convert from float vector
		public Vec3d(Vec3f v)
		{
			x = v.x;
			y = v.y;
			z = v.z;
		}

		public Vec3d(Vec2d v)
		{
			x = v.x;
			y = v.y;
			z = 0;
		}

		public Vec3f ToVec3f()
		{
			Vec3f v;
			v.x = (float)x;
			v.y = (float)y;
			v.z = (float)z;
			return v;
		}

		public UnityEngine.Vector3 ToVector3()
		{
			UnityEngine.Vector3 result;
			result.x = (float)x;
			result.y = (float)z;//TODO OGL to Unity
			result.z = (float)y;
			return result;
		}

		/// Returns euclidean length
		public double length()
        {
            return System.Math.Sqrt(x*x+y*y+z*z);
        }
    
		/// Normalize vector.
		public void normalize() 
        {
            double l = length();
            x /= l;
            y /= l;
            z /= l;
        }

		public override string ToString ()
		{
			
			return "[" + System.Math.Round(x, 3) + ", " + System.Math.Round(y, 3) + ", " + System.Math.Round(z, 3) + "]";
		}
	  
		/** Get the vector in spherical coordinates.
				The first argument (theta) is inclination from the vertical axis.
				The second argument (phi) is the angle of rotation about the vertical 
				axis. The third argument (r) is the length of the vector. */
		//public void get_spherical( double&, double&, double& ) const;

		/** Assign the vector in spherical coordinates.
				The first argument (theta) is inclination from the vertical axis.
				The second argument (phi) is the angle of rotation about the vertical 
				axis. The third argument (r) is the length of the vector. */
		//public bool set_spherical( double, double, double);

        /// Returns normalized vector
        public static Vec3d normalize(Vec3d v) 
        {
            return v/v.length();
        }

        /// Compute dot product
        public static double dot( Vec3d a, Vec3d b ) 
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        /// Compute cross product
        public static Vec3d cross( Vec3d a, Vec3d b ) 
        {
			Vec3d v;
			v.x = a.y * b.z - a.z * b.y;
			v.y = a.z * b.x - a.x * b.z;
			v.z = a.x * b.y - a.y * b.x;
			return v;
            //return new Vec3d(x[1]*y[2] - x[2]*y[1], x[2]*y[0] - x[0]*y[2], x[0]*y[1] - x[1]*y[0]);
        }
	}

}