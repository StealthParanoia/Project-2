﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2._0
{
	public class Polygon
	{

		private List<Vector> points = new List<Vector>();
		private List<Vector> edges = new List<Vector>();

		public void Rotate(float angle)
		{
			// foreach point, use angle to rotate around origin (0, 0)

			for (int i = 0; i < points.Count; i++)
			{

				var ox = 0;
				var oy = 0;
				var pnt = points[i];
				var px = pnt.X;
				var py = pnt.Y;
		
				// rotate and update value

				var radians = (angle / 180) * Math.PI;

				pnt.X = (float)(Math.Cos(radians) * (px - ox) - Math.Sin(radians) * (py - oy) + ox);
				pnt.Y = (float)(Math.Sin(radians) * (px - ox) + Math.Cos(radians) * (py - oy) + oy);
				points[i] = pnt;
			}

		}

		public void BuildEdges()
		{
			Vector p1;
			Vector p2;
			edges.Clear();
			for (int i = 0; i < points.Count; i++)
			{
				p1 = points[i];
				if (i + 1 >= points.Count)
				{
					p2 = points[0];
				}
				else
				{
					p2 = points[i + 1];
				}
				edges.Add(p2 - p1);
			}
		}

		public List<Vector> Edges
		{
			get { return edges; }
		}

		public List<Vector> Points
		{
			get { return points; }
		}

		public Vector Center
		{
			get
			{
				float totalX = 0;
				float totalY = 0;
				for (int i = 0; i < points.Count; i++)
				{
					totalX += points[i].X;
					totalY += points[i].Y;
				}

				return new Vector(totalX / (float)points.Count, totalY / (float)points.Count);
			}
		}

		public void Offset(Vector v)
		{
			Offset(v.X, v.Y);
		}

		public void Offset(float x, float y)
		{
			for (int i = 0; i < points.Count; i++)
			{
				Vector p = points[i];
				points[i] = new Vector(p.X + x, p.Y + y);
			}
		}

		public override string ToString()
		{
			string result = "";

			for (int i = 0; i < points.Count; i++)
			{
				if (result != "") result += " ";
				result += "{" + points[i].ToString(true) + "}";
			}

			return result;
		}

	}


}
