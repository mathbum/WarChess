using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Unit{

		//protected Unit() {}

		public Unit(string Name,int Points,int Width,int Length,Config.Allegiance Allegiance, int Strength,int Defense,int Attack,int Wounds,int Might,int Will,int Fate) {
			this.Name = Name;
			this.Points = Points;
			this.Width = Width;
			this.Length = Length;
			this.Allegiance = Allegiance;
			this.Strength = Strength;
			this.Defense = Defense;
			this.Attack = Attack;
			this.Wounds = Wounds;
			this.Might = Might;
			this.Will = Will;
			this.Fate = Fate;
		}//TODO add range distace
		//TODO add max move dist
		//TODO add dist left?
		//TODO add max move dist while shootable
		public Player Player { get; set; }
		public string Name { get; protected set; }
		public int Points { get; protected set; }
		public int Width { get; protected set; }
		public int Length { get; protected set; }
		public Config.Allegiance Allegiance { get; protected set; }
		//TODO fighting? first is handtohand fighting second is minimum roll to land a hit with range
		public int Strength { get; protected set; }
		public int Defense { get; protected set; }
		public int Attack { get; protected set; }
		public int Wounds { get; set; }
		//TODO public int Courage { get; set; }
		public int Might { get; set; }
		public int Will { get; set; }
		public int Fate { get; set; }
	}
}
