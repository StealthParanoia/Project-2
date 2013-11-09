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

		public Shell(int x, int y, float angle)
		{
			X = x;
			Y = y;
			Angle = angle;
		}

		public Shell() { }

		public void MoveShell(float secs)
		{
			double sine = Math.Sin(Angle * (Math.PI / 180F));
			double cosine = Math.Cos(Angle * (Math.PI / 180F));
			float time = secs;

			X += (float)-sine * secs * Speed;
			Y += (float)cosine * secs * Speed;
		}

	}
}
