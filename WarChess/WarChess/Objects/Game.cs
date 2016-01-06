using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			this.IsInSetup = true;
		}

		public Phases Phase { get; set; }
		public Board Board { get; private set; }
		public bool IsInSetup { get; private set; }
		//public enum Phases { Priority, Move, Shoot, Fight };//TODO need end phase? 	do i need priotiry phase?
		public enum Phases { Move};//TODO need end phase? 	do i need priotiry phase?
		private List<Player> Players;// { get; set; }
		private int PlayerTurnIndex;// { get; set; }
		private List<List<Unit>> Conflicts = new List<List<Unit>>();//{ get; set; }

		public bool PlaceUnit(Position position,Unit unit) {
			bool succ = Board.PlaceUnit(position, unit);
			if (succ) {
				Dictionary<string, int> unitstoplace = GetCurrentPlayer().UnitsToPlace;
				if (unitstoplace.ContainsKey(unit.Name)) {//this should always be true. 					
					if (unitstoplace[unit.Name] == 1) {
						GetCurrentPlayer().UnitsToPlace.Remove(unit.Name);
					} else {
						unitstoplace[unit.Name] = unitstoplace[unit.Name] - 1;
					}
				} else {
					throw new ArgumentException();// someone has placed a unit that they aren't allowed to be placing
				}								
			}
			return succ;
		}

		public Player GetCurrentPlayer() {
			return Players[PlayerTurnIndex];
		}
		public Unit CreateUnit(string unitName) {
			Unit tempUnit = Config.Units[unitName];
			string Name = unitName;
			int Points = tempUnit.Points;
			int Width = tempUnit.Width;
			int Length = tempUnit.Length;
			Config.Allegiance Allegiance = tempUnit.Allegiance;
			int Strength = tempUnit.Strength;
			int Defense = tempUnit.Defense;
			int Attack = tempUnit.Attacks;
			int Wounds = tempUnit.Wounds;
			int Might = tempUnit.Might;
			int Will = tempUnit.Will;
			int Fate = tempUnit.Fate;
			return new Unit(Name, Points, Width, Length, Allegiance, Strength, Defense, Attack, Wounds, Might, Will, Fate);
		}

		public bool Move(Position originalPos, Position newPos) {
			return Board.MoveUnit(originalPos, newPos, GetCurrentPlayer());
		}

		public void AddConflict(Unit enemyUnit, Unit playerUnit) {
			for(int i = 0; i < Conflicts.Count; i++) {
				for(int j = 0; j < Conflicts[i].Count; j++) {
					Unit unit = Conflicts[i][j];
					if (unit==enemyUnit) {
						Conflicts[i].Add(playerUnit);
						return;
					}
				}
			}
			Conflicts.Add(new List<Unit>() { enemyUnit, playerUnit });
			Trace.WriteLine("Conflict: " + enemyUnit.Player.Name + ": " + enemyUnit.Name + "--" + playerUnit.Player.Name + ": " + playerUnit.Name);
			//TODO remove the trace
		}

		public void ResolveAllConflicts() {			
			for (int i = 0; i < Conflicts.Count; i++) {
				ResolveConflict(Conflicts[i]);				
			}
			Conflicts.Clear();
		}

		public void ResolveConflict(List<Unit> Conflict) {
			Player Victor = DetermineConflictVictor(Conflict);			
			int wounds = 0;
			Unit enemyUnit=null;
			for (int i = 0; i < Conflict.Count; i++) {//TODO pick who is attacked
				if (Conflict[i].Player != Victor) {
					enemyUnit = Conflict[i];
					break;
				}
			}
			for (int i = 0; i < Conflict.Count; i++) {//sums total wounds to that unit
				if (Conflict[i].Player == Victor) {
					if (Utils.RandomBoolByPercent(Config.WoundChart[Conflict[i].Strength][enemyUnit.Defense])) {
						wounds += 1;
					}					
				}
			}
			enemyUnit.Wounds -= wounds;
			if (enemyUnit.Wounds < 1) {
				Board.KillUnit(enemyUnit);
			}

		}

		public Player DetermineConflictVictor(List<Unit> Conflict) {
			Dictionary<Player, int> PlayerRolls = new Dictionary<Player, int>();
			for (int i = 0; i < Conflict.Count; i++) {//determine number of rolls each player gets
				Unit unit = Conflict[i];
				if (PlayerRolls.ContainsKey(unit.Player)){
					PlayerRolls[unit.Player] = PlayerRolls[unit.Player] + unit.Attacks;
				} else {
					PlayerRolls[unit.Player] = unit.Attacks;
				}
			}

			List<Player> HighestPlayers = new List<Player>();
			int HighestRoll = 0;
			List<KeyValuePair<Player, int>> PlayerRollsList = PlayerRolls.ToList();
			for (int i = 0; i < PlayerRollsList.Count; i++) {//find player(s) with highest roll
				int MaxPlayerRoll = Utils.RollD6(PlayerRollsList[i].Value).Max();//TODO: this should shortcircuit
				if (MaxPlayerRoll == HighestRoll) {
					HighestPlayers.Add(PlayerRollsList[i].Key);
				} else if (MaxPlayerRoll > HighestRoll) {
					HighestRoll = MaxPlayerRoll;
					HighestPlayers.Clear();
					HighestPlayers.Add(PlayerRollsList[i].Key);
				}
			}
			if (HighestPlayers.Count == 1) {
				return HighestPlayers[0];
			}
			//TODO try to resolve fight via fighting stat on units
			int victor = Utils.GenerateRandomInt(HighestPlayers.Count);
			return HighestPlayers[victor];
		}


		public void EndTurn() {
			if (PlayerTurnIndex == Players.Count - 1) {
				if (IsInSetup) {
					IsInSetup = false;
					Phase = Phases.Move;
					//Phase = Phases.Priority;
				} else {
					NextPhase();
					if (this.Phase == Phases.Move) {
					//if (this.Phase == Phases.Priority) {
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
				ResolveAllConflicts();
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
