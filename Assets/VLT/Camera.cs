using System;
using CGLA;

namespace VLT
{
	[Serializable]
    public class Camera : IDisposable
    {
		[UnityEngine.SerializeField]private Vec3d m_eye;
		[UnityEngine.SerializeField]private Vec3d m_center;
		[UnityEngine.SerializeField]private Vec3d m_up; 
		[UnityEngine.SerializeField]private Vec3d m_previous_eye;
		[UnityEngine.SerializeField]private float m_velocity;
        private float m_fov;
        private float m_aspect;
        private float m_z_near;
        private float m_z_far;
		[UnityEngine.SerializeField]private double m_horizontal_angle;
		[UnityEngine.SerializeField]private double m_vertical_angle;
        private Plane[] m_frustum_planes = new Plane[6];
		[UnityEngine.SerializeField]private bool m_changed;

        private void make_planes()
        {
			Vec3f forward = new Vec3f(Vec3d.normalize(m_center - m_eye));
			Vec3f right = Vec3f.cross(forward, new Vec3f(m_up));
			Vec3f up = Vec3f.cross(right, forward);

			Mat3x3f camera_mat = (new Mat3x3f(right, forward, up)).Transpose();

			Vec3f eye = new Vec3f(m_eye);

            // near
            m_frustum_planes[0] = new Plane(eye + forward * m_z_near, forward);

            // far
            m_frustum_planes[1] = new Plane(eye + forward * m_z_far, -forward);

            // left
			Mat3x3f rotate_horizontal_left = Mat3x3f.Rotation_Mat3x3f(Axis.ZAXIS, deg_to_rad(90.0f - m_fov * m_aspect * 0.5f));
			m_frustum_planes[2] = new Plane(eye, Vec3f.normalize((camera_mat * rotate_horizontal_left) * new Vec3f(0,1,0)));

            // right
			Mat3x3f rotate_horizontal_right = Mat3x3f.Rotation_Mat3x3f(Axis.ZAXIS, deg_to_rad(m_fov * m_aspect * 0.5f - 90.0f));
			m_frustum_planes[3] = new Plane(eye, Vec3f.normalize((camera_mat * rotate_horizontal_right) * new Vec3f(0,1,0)));

            // top
			Mat3x3f rotate_vertical_top = Mat3x3f.Rotation_Mat3x3f(Axis.XAXIS, deg_to_rad(90.0f - m_fov * 0.5f));
			m_frustum_planes[4] = new Plane(eye, Vec3f.normalize((camera_mat * rotate_vertical_top) * new Vec3f(0,1,0)));

            // bottom
			Mat3x3f rotate_vertical_bottom = Mat3x3f.Rotation_Mat3x3f(Axis.XAXIS, deg_to_rad(m_fov * 0.5f - 90.0f));
			m_frustum_planes[5] = new Plane(eye, Vec3f.normalize((camera_mat * rotate_vertical_bottom) * new Vec3f(0,1,0)));

			set_changed(false);
        }

		void set_changed(bool status)
		{
//			if(status)
//				UnityEngine.Debug.Log ("True");
//			else
//				UnityEngine.Debug.Log ("False");
			m_changed = status;
		}

		float deg_to_rad(float deg)
		{
			return deg * (float)Math.PI / 180.0f;
		}

		double clamp(double value, double min_value, double max_value)
		{
			return value < min_value ? min_value : (value > max_value ? max_value : value);
		}

        public Camera(float fov, float aspect, float z_near, float z_far)
        {
            m_eye = new Vec3d(0.0f, 0.0f, 0.0f);
            m_center = new Vec3d(0.0f, 1.0f, 0.0f);
            m_up = new Vec3d(0.0f, 0.0f, 1.0f);
            m_previous_eye = new Vec3d(0.0f, 0.0f, 0.0f);
            m_velocity = 0.0f;
            m_horizontal_angle = 0.0;
            m_vertical_angle = 0.0;
            m_fov = fov;
            m_aspect = aspect;
            m_z_near = z_near;
            m_z_far = z_far;

			UnityEngine.Camera.main.aspect = aspect;
			UnityEngine.Camera.main.fieldOfView = fov;
			UnityEngine.Camera.main.nearClipPlane = z_near;
			UnityEngine.Camera.main.farClipPlane = z_far;
            set_changed(true);
        }

