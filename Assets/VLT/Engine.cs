using System;
using System.Collections.Generic;
using System.IO;
using CGLA;

namespace VLT
{
	[System.Serializable]
    public class Engine : IDisposable
    {
		[UnityEngine.SerializeField]private Render m_render;
		[UnityEngine.SerializeField]private Terrain m_terrain;
		[UnityEngine.SerializeField]private UserInterface m_user_interface;
		[UnityEngine.SerializeField]private List<Camera> m_cameras = new List<Camera>();
		[UnityEngine.SerializeField]private int m_current_camera;
		[UnityEngine.SerializeField]SplineCamera m_spline_camera;
		[UnityEngine.SerializeField]private float m_frame_time;
		[UnityEngine.SerializeField]private float m_move_speed;
		[UnityEngine.SerializeField]private bool m_video_mode;
		[UnityEngine.SerializeField]private string m_frame_path;
		[UnityEngine.SerializeField]private Vec3d m_direction;

//        private Skybox m_skybox;

        private void update_sun()
        {
            float i = 0.8f;

            //i += 0.510472f * m_frame_time;
			if (i > MathExt.M_PI) i -= MathExt.M_PI;

            m_terrain.set_sun_direction(new Vec3f((float)System.Math.Cos(i), 0, (float)System.Math.Sin(i)));
        }

        private void detect_collision()
        {
			Vec3f eye = m_cameras[m_current_camera].get_eye().ToVec3f();
            float h = m_terrain.get_height(eye[0], eye[1]);
            float difference = eye[2] - h;
            if( difference < 1.8f )
            {
                eye[2] = h + 1.8f;
				m_cameras[m_current_camera].set_eye(new Vec3d(eye));
            }
        }
    
        public Engine(string filename, int width, int height, bool fullscreen)
        {
            m_frame_time = 0;
            m_move_speed = 2.0f;
            m_direction = new Vec3d(0,0,0);

            float fov = Configuration.get_float("camera_fov");
            float aspect = (float)(Configuration.get_int("screen_width")) / Configuration.get_int("screen_height");
            float z_near = Configuration.get_float("camera_z_near");
            float z_far = Configuration.get_float("camera_z_far");

            m_cameras.Add(new Camera(fov, aspect, z_near, z_far));
			m_spline_camera = new SplineCamera (fov, aspect, z_near, z_far);
			m_cameras.Add(m_spline_camera);

			m_current_camera = 0;

            m_render = Render.create_render(this, width, height, fullscreen);
            
            // FIXME
        	//#error Har du husket at tilpasse denne til din .map fil? :-)
            m_render.initialize_textures(Configuration.get_int("quad_cache_size"), Configuration.get_int("dynamic_texture_size"), 32, true);

            m_terrain = new Terrain(m_render, filename);
//            m_skybox = new Skybox(m_render);

            m_user_interface = new UserInterface(m_render);

            detect_collision();

            m_video_mode = false;

			if (Configuration.get_bool("demo_mode"))
			{
				m_spline_camera.load(Configuration.get_string("camera_path"));
				m_current_camera++;
				m_spline_camera.start();
			}
        }

        public void Dispose()
        {
            m_user_interface.Dispose();
//          m_skybox.Dispose();
            m_terrain.Dispose();
            m_render.Dispose();
			m_user_interface.Dispose ();
//          m_skybox = null;
            m_terrain = null;
            m_render = null;
        }

        public void run()
        {
			QuadNodeGenerator.thread_proc (null);//TODO background threads
            m_render.run();
        }

        public void render()
        {
            if(!m_video_mode)
            {
				m_frame_time = UnityEngine.Time.deltaTime;
            }

            // FIXME
           /* m_user_interface.write(0, "frame_time: ");
            m_user_interface.append(0, m_frame_time);*/

            m_user_interface.write(0, "fps: ");
            m_user_interface.append(0, 1.0f / m_frame_time);
        
            m_user_interface.write(1, "speed:");
            m_user_interface.append(1, m_move_speed);
            m_user_interface.append(1, "m/s");


			m_cameras[m_current_camera].update(m_frame_time, m_user_interface);
            m_terrain.update(m_frame_time, m_user_interface);

            m_user_interface.update();

			m_terrain.set_camera_velocity(m_cameras[m_current_camera].get_velocity());//TODO

            update_sun();

//           m_skybox.draw(m_current_camera);
			m_terrain.draw(m_cameras[m_current_camera]);

            if (m_direction.length() > 0.0)
            {
                m_direction.normalize();
				m_cameras[m_current_camera].move(m_direction * m_move_speed * m_frame_time);
                detect_collision();
                m_direction = new Vec3d(0,0,0);
            }
        }

