using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public static class Config { 
		public static Dictionary<string, Unit> Units = new Dictionary<string, Unit> {
			{ "Goblin", new Unit("Goblin",4,1,1,Allegiance.Evil,2,3,5,1,1,0,0,0)},
			//{ "Orc", new Unit("Orc",7,1,1,Allegiance.Evil,3,3,5,1,1,0,0,0)},
			{ "Warrior",new Unit("Warrior",9,1,1,Allegiance.Good,3,3,5,1,1,0,0,0)}
		};
	//	public static List<List<int>> WoundChart = new List<List<int>>() {
	//		//   DEFENSE         1   2   3   4   5   6   7   8   9  10
	//			new List<int>(){ 4,  5,  5,  6,  6, 64, 65, 66, -1, -1}, /*1*/
	///*S*/		new List<int>(){ 4,  4,  5,  5,  6,  6, 64, 65, 66, -1}, /*2*/
	///*T*/		new List<int>(){ 3,  4,  4,  5,  5,  6,  6, 64, 65, 66}, /*3*/
	///*R*/		new List<int>(){ 3,  3,  4,  4,  5,  5,  6,  6, 64, 65}, /*4*/
	///*E*/		new List<int>(){ 3,  3,  3,  4,  4,  5,  5,  6,  6, 64}, /*5*/
	///*N*/		new List<int>(){ 3,  3,  3,  3,  4,  4,  5,  5,  6,  6}, /*6*/
	///*G*/		new List<int>(){ 3,  3,  3,  3,  3,  4,  4,  5,  5,  6}, /*7*/
	///*T*/		new List<int>(){ 3,  3,  3,  3,  3,  3,  4,  4,  5,  5}, /*8*/
	///*H*/    	new List<int>(){ 3,  3,  3,  3,  3,  3,  3,  4,  4,  5}, /*9*/
	//    		new List<int>(){ 3,  3,  3,  3,  3,  3,  3,  3,  4,  4}	 /*10*/
	//	};

		public static List<List<double>> WoundChart = new List<List<double>>() {
			//   DEFENSE               1     2      3      4      5      6      7      8      9      10
				new List<double>(){ .5000, .3333, .3333, .1667, .1667, .0833, .0556, .0278, .0000, .0000}, /*1*/
	/*S*/		new List<double>(){ .5000, .5000, .3333, .3333, .1667, .1667, .0833, .0556, .0278, .0000}, /*2*/
	/*T*/		new List<double>(){ .6667, .5000, .5000, .3333, .3333, .1667, .1667, .0833, .0556, .0278}, /*3*/
	/*R*/		new List<double>(){ .6667, .6667, .5000, .5000, .3333, .3333, .1667, .1667, .0833, .0556}, /*4*/
	/*E*/		new List<double>(){ .6667, .6667, .6667, .5000, .5000, .3333, .3333, .1667, .1667, .0833}, /*5*/
	/*N*/		new List<double>(){ .6667, .6667, .6667, .6667, .5000, .5000, .3333, .3333, .1667, .1667}, /*6*/
	/*G*/		new List<double>(){ .6667, .6667, .6667, .6667, .6667, .5000, .5000, .3333, .3333, .1667}, /*7*/
	/*T*/		new List<double>(){ .6667, .6667, .6667, .6667, .6667, .6667, .5000, .5000, .3333, .3333}, /*8*/
	/*H*/    	new List<double>(){ .6667, .6667, .6667, .6667, .6667, .6667, .6667, .5000, .5000, .3333}, /*9*/
	    		new List<double>(){ .6667, .6667, .6667, .6667, .6667, .6667, .6667, .6667, .5000, .5000}  /*10*/
		};	

		public enum Allegiance { Good, Evil, Neutral };
		public static List<string> GetUnitNames(Dictionary<string, Unit> dict) {
			List<string> UnitNames = new List<string>();
			List<KeyValuePair<string, Unit>> dictList = dict.ToList();
			for (int i = 0; i < dictList.Count; i++) {
				UnitNames.Add(dictList[i].Key);
			}
			return UnitNames;
		}
	}
	//public class Orc : Unit { public Orc() : base("Orc", 7, 1, 1, Config.Allegiance.Evil, 3, 5, 1, 1, 0, 0, 0) { } }
	//public class Warrior : Unit { public Warrior() : base("Warrior", 9, 1, 1, Config.Allegiance.Good, 3, 5, 1, 1, 0, 0, 0) { } }
	public class NullUnit : Unit { public NullUnit() : base("", 0, 0, 0, Config.Allegiance.Neutral, 0, 0, 0, 0, 0, 0, 0, 0) { } }
}