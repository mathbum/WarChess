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
		public Dictionary<Unit,List<Unit>> Conflicts = new Dictionary<Unit,List<Unit>>(); //TODO public?
		private Dictionary<Unit, List<Unit>> TempConflicts = new Dictionary<Unit, List<Unit>>();

		//TODO make temp conflicts variable that just replaces the main one after the player ends their turn		

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
			return Board.MoveUnit(originalPos, newPos);//, GetCurrentPlayer());
		}

		//only way to have three players in same conflict is if A and B are in conflict then C attacks both. In everyother case C can either only attack one or he will branch off to a new conflict
		//when victor is chosen the player can choose who he strikes (rolls for wounds). Different types of units or if unit is trapped.  
		//what if A has super unit. So B,C,D all attack A (but don't want to attack eachother) in this case B,C,D should be victors together? (and only be able to attack A)
		
		//TODO allow users to cancel a movement 
		//TODO add gui to show conflicts		

		public List<List<Position>> GetPossibleAttackPos(Position position) {
			Unit playersUnit = Board.GetSquareAtPos(position).Unit;
			List<Position> surroundingUnitPos = Board.GetPossibleAttackPos(position, GetCurrentPlayer());
			List<Position> alreadyAttackedPos = new List<Position>();
			List<Position> PossibleAttackPos = new List<Position>();
			for (int i = 0; i < surroundingUnitPos.Count; i++) {
				Position potentialPos = surroundingUnitPos[i];
				Unit unit = Board.GetSquareAtPos(potentialPos).Unit;

				if (playersUnit.InConflict) {
					if (!TempConflicts.ContainsKey(playersUnit)) {//if playerunit is in conflict but hasn't charged anyone this turn then they must have been charged
						return new List<List<Position>>() { new List<Position>(), new List<Position>() };//you can't charge or cancel anyone if you have been charged previously
					} else {
						if (TempConflicts[playersUnit].Contains(unit)) {//if playerunit has attacked unit in this turn
							alreadyAttackedPos.Add(potentialPos);
							continue;
						}

						bool alone = false;
						List<Unit> unitsPlayerIsCharging = TempConflicts[playersUnit];
						if (unitsPlayerIsCharging.Count > 1) {//if you are charging multiple enemies then you are alone in conflict
							alone = true;
						} else {
							Unit unitPlayerIsCharging = unitsPlayerIsCharging[0];
							if (OccurancesInConflictDictValues(unitPlayerIsCharging, TempConflicts) == 1) {//if you are the only one charging "unitPlayerIsCharging"
								if (OccurancesInConflictDictValues(unitPlayerIsCharging, Conflicts) == 0) {//if noone is attacking "unitPlayerIsCharging"
									alone = true;
								}
							}
						}

						if (alone) {
							if (unit.InConflict) {
								if (OccurancesInConflictDictValues(unit, TempConflicts) > 0) {//if anyone is charging unit then you can't
									continue;
								}else if (OccurancesInConflictDictValues(unit, Conflicts) > 1) {//if unit isn't alone in combat so you could break them off 
									PossibleAttackPos.Add(potentialPos);
								}							
							} else {
								PossibleAttackPos.Add(potentialPos);
							}
						}//else you can't charge anyone else						
					}
				} else {
					if (unit.InConflict) {
						bool canBeCharged = true;
						List<KeyValuePair<Unit, List<Unit>>> TempConflictsList = TempConflicts.ToList();
						for (int j = 0; j < TempConflictsList.Count; j++) {
							Unit attacker = TempConflictsList[j].Key;
							for (int k = 0; k < TempConflictsList[j].Value.Count; k++) {
								if (unit == TempConflictsList[j].Value[k]) {//found unit in tempconflict
									if (TempConflictsList[j].Value.Count == 1) {//if unit is being charged alone. (attacker is only charging one unit)
										canBeCharged = true;
									} else {//attacker is charging more than one unit. so you can't attack him
										canBeCharged = false;
										break;
									}
								}
							}
						}
						if (canBeCharged) {//if he was is in conflict but not about to be charged or 
							//if 'unit' was found in tempconflict and his attacker is only charging him
							PossibleAttackPos.Add(potentialPos);
						}
					} else {
						PossibleAttackPos.Add(potentialPos);
					}
				}				
			}
			return new List<List<Position>>() { alreadyAttackedPos, PossibleAttackPos };
		}
		private int OccurancesInConflictDictValues(Unit unit, Dictionary<Unit,List<Unit>> ConflictDict) {
			int attackers = 0;
			List<KeyValuePair<Unit, List<Unit>>> DictConflictsList = ConflictDict.ToList();
			for (int i = 0; i < DictConflictsList.Count; i++) {
				for (int j = 0; j < DictConflictsList[i].Value.Count; j++) {
					if (unit == DictConflictsList[i].Value[j]) {
						attackers++;
					}
				}
			}
			return attackers;
		}
		public void AddTempConflict(Unit defendingUnit,Unit attackingUnit) {
			defendingUnit.InConflict = true;
			attackingUnit.InConflict = true;
			if (TempConflicts.ContainsKey(attackingUnit)) {
				TempConflicts[attackingUnit].Add(defendingUnit);
			} else {
				TempConflicts[attackingUnit] = new List<Unit>() { defendingUnit };
			}
		}
		public void RemoveTempConflict(Unit defendingUnit, Unit attackingUnit) {
			List<Unit> unitsAttackedByAttacker = TempConflicts[attackingUnit];
			unitsAttackedByAttacker.Remove(defendingUnit);
			if (unitsAttackedByAttacker.Count == 0) {
				attackingUnit.InConflict = false;
				TempConflicts.Remove(attackingUnit);
			}
			List<KeyValuePair<Unit, List<Unit>>> ConflictList = Conflicts.ToList();
			for (int i = 0; i < ConflictList.Count; i++) {//check if defending unit is in conflict
				for (int j = 0; j < ConflictList[i].Value.Count; j++) {
					if (defendingUnit == ConflictList[i].Value[j]) {
						return;
					}
				}
			}
			List<KeyValuePair<Unit, List<Unit>>> TempConflictList = TempConflicts.ToList();
			for(int i = 0; i < TempConflictList.Count; i++) {//check if defending unit is in tempconflict
				for(int j = 0; j < TempConflictList[i].Value.Count; j++) {
					if (defendingUnit == TempConflictList[i].Value[j]) {
						return;
					}
				}				
			}
			defendingUnit.InConflict = false;//couldn't be found in either conflict list
		}
		private void RollInTempConflicts() {
			List<KeyValuePair<Unit, List<Unit>>> TempConflictsList = TempConflicts.ToList();
			for(int i = 0; i < TempConflictsList.Count;i++) {
				Unit attacker = TempConflictsList[i].Key;
				for(int j = 0; j < TempConflictsList[i].Value.Count; j++) {
					AddConflict(attacker,TempConflictsList[i].Value[j]);
				}
			}
			TempConflicts.Clear();
		}

		private void AddConflict(Unit defendingUnit, Unit attackingUnit) {
			//defendingUnit.InConflict = true;
			//attackingUnit.InConflict = true;
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

			Conflict.Key.InConflict = false;
			for(int i = 0; i < Conflict.Value.Count; i++) {
				Conflict.Value[i].InConflict = false;
			}
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
				//TODO try to resolve fight via fighting stat on units
				//TODO break ties with fighting between the units that rolled the tie?
				return Utils.RollD6(1)[0]>=4;
			}
		}

		public void EndTurn() {
			if (PlayerTurnIndex == Players.Count - 1) {
				if (IsInSetup) {//TODO probably don't need this code when setup is branched off
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
				if (this.Phase == Phases.Move) {
					RollInTempConflicts();
				}
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