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
		public Board Board { get; private set; }//public?
		public bool IsInSetup { get; private set; }
		//public enum Phases { Priority, Move, Shoot, Fight };//TODO need end phase? 	do i need priotiry phase?
		public enum Phases { Move};//TODO need end phase? 	do i need priotiry phase?
		private List<Player> Players;
		private int PlayerTurnIndex;
		private Dictionary<Unit,List<Unit>> Conflicts = new Dictionary<Unit,List<Unit>>();
		private Dictionary<Unit, List<Unit>> Charges = new Dictionary<Unit, List<Unit>>();
		public Dictionary<Unit, List<Unit>> TempConflicts = new Dictionary<Unit, List<Unit>>();

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
					throw new ArgumentException();//someone has placed a unit that they aren't allowed to be placing
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
			return Board.MoveUnit(originalPos, newPos);
		}

		//only way to have three players in same conflict is if A and B are in conflict then C attacks both. In everyother case C can either only attack one or he will branch off to a new conflict
		//when victor is chosen the player can choose who he strikes (rolls for wounds). Different types of units or if unit is trapped.  
		//what if A has super unit. So B,C,D all attack A (but don't want to attack eachother) in this case B,C,D should be victors together? (and only be able to attack A)

		//TODO make tempconflicts become chargingDict. then make a tempconflicts with constant rolling of charges? (how hard is removing a charge? especially if you pulled someone from a conflict)		
		//TODO Or everytime a charge is made or cancelled recalculate a new current conflicts dict by rolling the charges into this dict. This dict is only used for gui purposes

		//TODO avoid setup scrollview from going all over the place. 
		//TODO memory useage?
		//TODO scroll bars on grid? maybe zoom level? maybe map like AOE?

		//TODO allow users to cancel a movement 

		//you can charge a unit only if he is not on your team, you havent been sucessfully charged and one of the following:
			//1) you aren't charing anyone else and he isn't being charged by someone who is charging multiple units
			//2) you are alone in your charge (you aren't charging a unit with an ally) and he is not (in conflict or being charged)
			//3) you are alone in your charge (you aren't charging a unit with an ally) and he is in conflict with an ally (so he can be broken off to your conflict)

		public List<List<Position>> GetPossibleAttackPos(Position position) {
			Unit playersUnit = Board.GetSquareAtPos(position).Unit;
			List<Position> surroundingUnitPos = Board.GetPossibleAttackPos(position, GetCurrentPlayer());
			List<Position> alreadyAttackedPos = new List<Position>();
			List<Position> PossibleAttackPos = new List<Position>();
			for (int i = 0; i < surroundingUnitPos.Count; i++) {
				Position potentialPos = surroundingUnitPos[i];
				Unit unit = Board.GetSquareAtPos(potentialPos).Unit;

				if (playersUnit.InConflict) {
					if (!Charges.ContainsKey(playersUnit)) {//if playerunit is in conflict but hasn't charged anyone this turn then they must have been charged
						return new List<List<Position>>() { new List<Position>(), new List<Position>() };//you can't charge or cancel anyone if you have been charged previously
					} else {
						if (Charges[playersUnit].Contains(unit)) {//if playerunit has attacked unit in this turn
							alreadyAttackedPos.Add(potentialPos);
							continue;
						}

						bool alone = false;
						List<Unit> unitsPlayerIsCharging = Charges[playersUnit];
						if (unitsPlayerIsCharging.Count > 1) {//if you are charging multiple enemies then you are alone in conflict
							alone = true;
						} else {
							Unit unitPlayerIsCharging = unitsPlayerIsCharging[0];
							if (OccurancesInConflictDictValues(unitPlayerIsCharging, Charges) == 1) {//if you are the only one charging "unitPlayerIsCharging"
								if (!ConflictDictConatinsUnit(unitPlayerIsCharging, Conflicts)) {//if noone is attacking "unitPlayerIsCharging"
									alone = true;
								}
							}
						}

						if (alone) {
							if (unit.InConflict) {
								if (ConflictDictConatinsUnit(unit, Charges)) {//if anyone is charging unit then you can't
									continue;
								}else if (OccurancesInConflictDictValues(unit, Conflicts) > 1) {//if unit isn't alone in combat so you could break them off 
									PossibleAttackPos.Add(potentialPos);
								}							
							} else {
								PossibleAttackPos.Add(potentialPos);
							}
						}// if you aren't alone you can't charge anyone else
					}
				} else {
					if (unit.InConflict) {
						bool canBeCharged = true;
						List<KeyValuePair<Unit, List<Unit>>> ChargeList = Charges.ToList();
						for (int j = 0; j < ChargeList.Count; j++) {
							List<Unit> unitsBeingCharged = ChargeList[j].Value;
							for (int k = 0; k < unitsBeingCharged.Count; k++) {
								if (unit == unitsBeingCharged[k]) {//found unit in Charges
									if (unitsBeingCharged.Count > 1) {
										canBeCharged = false;
										break;//TODO possibly break out of j loop? if not just take perf hit
									}
								}
							}
						}
						if (canBeCharged) {//if he was is in conflict but not about to be charged or 
							//if 'unit' was found in Charges and his attacker is only charging him
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
		private bool ConflictDictConatinsUnit(Unit unit, Dictionary<Unit, List<Unit>> ConflictDict) {
			List<KeyValuePair<Unit, List<Unit>>> DictConflictsList = ConflictDict.ToList();
			for (int i = 0; i < DictConflictsList.Count; i++) {
				for (int j = 0; j < DictConflictsList[i].Value.Count; j++) {
					if (unit == DictConflictsList[i].Value[j]) {
						return true;
					}
				}
			}
			return false;
		}
		public void AddCharge(Unit defendingUnit,Unit attackingUnit) {
			defendingUnit.InConflict = true;
			attackingUnit.InConflict = true;
			if (Charges.ContainsKey(attackingUnit)) {
				Charges[attackingUnit].Add(defendingUnit);
			} else {
				Charges[attackingUnit] = new List<Unit>() { defendingUnit };
			}
			AddConflictToTempConficts(defendingUnit, attackingUnit);//TODO for gui
		}
		private void CloneConflictDict(Dictionary<Unit,List<Unit>> DictToCloneTo, Dictionary<Unit, List<Unit>> DictToClone) {
			DictToCloneTo.Clear();
			List<KeyValuePair<Unit, List<Unit>>> DictToCloneList = DictToClone.ToList();
			for (int i = 0; i < DictToCloneList.Count; i++) {
				Unit unitKey = DictToCloneList[i].Key;
				List<Unit> newValueList = new List<Unit>();
				List<Unit> unitList = DictToCloneList[i].Value;
				for (int j = 0; j < unitList.Count; j++) {
					newValueList.Add(unitList[j]);
				}					
				DictToCloneTo[unitKey] = newValueList;
			}
		}
		public void RemoveCharge(Unit defendingUnit, Unit chargingUnit) {			
			List<Unit> unitsChargedByAttacker = Charges[chargingUnit];
			unitsChargedByAttacker.Remove(defendingUnit);
			if (unitsChargedByAttacker.Count == 0) {
				chargingUnit.InConflict = false;
				Charges.Remove(chargingUnit);
			}

			CloneConflictDict(TempConflicts, Conflicts);//this resets tempconflics so the gui can use it. could i make a rolling tempconflicts?
			RollInCharges();

			if (Conflicts.ContainsKey(defendingUnit) || ConflictDictConatinsUnit(defendingUnit, Conflicts)) {
				return;//defending unit is not in conflict as a key or value
			}
			if (ConflictDictConatinsUnit(defendingUnit, Charges)) {//becuase its not possible that defending unit is the charging unit
				return;//defending unit is in Charges (values)
			}

			defendingUnit.InConflict = false;//couldn't be found in either conflicts or charges
		}

		private void SolidifyCharges() {
			CloneConflictDict(Conflicts, TempConflicts);
			Charges.Clear();
		}

		private void RollInCharges() {
			List<KeyValuePair<Unit, List<Unit>>> ChargeList = Charges.ToList();
			for(int i = 0; i < ChargeList.Count;i++) {
				Unit attacker = ChargeList[i].Key;
				for(int j = 0; j < ChargeList[i].Value.Count; j++) {
					AddConflictToTempConficts(attacker,ChargeList[i].Value[j]);
				}
			}
			//Charges.Clear();
		}

		private void AddConflictToTempConficts(Unit defendingUnit, Unit attackingUnit) {
			if (TempConflicts.ContainsKey(defendingUnit)) {
				TempConflicts[defendingUnit].Add(attackingUnit);
				return; 
			}
			if (TempConflicts.ContainsKey(attackingUnit)) {//attacking unit charges three Units (or more) or attacking unit charges two units then removes the charge then charges another unit
				TempConflicts[attackingUnit].Add(defendingUnit);
				return;
			}

			//foreach(KeyValuePair<Unit,List<Unit>> Conflict in TempConflicts) {
			//	List<Unit> unitsBeingAttackedList = Conflict.Value;
			//	for (int j = 0; j < unitsBeingAttackedList.Count; j++) {
			//		if (unitsBeingAttackedList[j] == defendingUnit) {
			//			//Unit OtherUnit = ConflictList[j].Key;
			//			Unit OtherUnit = Conflict.Key;
			//			if (unitsBeingAttackedList.Count > 1) {//if defending unit is in conflict already and not alone, remove from conflict and is key in new conflict and attacker becomes value
			//				TempConflicts[OtherUnit].Remove(defendingUnit);
			//				TempConflicts[defendingUnit] = new List<Unit>() { attackingUnit };
			//			} else {//if defending unit is in conflict already and alone then defending unit becomes key and attacking unit and old key become value
			//				TempConflicts.Remove(OtherUnit);
			//				TempConflicts[defendingUnit] = new List<Unit> { OtherUnit, attackingUnit };
			//			}
			//			return;
			//		}
			//	}
			//}
			List<KeyValuePair<Unit, List<Unit>>> ConflictList = TempConflicts.ToList();
			for (int i = 0; i < ConflictList.Count; i++) {
				List<Unit> attackingUnitList = ConflictList[i].Value;
				for (int j = 0; j < attackingUnitList.Count; j++) {
					if (attackingUnitList[j] == defendingUnit) {
						//Unit OtherUnit = ConflictList[j].Key;
						Unit OtherUnit = ConflictList[i].Key;
						if (attackingUnitList.Count > 1) {//if defending unit is in conflict already and not alone, remove from conflict and is key in new conflict and attacker becomes value
							TempConflicts[OtherUnit].Remove(defendingUnit);
							TempConflicts[defendingUnit] = new List<Unit>() { attackingUnit };
						} else {//if defending unit is in conflict already and alone then defending unit becomes key and attacking unit and old key become value
							TempConflicts.Remove(OtherUnit);
							TempConflicts[defendingUnit] = new List<Unit> { OtherUnit, attackingUnit };
						}
						return;
					}
				}
			}

			//if defending unit isn't in conflict then it is key and attacker is value
			TempConflicts[defendingUnit] = new List<Unit>() { attackingUnit };

			//TODO: Add Three Way combat (highest combat possible though not possible with the block model)
			//A and B are fighting and C attacks both. D attacks A. It matters if A or B are key to determine if its A,B,C,D or if its A,D B,C					
		}
		public void ResolveAllConflicts() {
			//foreach (KeyValuePair<Unit, List<Unit>> Conflict in Conflicts) {
			//	ResolveConflict(Conflict);
			//}
			List<KeyValuePair<Unit, List<Unit>>> Conflicts1List = Conflicts.ToList();
			for (int i = 0; i < Conflicts1List.Count; i++) {
				ResolveConflict(Conflicts1List[i]);
			}
			Conflicts.Clear();
			TempConflicts.Clear();
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
			if (Phase == Phases.Move && !IsInSetup) {
				//RollInCharges();
				SolidifyCharges();
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