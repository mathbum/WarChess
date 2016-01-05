using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Player {
		
		public Player(string Name,List<KeyValuePair<string, int>> UnitsToPlace) {
			this.Name = Name;
			this.UnitsToPlace = UnitsToPlace;
		}
		public string Name { get; private set; }
		public List<KeyValuePair<string, int>> UnitsToPlace { get; set; }
		//possibly socket etc..

	}
}
