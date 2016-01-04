using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.TerrainObjs {
	class ShortWall : Terrain {
		public ShortWall() {
			Standable = true;
			SeeThrough = true;
			Shootable = true;
			Jumpable = true;
			Speed = 1;
		}
	}
}
