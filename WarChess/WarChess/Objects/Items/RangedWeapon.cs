using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.Items {
	public class RangedWeapon : Item {
		public int Range;
		public int Strength;
		public double MovementCost;//measured in %
		public bool IsThrowable;//can be even thrown in movement phase.... this might be fun

		public RangedWeapon(string Name, int Range, int Strength, double MovementCost) {
			this.Name = Name;
			this.Range = Range;
			this.Strength = Strength;
			this.MovementCost = MovementCost;
		}
	}
}
