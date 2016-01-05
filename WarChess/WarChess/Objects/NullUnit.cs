using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class NullUnit : Unit {
		public NullUnit(): base("blank", 0, 0, 0, Config.Allegiance.Neutral, 0, 0, 0, 0, 0, 0, 0){}
	}
}
