using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLT
{
	[System.Serializable]
	public class UserInterface : System.IDisposable {

		Render m_render;
//		RenderState m_render_state;

		[SerializeField]List<StringTime> m_lines = new List<StringTime>();
		[SerializeField]List<StringTime> m_log = new List<StringTime>();

		[SerializeField]bool m_draw_interface;

		public UserInterface(Render render)
		{
			m_render = render;

			//m_render_state = m_render.create_state(RenderState::UIState);

			StringTime p = new StringTime("", 0.0f);
			for (int i = 0; i < 40; i++)
				m_lines.Add (p);

			m_draw_interface = Configuration.get_bool("draw_user_interface");
		}

		public void Dispose()
		{
//			delete m_render_state;
		}

		public void update()
		{	
			float now = Time.realtimeSinceStartup;

			while (m_log.Count > 15)
			{
				m_log.RemoveAt(0);
			}
				
			for (int i = 0; i < m_log.Count && i >= 0;)
			{
				StringTime it = m_log [i];
				if (it.second < now)
				{
					m_log.Remove(it);
					i--;
				}
				else
				{
					i++;
				}
			}

			for (int s = 0; s < m_lines.Count; s++)
			{
				if (m_lines[s].second > 0 && m_lines[s].second < now)
				{
					m_lines[s] = new StringTime("", 0);
				}
			}
		}

		public void draw()
		{
			if (!m_draw_interface)
			{
				return;
			}

			//m_render_state.enable();

			for (int s = 0; s < m_lines.Count; s++)
			{
				if (!string.IsNullOrEmpty(m_lines[s].first))
				{
					GUI.Label (new Rect (10, (s + 1) * 16, 200, 20), m_lines [s].first);
				}
			}

			int line = 0;

			foreach (StringTime it in m_log)
			{
				GUI.Label (new Rect (10, 300 + line * 16, 200, 20), it.first);

				++line;
			}

			//m_render_state.disable();
		}

		public void write(int line, string text, float timeout = 0)
		{
			if (timeout > 0)
			{
				timeout += Time.realtimeSinceStartup;
			}

			m_lines[line] = new StringTime(text, timeout);
		}

		public void append(int line, object arg)
		{
			m_lines[line] = new StringTime( m_lines[line].first + " " + arg.ToString(), m_lines[line].second);
		}

		public void log(string text)
		{
			m_log.Add(new StringTime(text, Time.realtimeSinceStartup + 5));
		}
	}

	public struct StringTime
	{
		public string first;
		public float second;

		public StringTime(string first, float second)
		{
			this.first = first;
			this.second = second;
		}
	}
}