        public Camera(Camera camera)
        {
            m_eye = camera.m_eye;
            m_center = camera.m_center;
            m_up = camera.m_up;
            m_horizontal_angle = camera.m_horizontal_angle;
            m_vertical_angle = camera.m_vertical_angle;
            m_fov = camera.m_fov;
            m_aspect = camera.m_aspect;
            m_z_near = camera.m_z_near;
            m_z_far = camera.m_z_far;

			UnityEngine.Camera.main.aspect = m_aspect;
			UnityEngine.Camera.main.fieldOfView = m_fov;
			UnityEngine.Camera.main.nearClipPlane = m_z_near;
			UnityEngine.Camera.main.farClipPlane = m_z_far;
            set_changed(true);
        }

        public virtual void Dispose()
        {

        }

        public virtual string get_camera_name()
        {
            return "Regular";
        }

        public virtual void update(float frame_time, UserInterface user_interface)
        {
            m_velocity = (float)((m_previous_eye - m_eye).length()) / frame_time;

            m_previous_eye = m_eye;

            user_interface.write(6, "Camera:");
            user_interface.append(6, get_camera_name());

            user_interface.write(7, "position:");
            user_interface.append(7, get_eye());

            user_interface.write(8, "direction:");
            user_interface.append(8, get_center() - get_eye());

            // FIXME!!
            user_interface.write(9, "");
            user_interface.write(10, "");

			UnityEngine.Camera.main.transform.position = m_eye.ToVector3 ();
			Vec3d v = m_center - m_eye;
			v.normalize ();
			UnityEngine.Camera.main.transform.forward = v.ToVector3 ();
        }

        public virtual void move(Vec3d movement)
        {
			Vec3d forward = Vec3d.normalize(m_center - m_eye);
			Vec3d right = Vec3d.cross(forward, m_up);
			Vec3d up = Vec3d.cross(right, forward);

            Mat3x3d mat = new Mat3x3d(right, forward, up);

			Vec3d translation = mat.Transpose() * movement;

            m_eye += translation;
            m_center += translation;


            set_changed(true);
        }
		public virtual void rotate(double horizontal, double vertical)
        {
			m_horizontal_angle = (m_horizontal_angle + horizontal) % (2.0 * Math.PI);
			m_vertical_angle = clamp(m_vertical_angle + vertical, -Math.PI / 3.0, Math.PI / 3.0);

			Mat3x3d rotate_vertical = Mat3x3d.Rotation_Mat3x3d(Axis.XAXIS, m_vertical_angle);
			Mat3x3d rotate_horizontal = Mat3x3d.Rotation_Mat3x3d(Axis.ZAXIS, m_horizontal_angle);

			m_center = rotate_horizontal * rotate_vertical * new Vec3d(0, 1, 0) + m_eye;

            set_changed(true);
        }

        public float get_velocity()
        {
            return m_velocity;
        }

        public void set_eye(Vec3d eye)
        {
            m_eye = eye;

            set_changed(true);
        }

        public void set_center(Vec3d center)
        {
            m_center = center;
			Vec3d diff = Vec3d.normalize(m_center - m_eye);
			m_horizontal_angle =  Math.PI / 2.0 - Math.Atan2(diff[1], diff[0]);

            if (diff[0] < 0.0 && diff[1] > 0.0)
            {
                m_horizontal_angle +=  2.0 * Math.PI;
            }

			m_vertical_angle = -Math.Atan(diff[2] / Math.Sqrt(diff[0] * diff[0] + diff[1] * diff[1]));

            set_changed(true);
        }

        public void set_up(Vec3d up)
        {
            m_up = up;

            set_changed(true);
        }

        public void set_z_near(float z_near)
        {
            m_z_near = z_near;

            set_changed(true);
        }

        public void set_z_far(float z_far)
        {
            m_z_far = z_far;

            set_changed(true);
        }

        public virtual Vec3d get_eye()
        {
            return m_eye;
        }

        public virtual Vec3d get_center()
        {
            return m_center;
        }

        public virtual Vec3d get_up()
        {
            return m_up;
        }

        public float get_fov()
        {
            return m_fov;
        }

        public float get_aspect()
        {
            return m_aspect;
        }

        public float get_z_near()
        {
            return m_z_near;
        }

        public float get_z_far()
        {
            return m_z_far;
        }

