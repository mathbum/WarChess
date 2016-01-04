using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.TerrainObjs {
	class TallWall : Terrain{
		public TallWall() {
			Standable = false;
			SeeThrough = false;
			Shootable = false;
			Jumpable = false;
			Speed = 0;
		}
		//make a private get and set?
		//add length and width?
	}
}
