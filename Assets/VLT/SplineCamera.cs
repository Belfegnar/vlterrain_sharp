using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;
using System.IO;

namespace VLT
{
	public class SplineCamera : Camera {

		List<Vec3d> m_eye_keys = new List<Vec3d>();
		List<Vec3d> m_center_keys = new List<Vec3d>();

		float m_time_between_keys;
		float m_running_time;

		int m_current_key;
		bool m_current_key_changed;

		bool m_animating;

		bool m_demo_mode;

		int get_key_index(int first_key, int delta)
		{
			int max_size = m_eye_keys.Count;

			// FIXME!!!
			//return modulo(first_key + delta, max_size);
			return MathExt.clamp(first_key + delta, 0, max_size - 1);
		}

		Vec3d get_spline_position(Vec3d k1, Vec3d k2, Vec3d k3, Vec3d k4, double fraction)
		{
			Vec3d tangent_1 = 0.5 * (k2 - k1) + 0.5 * (k3 - k2);
			Vec3d tangent_2 = 0.5 * (k3 - k2) + 0.5 * (k4 - k3);
			double fraction2 = fraction * fraction;
			double fraction3 = fraction2 * fraction;

			return ((2 * fraction3 - 3 * fraction2 + 1) * k2 + (-2 * fraction3 + 3 * fraction2) * k3 +
				(fraction3 - 2 * fraction2 + fraction) * tangent_1 + (fraction3 - fraction2) * tangent_2);
		}

		public SplineCamera(float fov, float aspect, float z_near, float z_far) : base(fov, aspect, z_near, z_far)
		{
			m_time_between_keys = 2.0f;
			m_running_time = 0.0f;
			m_animating = false;
			m_current_key = 0;
			m_current_key_changed = false;

			m_demo_mode = Configuration.get_bool("demo_mode") || Configuration.get_bool("video_mode");
		}

		public override string get_camera_name()
		{
			return "Spline";
		}

		public override void update(float frame_time, UserInterface user_interface)
		{
			base.update(frame_time, user_interface);

			user_interface.write(9, "camera mode:");
			user_interface.append(9, m_animating ? "play" : "edit");
			user_interface.write(10, "camera keys:");
			user_interface.append(10, (int)(m_eye_keys.Count));
			user_interface.append(10, " - current key:");
			user_interface.append(10, (m_animating ? (int)(m_running_time * m_eye_keys.Count / ((m_eye_keys.Count - 1) * m_time_between_keys)) : (int)(m_current_key)));

			if (!m_animating || m_eye_keys.Count < 3)
			{
				if ( (m_eye_keys.Count > 0) && (m_current_key_changed))
				{
					set_eye(m_eye_keys[m_current_key - 1]);
					set_center(get_eye() + m_center_keys[m_current_key - 1]);
					m_current_key_changed = false;
				}

				return;
			}

			m_running_time += frame_time;

			if (m_running_time > (m_eye_keys.Count - 1) * m_time_between_keys)
			{
				if (m_demo_mode)
				{

					UnityEngine.Application.Quit();
				}

				m_running_time = 0.0f;
			}

			double fraction = m_running_time / m_time_between_keys;
			int first_key = (int)(Math.Floor(fraction));
			fraction -= first_key;

			int key_0 = get_key_index(first_key, -1);
			int key_1 = get_key_index(first_key,  0);
			int key_2 = get_key_index(first_key,  1);
			int key_3 = get_key_index(first_key,  2);

			set_eye(get_spline_position(m_eye_keys[key_0], m_eye_keys[key_1],
				m_eye_keys[key_2], m_eye_keys[key_3], fraction));

			set_center(get_eye() + get_spline_position(m_center_keys[key_0], m_center_keys[key_1],
				m_center_keys[key_2], m_center_keys[key_3], fraction));
		}

		public void next_key()
		{
			if (m_animating)
			{
				m_running_time += m_time_between_keys;
			}
			else if (m_eye_keys.Count > 0)
			{
				if (++m_current_key > m_eye_keys.Count)
				{
					m_current_key = 1;
				}

				m_current_key_changed = true;
			}
		}
		public void previous_key()
		{
			if (m_animating)
			{
				m_running_time = Math.Max(0.0f, m_running_time - m_time_between_keys);
			}
			else if (m_eye_keys.Count > 0)
			{
				if (--m_current_key <= 0)
				{
					m_current_key = m_eye_keys.Count;
				}

				m_current_key_changed = true;
			}
		}
		public void replace_key()
		{
			if (m_eye_keys.Count > 0)
			{
				m_eye_keys[m_current_key - 1] = get_eye();
				m_center_keys[m_current_key - 1] = Vec3d.normalize(get_center() - get_eye());
			}
		}
		public void insert_key()
		{
			if (m_eye_keys.Count > 0)
			{
				int eye_it = m_current_key - 1;
				int center_it = m_current_key - 1;

				m_eye_keys.Insert(eye_it, get_eye());
				m_center_keys.Insert(center_it, Vec3d.normalize(get_center() - get_eye()));
				m_current_key_changed = true;
			}
		}
		public void add_key()
		{
			m_eye_keys.Add(get_eye());
			m_center_keys.Add(Vec3d.normalize(get_center() - get_eye()));
			m_current_key = m_eye_keys.Count;
		}
		public void delete_key()
		{
			if (m_eye_keys.Count > 0)
			{
				int eye_it = m_current_key - 1;
				int center_it = m_current_key - 1;

				m_eye_keys.RemoveAt(eye_it);
				m_center_keys.RemoveAt(center_it);

				if (m_current_key > m_eye_keys.Count)
				{
					m_current_key = m_eye_keys.Count;
				}
				m_current_key_changed = true;
			}
		}

		public void start()
		{
			m_animating = true;
			m_running_time = 0.0f;
		}

		public void stop()
		{
			m_animating = false;
		}

		public void save(string filename)
		{
			using (StreamWriter output_file = new StreamWriter (File.Open (filename, FileMode.OpenOrCreate))) {

				output_file.WriteLine (m_center_keys.Count);
				output_file.WriteLine (m_time_between_keys);

				string line;
				for (int i = 0; i < m_center_keys.Count; i++) {
					
					line = (m_eye_keys [i] [0]) + " ";
					line += (m_eye_keys [i] [1]) + " ";
					line += (m_eye_keys [i] [2]) + " ";
					line += (m_center_keys [i] [0]) + " ";
					line += (m_center_keys [i] [1]) + " ";
					line += (m_center_keys [i] [2]);
					output_file.WriteLine (line);
				}
			}
		}

		public void load(string filename)
		{
			using (StreamReader input_file = new StreamReader (File.Open (filename, FileMode.Open))) {

				int keys = int.Parse (input_file.ReadLine ());
				m_time_between_keys = float.Parse (input_file.ReadLine ());

				Vec3d eye;
				Vec3d center;
				string line;
				string[] doubles;
				char[] separator = new char[]{ ' ' };

				for (int i = 0; i < keys; i++) 
				{
					line = input_file.ReadLine ();
					doubles = line.Split (separator);
					eye.x = double.Parse(doubles[0]);
					eye.y = double.Parse(doubles[1]);
					eye.z = double.Parse(doubles[2]);
					center.x = double.Parse(doubles[3]);
					center.y = double.Parse(doubles[4]);
					center.z = double.Parse(doubles[5]);
					//UnityEngine.Debug.Log ("eye = " + eye + ", center = " + center);
					m_eye_keys.Add(eye);
					m_center_keys.Add(center);
				}
			}
		}
	}
}