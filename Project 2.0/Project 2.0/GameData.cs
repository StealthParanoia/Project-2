using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_2._0
{
	public class GameData
	{
		public GameSettings Settings = new GameSettings();

		public List<Tank> Tanks = new List<Tank>();
		public List<Shell> Shells = new List<Shell>();
	}
}
