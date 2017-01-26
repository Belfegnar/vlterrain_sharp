using System;
using System.Collections.Generic;
using System.IO;

namespace VLT
{
    public class Configuration
    {
        private static Configuration m_instance;

        private class Value
        {
            private object m_value;

            public enum ValueType
            {
                Integer,
                Float,
                Boolean,
                String
            };
            public ValueType m_type;

            public Value(int val)
            {
                m_type = ValueType.Integer;
                m_value = val;
            }
            public Value(float val)
            {
                m_type = ValueType.Float;
                m_value = val;
            }
            public Value(bool val)
            {
                m_type = ValueType.Boolean;
                m_value = val;
            }
            public Value(string val)
            {
                m_type = ValueType.String;
                m_value = val;
            }

            public void Dispose()
            {
                m_value = null;
            }

            public int get_int()
            {
                return (int)m_value;
            }

            public float get_float()
            {
                return (float)m_value;
            }

            public bool get_bool()
            {
                return (bool)m_value;
            }

            public string get_string()
            {
                return (string)m_value;
            }

			public override string ToString ()
			{
				return m_type + " " + m_value;
			}
        };

		private Dictionary<string, Value> m_variables = new Dictionary<string, Value>();

        private Configuration()
        {
            set_value("screen_width", new Value(800));
            set_value("screen_height", new Value(600));
            set_value("screen_fullscreen", new Value(false));

            set_value("vertex_cache_size", new Value(24));
            set_value("quad_cache_size", new Value(1024));

            set_value("lod_error", new Value(8.0f));
            set_value("lod_minimum_spacing", new Value(0.1f));
            set_value("dynamic_texture_size", new Value(128));

            set_value("cg_profile", new Value("nv20"));

            set_value("reader", new Value("memory mapped"));

            set_value("render", new Value("gl"));

            set_value("memory", new Value("gl_vbo"));

            set_value("camera_fov", new Value(55.0f));
            set_value("camera_z_near", new Value(0.5f));
            set_value("camera_z_far", new Value(250000.0f));

            set_value("detail_create_new_nodes", new Value(true));
            set_value("detail_subdivision_factor", new Value(1.0f));
            set_value("detail_add_material", new Value(true));
            set_value("detail_material_type", new Value(1));

            set_value("camera_path", new Value("camera.pth"));

            set_value("draw_user_interface", new Value(true));

            set_value("demo_mode", new Value(false));

            set_value("log_file", new Value("tengine.log"));

            set_value("video_mode", new Value(false));
            set_value("frames_per_second", new Value(30));
            set_value("image_path", new Value("c:\\"));
        }

        private void Dispose()
        {
            foreach (KeyValuePair<string, Value> it in m_variables)
            {
                it.Value.Dispose();
            }
        }

        private static Configuration get_instance()
        {
            if (m_instance == null)
            {
                m_instance = new Configuration();
            }

            return m_instance;
        }

        private string get_name(string line)
        {
            int assignment = line.IndexOf('=');
            if(assignment >= 0)
            {
				return line.Substring(0, assignment).Trim ();//TODO: Check
            }
            else
            {
                return "";
            }
        }

        private string get_argument(string line)
        {
            int assignment = line.IndexOf('=');
			int last1 = line.Length;
			int last2 = line.IndexOf ("//");
			int last3 = line.IndexOf ("#");
			int last = last1;
			if (last2 >= 0 && last > last2)
				last = last2;
			if (last3 >= 0 && last > last3)
				last = last3;
			if (assignment >= 0 && last >= 0 && last > assignment)
            {
				return line.Substring(assignment+1, last-assignment-1).Trim ();
            }
            else
            {
                return "";
            }
        }

        private void add_line(string line)
        {

            string name = get_name(line);

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            string val = get_argument(line);

            if (string.IsNullOrEmpty(val))
            {
                return;
            }

            if (is_int(val))
            {
                set_value(name, to_int(val));
            }
            else if (is_float(val))
            {
                set_value(name, to_float(val));
            }
            else if (is_bool(val))
            {
                set_value(name, to_bool(val));
            }
            else
            {
                set_value(name, val);
            }
        }

        string to_lower(string str)
        {
			string lower_str = str.ToLower();

            return lower_str;
        }

        bool is_int(string str)
        {
			int result;
			return int.TryParse(str, out result);
        }

        int to_int(string str)
        {
            int i = 0;
            for (int c = 0; c < str.Length; c++)
            {
                i = i * 10 + str[c] - '0';
            }

            return i;
        }

        bool is_float(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            int dec = str.IndexOf('.');

			return dec >= 0 && is_int(str.Substring(0, dec)) && is_int(str.Substring(dec + 1, str.Length - dec - 1));
        }

        float to_float(string str)
        {
            int deci = str.IndexOf('.');

            float fract = (float)(to_int(str.Substring(0, deci)));
            float dec = (float)(to_int(str.Substring(deci + 1, str.Length - deci - 1)));

            while (dec > 1.0f)
            {
                dec *= 0.1f;
            }

            return fract + dec;
        }

        bool is_bool(string str)
        {
            string lower_str = to_lower(str);

            return string.Equals(lower_str, "true") || string.Equals(lower_str, "false");
        }

        bool to_bool(string str)
        {
            return string.Equals(to_lower(str), "true");
        }

        private void set_value(string name, Value val)
        {
            Value it;
			UnityEngine.Debug.Log ("CONFIGURATION: set_value (" + name + ", " + val.ToString () + ")");
            if(m_variables.TryGetValue(name, out it))
            {
                it.Dispose();
                m_variables.Remove(name);
            }

            m_variables.Add(name, val);
        }

        private Value get_value(string name)
        {
			UnityEngine.Debug.Log ("CONFIGURATION: get_value (" + name + ", " + (m_variables.ContainsKey(name) ? m_variables[name].ToString () : "null") + ")");
            if(m_variables.ContainsKey(name))
            {
                return m_variables[name];
            } else
            {
                return null;
            }
        }
			
        public static void parse_file(string filename)
        {
			UnityEngine.Debug.LogWarning ("Configuration: parsing " + filename);
            using(FileStream fs = File.OpenRead(filename))
            {
				if (fs != null && fs.CanRead) {
					using (StreamReader file = new StreamReader (fs)) {
						string line = "";
						while (!file.EndOfStream) {
							line = file.ReadLine ();
							if (!string.IsNullOrEmpty (line)) {
								get_instance ().add_line (line);
							}
						}
					}
				} else {
					UnityEngine.Debug.LogError ("Can't open file " + filename);
				}
            }
        }

        public static void set_value(string name, string val)
        {
            get_instance().set_value(name, new Value(val));
        }
        public static void set_value(string name, int val)
        {
            get_instance().set_value(name, new Value(val));
        }
        public static void set_value(string name, float val)
        {
            get_instance().set_value(name, new Value(val));
        }
        public static void set_value(string name, bool val)
        {
            get_instance().set_value(name, new Value(val));
        }

        public static int get_int(string name)
        {
			Value val = get_instance ().get_value (name);
			return val == null ? -1 : val.get_int();
        }
        public static float get_float(string name)
        {
			Value val = get_instance ().get_value (name);
			return val == null ? float.MinValue : val.get_float();
        }
        public static bool get_bool(string name)
        {
			Value val = get_instance ().get_value (name);
			return val == null ? false : val.get_bool();
        }
        public static string get_string(string name)
        {
			Value val = get_instance ().get_value (name);
			return val == null ? string.Empty : val.get_string();
        }
    }
}
