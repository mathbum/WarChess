using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects.TerrainObjs {
	public class Terrain{		
		public string Name;
		public bool IsStandable;
		public bool SeeThrough;
		//public bool Shootable;
		public bool IsJumpable;
		public int Speed;
		public Terrain(string Name,bool IsStandable,bool SeeThrough/*,bool Shootable*/,bool Jumpable,int Speed) {
			this.Name = Name;
			this.IsStandable = IsStandable;
			this.SeeThrough = SeeThrough;
			//this.Shootable = Shootable;
			this.IsJumpable = Jumpable;
			this.Speed = Speed;
		}
	}
	//TODO make terrain objects just like units. Why have a class for each?
}
