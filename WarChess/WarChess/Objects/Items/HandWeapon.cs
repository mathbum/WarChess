using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.Items {
	public class HandWeapon : Item{
		public bool IsTwoHanded;
		public int SupportDist;

		public HandWeapon(bool IsTwoHanded, int SupportDist) {
			this.IsTwoHanded = IsTwoHanded;
			this.SupportDist = SupportDist;
		}
	}
}
