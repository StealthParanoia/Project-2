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

		public List<Ship> Ships = new List<Ship>();
		public List<Cannonball> Cannonballs = new List<Cannonball>();
	}
}
