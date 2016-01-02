﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Game {//static?
		public enum Phases { Priority, Move, Shoot, Fight };//need end phase?		

		public Phases Phase { get; set; }
		public Board Board { get; set; }

		public Phases NextTurn() {
			Phases[] vals = (Phases[]) Enum.GetValues(typeof(Phases));

			if (vals[vals.Length-1] == Phase) {
				Phase = vals[0];
				return Phase;
			}
			for(int i = 0; i < vals.Length; i++) {
				if (vals[i]==Phase) {
					Phase = vals[i + 1];
					return Phase;
				}
			}

			throw new ArgumentException();
			//THIS BETTER NEVER HAPPEN
			//return Phase;
		}
		//movement functions should be here
		//vertifying a valid movement should be done here
	}
}
