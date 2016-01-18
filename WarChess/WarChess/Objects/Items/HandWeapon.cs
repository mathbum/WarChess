using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.Items {
	public class HandWeapon : Item{
		public bool IsTwoHanded;
		public int SupportDist;

		public HandWeapon(string Name, bool IsTwoHanded, int SupportDist) {
			this.Name = Name;
			this.IsTwoHanded = IsTwoHanded;
			this.SupportDist = SupportDist;
		}
	}
}
