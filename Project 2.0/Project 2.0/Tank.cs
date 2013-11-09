using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2._0
{
    public class Tank
    {
        public float X, Y;
        public float Angle;
        public float Speed;
        public float Health = 100;
        public System.Drawing.Pen DColour;
        public System.Drawing.Brush FColour;
        

        public Tank(int x, int y, float angle)
        {
            X = x;
            Y = y;
            Angle = angle;
            DColour = System.Drawing.Pens.Purple;
            FColour = System.Drawing.Brushes.Purple;
        }

        public Tank()
        {
            
        }

        public void MoveTank(float secs)
        {
            double sine = Math.Sin(Angle);
            double cosine = Math.Cos(Angle);
            float time = secs;

            X += (float) cosine * secs * Speed;
            Y += (float) sine * secs * Speed;

        }
    }
}
