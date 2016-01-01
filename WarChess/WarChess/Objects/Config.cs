using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public static class Config {
		public static Dictionary<string, Unit> GoodUnits = new Dictionary<string, Unit> {
			{ "Orc", new Unit("Orc",7,1,1,Allegiance.Evil,3,5,1,1,0,0,0)},
			{ "Warrior",new Unit("Warrior",9,1,1,Allegiance.Good,3,5,1,1,0,0,0)}
		};
		public enum Allegiance { Good,Evil,Neutral};
		public enum Phases { Priority,Move,Shoot,Fight};//need end phase?
	}
}