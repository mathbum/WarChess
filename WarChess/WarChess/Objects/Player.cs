using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Player {
		
		public Player(string Name,Dictionary<string, int> UnitsToPlace) {
			this.Name = Name;
			this.UnitsToPlace = UnitsToPlace;
		}
		public string Name { get; private set; }
		public Dictionary<string, int> UnitsToPlace { get; set; }
		//possibly socket etc..
		public bool HasUnitLeftToPlace(string unitName) {
			return UnitsToPlace.ContainsKey(unitName);//if it is in here it should have at least 1
		}
		public bool HasAnyUnitsLeftToPlace() {
			return UnitsToPlace.Count > 0;
		}
	}
}
