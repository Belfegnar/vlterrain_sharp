using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLT
{
	public class Program : MonoBehaviour
    {

		public bool debugGizmos = true;
		public string[] arguments;

		public static Program instance;
		public uint currentFrame = 0;
		public List<Texture2D> images = new List<Texture2D> ();

		[SerializeField]Engine _engine;

		public void Awake()
		{
			instance = this;
			M (arguments);
		}

        public void M(string[] args)
        {
            Configuration.parse_file("tengine.cfg");

            string map_file = string.Empty;
            if (args != null && args.Length > 0)
            {
                for (int a = 0; a < args.Length; a++)
                {
                    string input = args[a];
                    if (input.Substring(input.Length - 4, 4) == ".cfg")
                    {
                        Configuration.parse_file(input);
                    }
                    else
                    {
                        map_file = args[a];
                    }
                }
            }

			Debug.Log (map_file);
            if (string.IsNullOrEmpty(map_file))
            {
                map_file = Configuration.get_string("map_file");
            }

			_engine = new Engine (map_file,
				Configuration.get_int ("screen_width"),
				Configuration.get_int ("screen_height"),
				Configuration.get_bool ("screen_fullscreen"));

			Update ();
            return;
        }

		void Update()
		{
			if (_engine != null) {
				_engine.run ();
				_engine.input (null, null, Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
				_engine.render ();
				currentFrame++;
			}
		}

		void OnGUI()
		{
			if (_engine != null) {
				_engine.draw_interface();
			}
		}

		void OnDrawGizmos()
		{
			if (_engine != null && debugGizmos) {
				_engine.draw_gizmos ();
			}
		}
    }
}
