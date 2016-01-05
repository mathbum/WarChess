using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Config { //TODO make this staic. 
						  //TODO figure out how to make adding units easier
		//public static class Config {
		public static Dictionary<string, Unit> GoodUnits = new Dictionary<string, Unit> {
			{ "Orc", new Unit("Orc",7,1,1,Allegiance.Evil,3,5,1,1,0,0,0)},
			{ "Warrior",new Unit("Warrior",9,1,1,Allegiance.Good,3,5,1,1,0,0,0)}
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

		//public List<string> UnitOptions = new List<string>() { "Orc", "Warrior" };
		public Unit makeguy(string guy) {
			Unit tempUnit = GoodUnits[guy];
			string Name = guy;
			int Points = tempUnit.Points;
			int Width = tempUnit.Width;
			int Length = tempUnit.Length;
			Config.Allegiance Allegiance = tempUnit.Allegiance;
			int Strength = tempUnit.Strength;
			int Defense = tempUnit.Defense;
			int Attack = tempUnit.Attack;
			int Wounds = tempUnit.Wounds;
			int Might = tempUnit.Might;
			int Will = tempUnit.Will;
			int Fate = tempUnit.Fate;
			return new Unit(Name, Points, Width, Length, Allegiance, Strength, Defense, Attack, Wounds, Might, Will, Fate);
			//switch (guy) {
			//	case ("Orc"):
			//		return new Orc();
			//	case ("Warrior"):
			//		return new Warrior();				
			//	default:
			//		throw new ArgumentException();
			//}
		}
	}
	//public class Orc : Unit { public Orc() : base("Orc", 7, 1, 1, Config.Allegiance.Evil, 3, 5, 1, 1, 0, 0, 0) { } }
	//public class Warrior : Unit { public Warrior() : base("Warrior", 9, 1, 1, Config.Allegiance.Good, 3, 5, 1, 1, 0, 0, 0) { } }
	//public class NullUnit : Unit {public NullUnit() : base("blank", 0, 0, 0, Config.Allegiance.Neutral, 0, 0, 0, 0, 0, 0, 0) { }}
}