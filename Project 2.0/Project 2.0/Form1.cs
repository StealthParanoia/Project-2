using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_2._0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private GameData _data;

        private void Form1_Load(object sender, EventArgs e)
        {
            _data = new GameData();
            Tank tank1 = new Tank(140, 200, 20);
            Tank tank2 = new Tank(200, 100, 50);
            Tank userTank = new Tank(300, 100, 90);
            _data.Tanks.Add(tank1);
            _data.Tanks.Add(tank2);
            _data.Tanks.Add(userTank);
            gameControl1.Data = _data;
            gameControl1.Activate();
        }
    }
}
