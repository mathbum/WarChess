using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Unit {
		public Unit(string Name, int Points, int Width, int Length, Config.Allegiance Allegiance, int Fighting, int ShootingSkill, int Strength, int Defense, int Attacks, int Wounds, int Might, int Will, int Fate) {
			this.Name = Name;
			this.Points = Points;
			this.Width = Width;
			this.Length = Length;
			this.Allegiance = Allegiance;
			this.Fighting = Fighting;
			this.ShootingSkill = this.ShootingSkill;
			this.Strength = Strength;
			this.Defense = Defense;
			this.Attacks = Attacks;
			this.Wounds = Wounds;
			this.Might = Might;
			this.Will = Will;
			this.Fate = Fate;
		}
		 //TODO max move dist while shootable. put this to item
		public Player Player { get; set; }
		public string Name { get; protected set; }
		public int Points { get; protected set; }
		public int Width { get; protected set; }
		public int Length { get; protected set; }
		public Config.Allegiance Allegiance { get; protected set; }
		public int Fighting {get;protected set;} 
		public int ShootingSkill { get; protected set; }
		public int Strength { get; protected set; }
		public int Defense { get; protected set; }
		public int Attacks { get; protected set; }
		public int Wounds { get; set; }
		//TODO public int Courage { get; set; }
		public int Might { get; set; }
		public int Will { get; set; }
		public int Fate { get; set; }

		public int MaxMoveDist { get; private set; } = 3;
		public bool InConflict { get; set; } = false;
		public Position Position { get; set; }
		public int MovementLeft { get; set; } = 3;
		public bool HasShot { get; set; } = false;
	}
}
