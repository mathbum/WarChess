using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.Items;

namespace WarChess.Objects {
	public static class Config {		
		public enum Allegiance { Good, Evil, Neutral };
		public static List<string> GetUnitNames(Dictionary<string, UnitPair> dict) {
			List<string> UnitNames = new List<string>();
			List<KeyValuePair<string, UnitPair>> dictList = dict.ToList();
			for (int i = 0; i < dictList.Count; i++) {
				UnitNames.Add(dictList[i].Key);
			}
			return UnitNames;
		}
		public static List<char> GetTerrainKeys() {
			List<char> TerrainKeys = new List<char>();
			List<KeyValuePair<char, Terrain>> dictList = TerrainObjs.ToList();
			for (int i = 0; i < dictList.Count; i++) {
				TerrainKeys.Add(dictList[i].Key);
			}
			return TerrainKeys;
		}

		public struct UnitPair{
			public Unit unit;
			public Dictionary<Item, int> CompatableItems;
			public UnitPair(Unit unit, Dictionary<Item, int> CompatableItems) {
				this.unit = unit;
				this.CompatableItems = CompatableItems;
			}
		}
		public static Dictionary<string, Item> Items = new Dictionary<string, Item> {
			{"Orc Bow",new RangedWeapon("Orc Bow", 9, 2, .5) },
			{"Elven Bow", new RangedWeapon("Elven Bow", 12, 3, .5) },
			{"Crossbow", new RangedWeapon("Crossbow", 9, 4, 1) },
			{"Throwing Axe", new RangedWeapon("Throwing Axe", 2, 3, 0) },
			{"Shield", new DefensiveItem("Shield", true, 1) },
			{"Spear",new HandWeapon("Spear", false, 1) },
			{"Pike", new HandWeapon("Pike", false, 2) },//is this a two handed weapon?
			{"Elven Spear", new HandWeapon("Elven Spear", false, 1) },//this can be set to two handed or not, player choice
			{"Armour",new DefensiveItem("Armour", false,1) },
			{"Heavy Armour",new DefensiveItem("Heavy Armour", false, 2) }
			//lance
		};
		
		//name,points,width,length,allegiance,fighting,shootingprofeciency,strength,defense,attacks,wounds,mights,wills,fates
		public static Dictionary<string, UnitPair> Units = new Dictionary<string, UnitPair> {
			{ "Goblin", new UnitPair(new Unit("Goblin",4,1,1,Allegiance.Evil,2,5,3,4,1,1,0,0,0), new Dictionary<Item, int> {{ Items["Orc Bow"], 1}, {Items["Armour"], 1}} ) },
			//{ "Orc", new Unit("Orc",7,1,1,Allegiance.Evil,3,3,5,1,1,0,0,0)},
			{ "Warrior", new UnitPair(new Unit("Warrior",7,1,1,Allegiance.Good,3,4,3,5,1,1,0,0,0), new Dictionary<Item, int> {{Items["Orc Bow"],1}} ) },
			{"Wood Elf", new UnitPair(new Unit("Wood Elf",7,1,1,Allegiance.Good,6,3,3,3,1,1,0,0,0), new Dictionary<Item, int> {{Items["Elven Bow"],2}, {Items["Armour"],2}} ) },
			{"Uruk-hai Captain", new UnitPair(new Unit("Uruk-hai Captain",25,1,1,Allegiance.Evil,5,4,5,5,2,2,0,0,0), new Dictionary<Item, int> { {Items["Orc Bow"], 5 }, { Items["Crossbow"],5}, {Items["Heavy Armour"],5}} ) }
		};//TODO DO I HAVE AN EXTRA INSTANCE OF EVERY UNIT?
		public static Unit NullUnit = new Unit("", 0, 0, 0, Allegiance.Neutral, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		public static Dictionary<char, Terrain> TerrainObjs = new Dictionary<char, Terrain> {
			{' ',new Terrain("Grass","grasss.png",true,true,false,1) },
			{'u',new Terrain("Short Wall","ShortWalls.png",false,true,true,1) }
		};//probably going to have to make this like nullunit so you can tell terrain types
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
		public static Dictionary<string, List<string>> Boards = new Dictionary<string, List<string>>() {//TODO maybe move this to files? this probably means all boards are loaded to memory
			{"standard",new List<string>() {
											"     ",
											"  u  ",
											"u    ",
											"     "}},
			{"long",new List<string>() {
				                        "          ",
				                        "  u       ",
				                        "u         ",
				                        "          ",
				                        "uuuuu  uu ",
				                        "          ",
				                        "          ",}},
			{"ten",new List<string>() {
										"          ",
										"  u       ",
										"u         ",
										"      u   ",
										"        u ",
										"          ",
										"   u      ",
										"          ",
										"     u    ",
										"          ",}}
		};		
	}	
	//public class NullUnit : Unit { public NullUnit() : base("", 0, 0, 0, Config.Allegiance.Neutral, 0, 0, 0, 0, 0, 0, 0, 0) { } }
}