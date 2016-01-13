using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Game {//static?
		
		public Game(BoardManager BoardManager,List<Player> players) {
			this.BoardManager = BoardManager;
			List<Player> PlayerOrder = Utils.PickPriority(players);
			this.Players = PlayerOrder;
		}

		public Phases Phase { get; set; }
		private BoardManager BoardManager { get; set; }
		public bool IsInSetup { get; private set; } = true;
		//public enum Phases { Priority, Move, Shoot, Fight };//TODO need end phase? 	do i need priotiry phase?
		public enum Phases { Move};//TODO need end phase? 	do i need priotiry phase?
		private List<Player> Players;
		private int PlayerTurnIndex = 0;
		private ConflictManager conflictManager = new ConflictManager();
		private List<KeyValuePair<Position, int>> CurrentMoveOptions;

		public bool PlaceUnit(Position position,Unit unit) {
			bool succ = BoardManager.PlaceUnit(position, unit);
			if (succ) {
				Dictionary<string, int> unitstoplace = GetCurrentPlayer().UnitsToPlace;
				if (unitstoplace.ContainsKey(unit.Name)) {//this should always be true. 					
					if (unitstoplace[unit.Name] == 1) {
						GetCurrentPlayer().UnitsToPlace.Remove(unit.Name);
					} else {
						unitstoplace[unit.Name] = unitstoplace[unit.Name] - 1;
					}
				} else {
					throw new ArgumentException();//someone has placed a unit that they aren't allowed to be placing
				}								
			}
			return succ;
		}
		public int GetBoardRows() {
			return BoardManager.GetRows();
		}
		public int GetBoardColumns() {
			return BoardManager.GetColumns();
		}
		public Unit GetUnitAtPos(Position position) {
			return BoardManager.GetUnitAtPos(position);
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
			int Fighting = tempUnit.Fighting;
			int Strength = tempUnit.Strength;
			int Defense = tempUnit.Defense;
			int Attack = tempUnit.Attacks;
			int Wounds = tempUnit.Wounds;
			int Might = tempUnit.Might;
			int Will = tempUnit.Will;
			int Fate = tempUnit.Fate;
			return new Unit(Name, Points, Width, Length, Allegiance, Fighting, Strength, Defense, Attack, Wounds, Might, Will, Fate);
		}

		public bool Move(Position originalPos, Position newPos) {
			int cost = 0;
			for(int i = 0; i < CurrentMoveOptions.Count; i++) {
				if (CurrentMoveOptions[i].Key.Equals(newPos)) {
					cost = CurrentMoveOptions[i].Value;
				}
			}
			return BoardManager.MoveUnit(originalPos, newPos,cost);
		}
		public List<KeyValuePair<Position, int>> GetMoves(Position position) {
			CurrentMoveOptions = BoardManager.GetMoveablePos(BoardManager.GetUnitAtPos(position));
			return CurrentMoveOptions;
		}

		//TODO avoid setup scrollview from going all over the place. 
		//TODO memory useage?
		//TODO scroll bars on grid? maybe zoom level? maybe map like AOE?

		//TODO allow users to cancel a movement 

		public List<List<Position>> GetPossibleAttackPos(Position position) {
			Unit playersUnit = BoardManager.GetUnitAtPos(position);
			if (playersUnit.InConflict) {
				if (!conflictManager.ChargesContainsKey(playersUnit)) {//if playerunit is in conflict but hasn't charged anyone this turn then they must have been charged (and thus attacked)
					return new List<List<Position>>() { new List<Position>(), new List<Position>() };//you can't charge or cancel anyone if you have been charged previously
				}
			}

			List<Position> surroundingUnitPos = BoardManager.GetPossibleAttackPos(position, GetCurrentPlayer());
			List<Position> alreadyAttackedPos = new List<Position>();
			List<Position> PossibleAttackPos = new List<Position>();
			for (int i = 0; i < surroundingUnitPos.Count; i++) {
				Position potentialPos = surroundingUnitPos[i];
				Unit unit = BoardManager.GetUnitAtPos(potentialPos);
				if (conflictManager.IsUnitChargingDefendingUnit(playersUnit,unit)) {
					alreadyAttackedPos.Add(potentialPos);//if playerunit has attacked 'unit' in this turn
				} else if (conflictManager.IsValidAttack(playersUnit, unit)) {//already know playersunit hasn't been charged or hasn't charged unit
					PossibleAttackPos.Add(potentialPos);
				}
			}
			return new List<List<Position>>() { alreadyAttackedPos, PossibleAttackPos };
		}

		public void AddCharge(Unit defendingUnit, Unit chargingUnit) {
			conflictManager.AddCharge(defendingUnit, chargingUnit);
		}
		public void RemoveCharge(Unit defendingUnit, Unit chargingUnit) {
			conflictManager.RemoveCharge(defendingUnit, chargingUnit);
		}
		public Dictionary<Unit,List<Unit>> GetConflicts() {
			return conflictManager.TempConflicts;
		}
		private void ResolveAllConflicts() {
			foreach (KeyValuePair<Unit, List<Unit>> ConflictsItem in conflictManager.Conflicts) {
				ResolveConflict(ConflictsItem);
			}
			conflictManager.Conflicts.Clear();
			conflictManager.TempConflicts.Clear();
		}
		private void ResolveConflict(KeyValuePair<Unit,List<Unit>> Conflict) {
			Unit struckUnit = null;//TODO multiple different units can be struck. Let user choose
			List<Unit> StrickingUnits = new List<Unit>();
			if (WereAttackersVictorious(Conflict)) {
				struckUnit = Conflict.Key;
				StrickingUnits = Conflict.Value;
			} else {
				struckUnit = Conflict.Value[0];//TODO let victorious parties do this
				StrickingUnits.Add(Conflict.Key);
			}
			int WoundsInflicted = 0;
			for (int i = 0; i < StrickingUnits.Count; i++) {//sums total wounds to that unit
				if (Utils.ResolveStrike(StrickingUnits[i].Strength,struckUnit.Defense)) {
					WoundsInflicted += 1;
				}
			}
			struckUnit.Wounds -= WoundsInflicted;
			if (struckUnit.Wounds < 1) {
				BoardManager.KillUnit(struckUnit);
			}

			Conflict.Key.InConflict = false;
			for(int i = 0; i < Conflict.Value.Count; i++) {
				Conflict.Value[i].InConflict = false;
			}
		}
		public List<Position> GetJumpablePos(Position position) {
			return BoardManager.GetJumpablePos(position);
		}
		public Position Jump(Unit unit,Position position) {
			int roll = Utils.RollD6(1)[0];
			//update unit movement left
			if (roll == 1) {
				BoardManager.KillUnit(unit);
				return null;
			}else if (roll < 6) {
				unit.MovementLeft = 0;
			}//if roll==6 then leave their movement amount alone
			return BoardManager.Jump(unit, position);						
		}
		private bool WereAttackersVictorious(KeyValuePair<Unit, List<Unit>> Conflict) {
			int DefenderRolls = Conflict.Key.Attacks;
			int AttackerRolls = 0;
			for(int i = 0; i < Conflict.Value.Count; i++) {
				AttackerRolls += Conflict.Value[i].Attacks;
			}			

			int DefenderRoll = Utils.RollD6(DefenderRolls).Max();
			int AttackerRoll = Utils.RollD6(AttackerRolls).Max();

			if (DefenderRoll > AttackerRoll) {
				return false;
			}else if (DefenderRoll < AttackerRoll) {
				return true;
			} else {
				int DefenderFighting = Conflict.Key.Fighting;
				int AttackerFighting = Conflict.Value.Max(x => x.Fighting);//TODO break ties with fighting between the units that rolled the tie?
				if (DefenderFighting > AttackerFighting) {
					return false;
				} else if (DefenderFighting < AttackerFighting) {
					return true;
				} else {					
					return Utils.RollD6(1)[0] >= 4;
				}				
			}
		}

		public void EndTurn() {
			if (Phase == Phases.Move && !IsInSetup) {
				conflictManager.SolidifyCharges();
				BoardManager.SolidifyMoves();
			}
			if (PlayerTurnIndex == Players.Count - 1) {
				if (IsInSetup) {//TODO probably don't need this code when setup is branched off
					IsInSetup = false;
					Phase = Phases.Move;
					//Phase = Phases.Priority;
				} else {
					NextPhase();
					if (this.Phase == Phases.Move) {//start of a new round
						Players = Utils.PickPriority(Players);
						BoardManager.ResetAllMoveability();
					}
				}
				PlayerTurnIndex = 0;//same order as in setup phase. This is okay since everyone can set up at the same time
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