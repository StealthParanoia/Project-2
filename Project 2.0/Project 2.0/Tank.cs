using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2._0
{
	public class Ship
	{
		public float X, Y, Angle, Speed;
		public float Health = 100;
		public float Reload;
		public Pen DColour;
		public SolidBrush FColour;

		public Ship(int x, int y, float angle)
		{
			X = x;
			Y = y;
			Angle = angle;
			DColour = new Pen(Color.FromArgb(13,0,0));
			FColour = new SolidBrush(Color.FromArgb(25,0,0));
		}

		public Ship() { }

		public void MoveShip(float secs)
		{
			double sine = Math.Sin(Angle * (Math.PI / 180F));
			double cosine = Math.Cos(Angle * (Math.PI / 180F));
			float time = secs;

			X += (float)-sine * secs * Speed;
			Y += (float)cosine * secs * Speed;
		}

		public Cannonball FireCannonball(GameSettings settings)
		{

			var cannonball = new Cannonball(this.X, this.Y, this.Angle);
			cannonball.Speed = this.Speed + settings.CannonballSpeed;
			cannonball.Ship = this;
			cannonball.Life = settings.CannonballLife;

			Reload = settings.ReloadTime;

			return cannonball;

		}


	}
}