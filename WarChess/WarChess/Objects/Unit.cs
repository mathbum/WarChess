using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.Items;
//TODO make units directional, so you are more likely to win and do dmg if facing the enemy?
//TODO make more than one unit able to deal dmg in a conflict
namespace WarChess.Objects {
	public class Unit {
		//public Unit(string Name, int BasePoints, int Width, int Length, Config.Allegiance Allegiance, int Fighting, int ShootingSkill, int Strength, int BaseDefense, int Attacks, int Wounds, int Might, int Will, int Fate) {
		public Unit(string Name, int BasePoints, int Width, int Length, Config.Allegiance Allegiance, int Fighting, int ShootingSkill, int Strength, int BaseDefense, int Attacks, int TotalHealth, int Dexterity) {
			this.Name = Name;
			this.BasePoints = BasePoints;
			this.Width = Width;
			this.Length = Length;
			this.Allegiance = Allegiance;
			this.Fighting = Fighting;
			this.ShootingSkill = ShootingSkill;
			this.Strength = Strength;
			this.BaseDefense = BaseDefense;
			this.Attacks = Attacks;
			this.TotalHealth = TotalHealth;
			this.Dexterity = Dexterity;
			this.Health = TotalHealth;
			//this.Wounds = Wounds;
			//this.Might = Might;
			//this.Will = Will;
			//this.Fate = Fate;
		}
		 //TODO max move dist while shootable. put this to item
		public Player Player { get; set; }
		public string Name { get; protected set; }
		public int BasePoints { get; protected set; }
		public int Width { get; protected set; }
		public int Length { get; protected set; }
		public Config.Allegiance Allegiance { get; protected set; }
		public int Fighting {get; protected set;}
		public int ShootingSkill { get; protected set; }
		public int Strength { get; protected set; }
		public int BaseDefense;

		public int Attacks { get; protected set; }
		//public int Wounds { get; set; }
		//public int Might { get; set; }
		//public int Will { get; set; }
		//public int Fate { get; set; }
		//TODO public int Courage { get; set; }
		//TODO public int Energy {get;set;} //this will effect fighting ability strenghth and dexterity
		//TODO public double Stamina {get;set;} //this effects rate that energy is recovered. rate of recovery is also determined by health and remaining stamina (or maybe % of stamina left)
		public int Dexterity { get; protected set; }//this effects ability to dodge dmg, being trapped should reduce dexterity and increase energy cost. (armor will be able to deflect dmg as well as reduce dmg)
		public int TotalHealth { get; protected set; }//this will effect fighting ability 
		public int Health { get; set; }
		
		public int MaxMoveDist { get; private set; } = 3;
		public bool InConflict { get; set; } = false;
		public Position Position { get; set; }
		public int MovementLeft { get; set; } = 3;
		public bool HasShot { get; set; } = false;
		public List<KeyValuePair<Item, int>> EquipItems { get; set; } = new List<KeyValuePair<Item, int>>();//the item and the point value of that item
		public List<KeyValuePair<Item, int>> UnequipItems { get; set; } = new List<KeyValuePair<Item, int>>();

		private void SwitchItem(List<KeyValuePair<Item,int>> list1, List<KeyValuePair<Item, int>> list2, Item item) {
			KeyValuePair<Item, int> itemToRemove = new KeyValuePair<Item, int>();
			foreach(KeyValuePair<Item,int> kvp in list1) {
				if (kvp.Key == item) {
					list2.Add(kvp);
					itemToRemove = kvp;
					break;
				}				
			}
			list1.Remove(itemToRemove);
		}
		public void EquipItem(Item item){
			SwitchItem(UnequipItems, EquipItems, item);
		}
		public void UnequipItem(Item item) {
			SwitchItem(EquipItems, UnequipItems, item);
		}
		public RangedWeapon GetRangedWeapon() {//this assumes a unit can only have 1 ranged weapon
			for(int i = 0; i < EquipItems.Count; i++) {
				Item item = EquipItems[i].Key;
				if (item is RangedWeapon) {
					return (RangedWeapon) item;
				}
			}
			return null;
		}
		public bool HasItem(Item item) {
			foreach (KeyValuePair<Item, int> kvp in EquipItems) {
				if (kvp.Key == item) {
					return true;
				}
			}
			foreach (KeyValuePair<Item, int> kvp in UnequipItems) {
				if (kvp.Key == item) {
					return true;
				}
			}
			return false;
		}
		public void RemoveItemFromEquip(Item item) {
			KeyValuePair<Item, int> itemToRemove = new KeyValuePair<Item, int>();
			foreach (KeyValuePair<Item, int> kvp in EquipItems) {
				if (kvp.Key == item) {
					itemToRemove = kvp;
					break;
				}
			}
			EquipItems.Remove(itemToRemove);
		}

		public int GetDefense() {
			int TotalDefense = this.BaseDefense;
			for (int i = 0; i < EquipItems.Count; i++) {
				Item item = EquipItems[i].Key;
				if (item is DefensiveItem) {
					TotalDefense += ((DefensiveItem)item).DefenseBoost;
				}
			}
			return TotalDefense;
		}

		public int GetPoints() {
			int points = this.BasePoints;
			for(int i = 0; i < EquipItems.Count; i++) {
				points += EquipItems[i].Value;
			}
			for (int i = 0; i < UnequipItems.Count; i++) {
				points += UnequipItems[i].Value;
			}
			return points;
		}
	}
}
