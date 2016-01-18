using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Player {
		
		public Player(string Name) {
			this.Name = Name;
		}
		public string Name { get; private set; }
		//maybe give it a dict that has unitstoplace (give a unit obj instead of string)
		//possibly socket etc..
	}
}
