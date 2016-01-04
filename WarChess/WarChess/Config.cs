﻿using System;
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
		public static List<string> GetUnitNames(Dictionary<string, Unit> dict) {
			List<string> UnitNames = new List<string>();
			List<KeyValuePair<string, Unit>> dictList = dict.ToList();
			for (int i=0;i< dictList.Count; i++) {
				UnitNames.Add(dictList[i].Key);
			}
			return UnitNames;
		}
	}
}