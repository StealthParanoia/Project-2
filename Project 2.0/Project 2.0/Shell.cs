using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2._0
{
	public class Cannonball
	{
		public float X, Y, Angle, Speed, Life;
		public Ship Ship;
		public Polygon cannonballPolygon = new Polygon();

		public Cannonball(float x, float y, float angle)
		{
			X = x;
			Y = y;
			Angle = angle;

			cannonballPolygon.Points.Add(new Vector(-1, -1));
			cannonballPolygon.Points.Add(new Vector(1, -1));
			cannonballPolygon.Points.Add(new Vector(1, 1));
			cannonballPolygon.Points.Add(new Vector(-1, 1));
			cannonballPolygon.Offset(x, y);
			cannonballPolygon.BuildEdges();

		}

		public Cannonball() { }

		public void MoveCannonball(Vector v)
		{
			X += v.X;
			Y += v.Y;

			cannonballPolygon.Offset(v.X, v.Y);

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
