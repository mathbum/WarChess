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
		private Dictionary<Unit,List<Unit>> Conflicts = new Dictionary<Unit,List<Unit>>();

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

		//only way to have three players in same conflict is if A and B are in conflict then C attacks both. In everyother case C can either only attack one or he will branch off to a new conflict
		//when victor is chosen the player can choose who he strikes (rolls for wounds). Different types of units or if unit is trapped.  

		//maybe key/value should determine team sides instead of just determining units in the conflict then resolving as if there are no sides	
		//victors should be by unit instead of by player. this solves the below issue
		//what if A has super unit. So B,C,D all attack A (but don't want to attack eachother) in this case B,C,D should be victors together? (and only be able to attack A)

		//TODO make function to determine if user is in conflict	
		
		//TODO add function to get all possible charges? then use this on the gui to allow user to determine who they attack
		//TODO freeze units that are in combat
		//TODO allow users to cancel a movement //TODO add temp conflicts 
		//TODO add gui to show conflicts		


		public void AddConflict(Unit defendingUnit, Unit attackingUnit) {
			if (Conflicts.ContainsKey(defendingUnit)) {
				Conflicts[defendingUnit].Add(attackingUnit);
				return; 
			}
			if (Conflicts.ContainsKey(attackingUnit)) {//attacking unit charges three Units (or more)
				Conflicts[attackingUnit].Add(defendingUnit);
				return;
			}			
			
			List<KeyValuePair<Unit, List<Unit>>> ConflictList = Conflicts.ToList();
			for (int i=0; i < ConflictList.Count; i++){
				List<Unit> attackingUnitList = ConflictList[i].Value;
				for (int j = 0; j < attackingUnitList.Count; j++) {
					if (attackingUnitList[j] == defendingUnit) {
						Unit OtherUnit = ConflictList[j].Key;
						if (attackingUnitList.Count > 1) {//if defending unit is in conflict already and not alone removed from conflict and is key in new conflict and attacker becomes value
							Conflicts[OtherUnit].Remove(defendingUnit);
							Conflicts[defendingUnit]= new List<Unit>() { attackingUnit };
						} else {//if defending unit is in conflict already and alone then defending unit becomes key and attacking unit and old key become value
							Conflicts.Remove(OtherUnit);
							Conflicts[defendingUnit] = new List<Unit> { OtherUnit, attackingUnit };							
						}
						return;
					}
				}
			}

			//if defending unit isn't in conflict then it is key and attacker is value
			Conflicts[defendingUnit] = new List<Unit>() { attackingUnit };

			//TODO: Add Three Way combat (highest combat possible though not possible with the block model)
			//A and B are fighting and C attacks both. D attacks A. It matters if A or B are key to determine if its A,B,C,D or if its A,D B,C			
		}
		public void ResolveAllConflicts() {
			List<KeyValuePair<Unit, List<Unit>>> Conflicts1List = Conflicts.ToList();
			for (int i = 0; i < Conflicts1List.Count; i++) {
				ResolveConflict(Conflicts1List[i]);
			}
			Conflicts.Clear();
		}
		public void ResolveConflict(KeyValuePair<Unit,List<Unit>> Conflict) {
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
				Board.KillUnit(struckUnit);
			}
		}

		public bool WereAttackersVictorious(KeyValuePair<Unit, List<Unit>> Conflict) {
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
				//TODO try to resolve fight via fighting stat on units
				//TODO break ties with fighting between the units that rolled the tie?
				return Utils.RollD6(1)[0]>=4;
			}
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