        public bool can_see(QuadNode node)
        {
            if (m_changed)
            {
                make_planes();
            }

            for (int p = 0; p < 6; p++)
            {
                if (!node.get_bounding_box().is_above(m_frustum_planes[p]))
                {
                    return false;
                }
            }

            return true;
        }
        public float get_distance(BoundingBox bounding_box)
        {
            return bounding_box.get_distance(new Vec3f(m_eye));
        }

		public void draw_gizmos()
		{
			Vec3d forward = Vec3d.normalize(m_center - m_eye);
			Vec3d right = Vec3d.cross(forward, m_up);
			Vec3d up = Vec3d.cross(right, forward);

			UnityEngine.Vector3 v1 = m_eye.ToVector3();
			UnityEngine.Gizmos.color = UnityEngine.Color.green;
			UnityEngine.Gizmos.DrawWireSphere (v1, 0.1f);

			UnityEngine.Vector3 v2 = m_center.ToVector3();
			UnityEngine.Gizmos.color = UnityEngine.Color.red;
			UnityEngine.Gizmos.DrawWireSphere (v2, 0.1f);

			UnityEngine.Vector3 f = forward.ToVector3();
			UnityEngine.Gizmos.color = UnityEngine.Color.yellow;
			UnityEngine.Gizmos.DrawRay (v1, f);

			UnityEngine.Vector3 r = right.ToVector3();
			UnityEngine.Gizmos.color = UnityEngine.Color.black;
			UnityEngine.Gizmos.DrawRay (v1, r);

			UnityEngine.Vector3 u = up.ToVector3();
			UnityEngine.Gizmos.color = UnityEngine.Color.blue;
			UnityEngine.Gizmos.DrawRay (v1, u);

			UnityEngine.Gizmos.color = UnityEngine.Color.green;
			UnityEngine.Gizmos.DrawRay (v1, v2-v1);

			draw_frustum ();
		}

		UnityEngine.Vector3[] nearCorners = new UnityEngine.Vector3[4];
		UnityEngine.Vector3[] farCorners = new UnityEngine.Vector3[4];
		UnityEngine.Plane[] camPlanes = new UnityEngine.Plane[6];

		void draw_frustum () {

			for (int i = 0; i < m_frustum_planes.Length; i++) {
				camPlanes [i] = new UnityEngine.Plane (m_frustum_planes [i].get_normal ().ToVector3 (), m_frustum_planes [i].get_point ().ToVector3 ());
			}

			UnityEngine.Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop

			for ( int i = 0; i < 4; i++ ) {
				nearCorners[i] = Plane3Intersect( camPlanes[4], camPlanes[i], camPlanes[( i + 1 ) % 4] ); //near corners on the created projection matrix
				farCorners[i] = Plane3Intersect( camPlanes[5], camPlanes[i], camPlanes[( i + 1 ) % 4] ); //far corners on the created projection matrix
			}

			for ( int i = 0; i < 4; i++ ) {
				UnityEngine.Debug.DrawLine( nearCorners[i], nearCorners[( i + 1 ) % 4], UnityEngine.Color.red, UnityEngine.Time.deltaTime, true ); //near corners on the created projection matrix
				UnityEngine.Debug.DrawLine( farCorners[i], farCorners[( i + 1 ) % 4], UnityEngine.Color.blue, UnityEngine.Time.deltaTime, true ); //far corners on the created projection matrix
				UnityEngine.Debug.DrawLine( nearCorners[i], farCorners[i], UnityEngine.Color.green, UnityEngine.Time.deltaTime, true ); //sides of the created projection matrix
			}
		}

		UnityEngine.Vector3 Plane3Intersect ( UnityEngine.Plane p1, UnityEngine.Plane p2, UnityEngine.Plane p3 ) { //get the intersection point of 3 planes
			return ( ( -p1.distance * UnityEngine.Vector3.Cross( p2.normal, p3.normal ) ) +
				( -p2.distance * UnityEngine.Vector3.Cross( p3.normal, p1.normal ) ) +
				( -p3.distance * UnityEngine.Vector3.Cross( p1.normal, p2.normal ) ) ) /
				( UnityEngine.Vector3.Dot( p1.normal, UnityEngine.Vector3.Cross( p2.normal, p3.normal ) ) );
		}
    }
}