		public void draw_interface()
		{
			m_user_interface.draw ();
		}

		public void draw_gizmos()
		{
			if (m_cameras == null || m_cameras.Count <= m_current_camera) {
				return;
			}

			m_cameras [m_current_camera].draw_gizmos ();

			if (m_terrain != null)
				m_terrain.draw_gizmos ();
		}

		public bool input(byte[] key_states, byte[] key_presses, float x, float y)
        {
            /* speed:
            200 m/s = 720 km/h
            100 m/s = 360 km/h
            50 m/s = 180 km/h
            25 m/s =  90 km/h
            10 m/s =  36 km/h
            5 m/s =  18 km/h
            2 m/s =   7 km/h
            1 m/s =   4 km/h
            */

			if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
            {
				UnityEngine.Application.Quit ();
                return false;
            }
			if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
            {
                move_forward();
            }
			if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
            {
                move_backwards();
            }
			if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
            {
                move_left();
            }
			if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
            {
                move_right();
            }
			if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Q))
            {
                move_up();
            }
			if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.Z))
            {
                move_down();
            }

			if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Alpha1))
            {
                accelerate();
            }

			if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Alpha2))
            {
                deaccelerate();
            }

			if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.L))
            {
                toggle_wireframe();
            }

			if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.C))
            {
                m_current_camera++;

                if (m_current_camera >= m_cameras.Count)
                {
                    m_current_camera = 0;
                }
            }

			mouse_input(x * m_frame_time, y * m_frame_time);

			if (m_cameras[m_current_camera] == m_spline_camera)
			{
				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
				{
					m_spline_camera.add_key();
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.P))
				{
					m_spline_camera.start();
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.O))
				{
					m_spline_camera.stop();
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.U))
				{
					m_spline_camera.save(Configuration.get_string("camera_path"));
					m_user_interface.log("spline path saved");
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.I))
				{
					m_spline_camera.load(Configuration.get_string("camera_path"));
					m_user_interface.log("spline path loaded");
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Period))
				{
					m_spline_camera.next_key();
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Comma))
				{
					m_spline_camera.previous_key();
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.T))
				{
					m_spline_camera.replace_key();
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Y))
				{
					m_spline_camera.insert_key();
				}

				if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.N))
				{
					m_spline_camera.delete_key();
				}
			}

            return true;
        }

        public void move_forward()
        {
            m_user_interface.log("w pressed");

			m_direction += new Vec3d(0, 1, 0);
        }

        public void move_backwards()
        {
            m_user_interface.log("s pressed");

			m_direction += new Vec3d(0, -1, 0);
        }

        public void move_left()
        {
            m_user_interface.log("a pressed");

			m_direction += new Vec3d(-1, 0, 0);
        }

        public void move_right()
        {
            m_user_interface.log("d pressed");

			m_direction += new Vec3d(1, 0, 0);
        }

        public void move_up()
        {
            m_user_interface.log("q pressed");

			m_direction += new Vec3d(0, 0, 1);
        }

        public void move_down()
        {
            m_user_interface.log("z pressed");

			m_direction += new Vec3d(0, 0, -1);
        }

        public void accelerate()
        {
            m_move_speed *= 2;

            m_user_interface.write(8, "Moving faster!", 3); // FIXME
        }

        public void deaccelerate()
        {
            m_move_speed /= 2;

            m_user_interface.write(8, "Moving slower.", 3);  // FIXME
        }

        public void toggle_wireframe()
        {
            m_terrain.toggle_wireframe();
        }

        public void mouse_input(float x, float y)
        {
            const float rotate_speed = 10.0f;
			m_cameras[m_current_camera].rotate(x * rotate_speed, y * rotate_speed);
        }
    }
}