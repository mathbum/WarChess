using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Unit {
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
		}

		public string Name { get; private set; }
		public int Points { get; private set; }
		public int Width { get; private set; }
		public int Length { get; private set; }
		public Config.Allegiance Allegiance { get; private set; }
		//fighting? first is handtohand fighting second is minimum roll to land a hit with range
		public int Strength { get; private set; }
		public int Defense { get; private set; }
		public int Attack { get; private set; }
		public int Wounds { get; set; }
		//public int Courage { get; set; }
		public int Might { get; set; }
		public int Will { get; set; }
		public int Fate { get; set; }
	}

}
