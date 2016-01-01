using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.Terrain {
	abstract class Terrain {
		public bool Standable;
		public bool SeeThrough;
		public bool Shootable;
		public bool Jumpable;
		public int Speed;
	}
}
