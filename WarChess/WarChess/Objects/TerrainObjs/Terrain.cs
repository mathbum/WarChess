using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.TerrainObjs {
	public abstract class Terrain{
		public bool Standable;
		public bool SeeThrough;
		public bool Shootable;
		public bool Jumpable;
		public int Speed;
	}
}
