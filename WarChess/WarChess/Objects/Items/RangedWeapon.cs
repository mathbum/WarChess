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

		public RangedWeapon(int Range, int Strength, double MovementCost) {
			this.Range = Range;
			this.Strength = Strength;
			this.MovementCost = MovementCost;

		}
	}
}
