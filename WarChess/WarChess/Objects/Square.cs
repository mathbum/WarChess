using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.TerrainObjs;

namespace WarChess.Objects {
	public class Square {//TODO shouldn't have null units or terrain. nullpointexcept waiting to happen
		public Square(Unit unit) {
			this.Unit = unit;
		}
		public Terrain Terrain { get; set; }
		public Unit Unit { get; set; }
	}
}