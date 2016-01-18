using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.Items {
	public class DefensiveItem: Item {
		public bool IsShield;
		public int DefenseBoost;
		public DefensiveItem(string Name, bool IsShield, int DefenseBoost) {
			this.Name=Name;
            this.IsShield = IsShield;
			this.DefenseBoost = DefenseBoost;
		}
	}
}
