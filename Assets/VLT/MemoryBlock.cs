using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace VLT
{
	public class MemoryBlock : IDisposable {

		BlockType m_type;

		int[] indices;
		Vector3[] vertices;
		Vector2[] uvs;
		Color[] colors;

		public MemoryBlock(BlockType type)
		{
			m_type = type;
		}

		public virtual void Dispose(){
			indices = null;
			vertices = null;
			uvs = null;
			colors = null;
		}

		public virtual void set_data(int[] data, int size)
		{
			indices = new int[(data.Length)*3];
			tstrip = data;
			list = indices;
			buildList (data [1], data [0]);
			indices = list;
		}

		public virtual void set_data(UnityEngine.Vector2[] data, int size)
		{
			uvs = data;
		}

		public virtual void set_data(UnityEngine.Color[] data, int size)
		{
			colors = data;
		}

		public virtual void set_data(float[] data, int size)
		{
			if (m_type == BlockType.DisplacementBlock) {
				if (uvs == null || uvs.Length != data.Length / 2)
					uvs = new Vector2[data.Length / 2];

				Vector2 v;
				for (int i = 0; i < uvs.Length; i++) {
					v.x = data [i * 2];
					v.y = data [i * 2+1];
					uvs [i] = v;
				}
			}
			if (m_type == BlockType.VertexBlock) {
				if (vertices == null || vertices.Length != data.Length / 3)
					vertices = new Vector3[data.Length / 3];

				Vector3 v;
				for (int i = 0; i < vertices.Length; i++) {
					v.x = data [i * 3];
					v.y = data [i * 3+1];
					v.z = data [i * 3+2];
					vertices [i] = v;
				}
			}
			if (m_type == BlockType.LightBlock) {
				if (colors == null || colors.Length != data.Length / 4)
					colors = new Color[data.Length / 4];

				Color v;
				for (int i = 0; i < colors.Length; i++) {
					v.r = data [i * 4];
					v.g = data [i * 4+1];
					v.b = data [i * 4+2];
					v.a = data [i * 4+3];
					colors [i] = v;
				}
			}
		}

		public virtual void bind(Mesh mesh)
		{
			if (m_type == BlockType.DisplacementBlock) {
				mesh.uv = uvs;//(0, new List<Vector2>(uvs));//TODO WTF, UNITY? WHY DO U NEED A LIST???
			}
			if (m_type == BlockType.VertexBlock) {
				mesh.vertices = vertices;
			}
			if (m_type == BlockType.LightBlock) {
				mesh.colors = colors;
			}
			if (m_type == BlockType.ElementBlock) {
				mesh.triangles = indices;
			}
		}

		static int[] tstrip;
		static int[] list;

		void buildList(int p, int gp, int trion=0, bool side=true, int i=2)
		{
			for (; i < tstrip.Length-2; i++) {
				if (tstrip [i] == gp) {
					side = !side;
					gp = p;
					p = tstrip [i];
				} else {
					if(side)  // clockwise (if default)
					{
						list[trion*3] = p;
						list[trion*3+1] = gp;
						list[trion*3+2] = tstrip[i];
					}
					else    //  cc
					{
						list[trion*3] = gp;
						list[trion*3+1] = p;
						list[trion*3+2] = tstrip[i];
					}
					side = !side;
					trion++;
					gp = p;
					p = tstrip[i];
				}
			}
		}

		public virtual void get_pointer(){}

		public enum BlockType : byte
		{
			DisplacementBlock,
			ElementBlock,
			LightBlock,
			VertexBlock
		};
	}
}