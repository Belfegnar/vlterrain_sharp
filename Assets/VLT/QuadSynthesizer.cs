//#define PREP
using System.Collections;
using System.Collections.Generic;
using System;
using CGLA;

namespace VLT
{
	public class QuadSynthesizer : IDisposable {

		static int m_next_id = 0x10000000;// FIXME ?!?

		Material material;

		int get_next_id()
		{
			return m_next_id++;
		}

		HeightMap create_base_map(QuadNode parent, int child_no)
		{
			HeightMap parent_heightmap = parent.get_heightmap().get_base_map();

			Vec2d origin = parent_heightmap.get_origin();
			float size = (parent.get_size() - 1) * parent.get_spacing() * 0.5f;
			Vec2i offset = new Vec2i(0,0);

			if (child_no == 1 || child_no == 2)
			{
				origin[0] += size;
				offset[0] += parent_heightmap.get_size() - 1;
			}
			if (child_no == 2 || child_no == 3)
			{
				origin[1] += size;		
				offset[1] += parent_heightmap.get_size() - 1;
			}

			HeightMap heightmap = HeightMap.Create(parent_heightmap.get_size(), parent_heightmap.get_spacing() * 0.5f, origin );

			for (int y = -1; y <= heightmap.get_size(); y++)
			{
				for (int x = -1; x <= heightmap.get_size(); x++)
				{
					int offset_x = x + offset[0];
					int offset_y = y + offset[1];

					heightmap.set_height(m_subdivider.subdivide(offset_x, offset_y, parent_heightmap), x, y);
				}
			}

			return heightmap;
		}

		bool m_add_material;

		KobbeltSubdivider m_subdivider;

		public QuadSynthesizer()
		{
			float subdivision_factor = 1.0f;
		    m_add_material = true;

		    // FIXME!!!
		#if !PREP
		    m_add_material = Configuration.get_bool("detail_add_material");
		    subdivision_factor = Configuration.get_float("detail_subdivision_factor");

			int material_type = Configuration.get_int("detail_material_type");
			
			switch (material_type)
			{
			case 0:
				material = new TestMaterial(3.0f, 1.0f);
				break;

			case 1:
				material = new TestMaterial(3.0f, 1.0f);//TestMaterial1(3.0f, 1.0f);
				break;
			}

		#else
			material = new TestMaterial(3.0f, 1.0f);

		#endif

		    m_subdivider = new KobbeltSubdivider(subdivision_factor);
		}

		public void Dispose()
		{
			m_subdivider.Dispose ();
		}

		public QuadInfo create_info(QuadNode parent, int child_no)
		{
			HeightMap base_map = create_base_map(parent, child_no);

			int id = get_next_id();
			parent.set_child_id(child_no, id);

			return new QuadInfo(id, parent.get_error() * 0.55f, parent.get_skirt_depth() * 0.55f, parent.get_texture_origin(),
				parent.get_texture_size(), base_map, null, null, null);
		}

		public HeightMap create_detail_map(HeightMap heightmap) // FIXME?!?
		{
			HeightMap detailmap = HeightMap.Create(heightmap.get_size(), heightmap.get_spacing(), heightmap.get_origin());

			for (int y = -1; y <= detailmap.get_size(); y++)
			{
				for (int x = -1; x <= detailmap.get_size(); x++)
				{
					double fx = x * detailmap.get_spacing() + detailmap.get_origin()[0];
					double fy = y * detailmap.get_spacing() + detailmap.get_origin()[1];

					if (m_add_material)
					{
						detailmap.set_height(material.get_displacement(fx, fy), x, y);
					}
					else
					{
						detailmap.set_height(0, x, y);
					}
				}
			}

			return detailmap;
		}

		public Material get_material()
		{
			return material;
		}
	}
}