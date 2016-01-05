using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Game {//static?
		
		public Game(Board Board,List<Player> players) {
			this.Board = Board;
			List<Player> PlayerOrder = Utils.PickPriority(players);
			this.Players = PlayerOrder;
			this.PlayerTurnIndex = 0;
			this.isSetUp = true;
		}

		public Phases Phase { get; set; }
		public Board Board { get; private set; }
		public bool isSetUp { get; private set; }
		public enum Phases { Priority, Move, Shoot, Fight };//TODO need end phase? 	do i need priotiry phase?
		public List<Player> Players { get; private set; }
		public int PlayerTurnIndex { get; private set; }

		public void Move(Position originalPos, Position newPos) {
			Board.MoveUnit(originalPos, newPos, Players[PlayerTurnIndex]);
		}

		public void EndTurn() {
			if (PlayerTurnIndex == Players.Count - 1) {
				if (isSetUp) {
					isSetUp = false;
					Phase = Phases.Priority;
				} else {
					NextPhase();
					if (this.Phase == Phases.Priority) {
						Players = Utils.PickPriority(Players);
					}
				}
				PlayerTurnIndex = 0;
			} else {
				PlayerTurnIndex += 1 ;
			}			
		}

		public Phases NextPhase() {
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
		}
	}
}
