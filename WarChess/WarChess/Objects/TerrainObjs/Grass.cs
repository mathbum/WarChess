using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.TerrainObjs {
	class Grass : Terrain {
		public Grass() {
			Standable = true;
			SeeThrough = true;
			Shootable = true;
			Jumpable = false;
			Speed = 1;
		}
	}
}
