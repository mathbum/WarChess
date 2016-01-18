using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.Items {
	public class DefensiveItems: Item {
		public bool IsShield;
		public int DefenseBoost;
		public DefensiveItems(bool IsShield, int DefenseBoost) {
			this.IsShield = IsShield;
			this.DefenseBoost = DefenseBoost;
		}
	}
}
