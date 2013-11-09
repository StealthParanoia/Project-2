using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_2._0
{
    public partial class GameControl : UserControl
    {

        public GameControl()
        {
            InitializeComponent();
        }

        private GameData _data;
        public GameData Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public void Activate()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_data == null) { return; }

            foreach (var tank in _data.Tanks)
            {
                DrawTank(e.Graphics, tank, tank.DColour, tank.FColour);
            }
        }

        private void DrawTank(Graphics g, Tank t, Pen dc, Brush fc)
        {
            g.TranslateTransform(t.X, t.Y);
            g.RotateTransform(t.Angle);

            var tankChassis = new Rectangle(-20, -20, 40, 45);
            g.DrawRectangle(dc, tankChassis);
            g.FillRectangle(fc, tankChassis);

            var tankTurret = new Rectangle(-20, -20, 40, 40);
            g.DrawEllipse(Pens.Red, tankTurret);
            g.FillEllipse(Brushes.Red, tankTurret);

            var tankNozzle = new Rectangle(0, 20, 2, 30);
            g.DrawRectangle(Pens.Blue, tankNozzle);
            g.FillRectangle(Brushes.Blue, tankNozzle);

            g.ResetTransform();
        }

        private void MoveTank()
        {
            // code to move tank
        }

        private void GameControl_Load(object sender, EventArgs e)
        {

        }

        private void GameControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                _data.Tanks[2].Angle -= 5;

                if (_data.Tanks[2].Angle < 0)
                {
                    _data.Tanks[2].Angle += 360;
                }

                this.Invalidate();
            }

            if (e.KeyCode == Keys.Right)
            {
                _data.Tanks[2].Angle += 5;

                if (_data.Tanks[2].Angle > 360)
                {
                    _data.Tanks[2].Angle -= 360;
                }

                this.Invalidate();
            }

            if (e.KeyCode == Keys.Space)
            {
                if (_data.Tanks[2].Angle == 90)
                {
                    foreach (var tank in _data.Tanks)
                    {
                        if (tank.X == _data.Tanks[2].X - 100 || tank.X == _data.Tanks[2].X + 100)
                        {
                            tank.Health -= 10;
                        }

                        if (tank.Health == 0)
                        {
                            tank.DColour = System.Drawing.Pens.Black;
                            tank.FColour = System.Drawing.Brushes.Black;
                        }
                    }
                }


                this.Invalidate();
            }
        }

        private void GameControl_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}