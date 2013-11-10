using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2._0
{
	public class Shell
	{
		public float X, Y, Angle, Speed, Life;
		public Tank Tank;
		public Polygon shellPolygon = new Polygon();

		public Shell(float x, float y, float angle)
		{
			X = x;
			Y = y;
			Angle = angle;

			shellPolygon.Points.Add(new Vector(-1, -1));
			shellPolygon.Points.Add(new Vector(1, -1));
			shellPolygon.Points.Add(new Vector(1, 1));
			shellPolygon.Points.Add(new Vector(-1, 1));
			shellPolygon.Offset(x, y);
			shellPolygon.BuildEdges();

		}

		public Shell() { }

		public void MoveShell(Vector v)
		{
			X += v.X;
			Y += v.Y;

			shellPolygon.Offset(v.X, v.Y);

		}

		public Vector GetMovementVector(float secs)
		{
			double sine = Math.Sin(Angle * (Math.PI / 180F));
			double cosine = Math.Cos(Angle * (Math.PI / 180F));
			float time = secs;

			return new Vector((float)-sine * secs * Speed, (float)cosine * secs * Speed);
		}

	}
}
