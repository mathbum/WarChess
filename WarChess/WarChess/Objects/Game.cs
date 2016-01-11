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
			return Board.MoveUnit(originalPos, newPos);
		}

		//only way to have three players in same conflict is if A and B are in conflict then C attacks both. In everyother case C can either only attack one or he will branch off to a new conflict
		//when victor is chosen the player can choose who he strikes (rolls for wounds). Different types of units or if unit is trapped.  
		//what if A has super unit. So B,C,D all attack A (but don't want to attack eachother) in this case B,C,D should be victors together? (and only be able to attack A)

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
			if (playersUnit.InConflict) {
				if (!Charges.ContainsKey(playersUnit)) {//if playerunit is in conflict but hasn't charged anyone this turn then they must have been charged (and thus attacked)
					return new List<List<Position>>() { new List<Position>(), new List<Position>() };//you can't charge or cancel anyone if you have been charged previously
				}
			}

			List<Position> surroundingUnitPos = Board.GetPossibleAttackPos(position, GetCurrentPlayer());
			List<Position> alreadyAttackedPos = new List<Position>();
			List<Position> PossibleAttackPos = new List<Position>();
			for (int i = 0; i < surroundingUnitPos.Count; i++) {
				Position potentialPos = surroundingUnitPos[i];
				Unit unit = Board.GetSquareAtPos(potentialPos).Unit;
				if (playersUnit.InConflict && Charges[playersUnit].Contains(unit)) {//charges[playerunit] wont cause an error cuz if it got here we know playersunit is in conflict but hasn't been succesfully charged so he must have charged to be in conflict
					alreadyAttackedPos.Add(potentialPos);//if playerunit has attacked 'unit' in this turn
				} else if(IsValidAttack(playersUnit, unit)) {//already know playersunit hasn't been charged or hasn't charged unit
					PossibleAttackPos.Add(potentialPos);
				}				
			}
			return new List<List<Position>>() { alreadyAttackedPos, PossibleAttackPos };
		}
		private bool IsValidAttack(Unit playersUnit,Unit unit) {//only if we know playersUnit hasn't been charged and it hasn't already charged unit
			if (playersUnit.InConflict) {
				bool alone = false;
				List<Unit> unitsPlayerIsCharging = Charges[playersUnit];
				if (unitsPlayerIsCharging.Count > 1) {//if you are charging multiple enemies then you are alone in conflict
					alone = true;
				} else {
					Unit unitPlayerIsCharging = unitsPlayerIsCharging[0];//becuase we know unitsplayerischarging.count==1
					if (OccurancesInChargesValues(unitPlayerIsCharging) == 1) {//if you are the only one charging "unitPlayerIsCharging"
						if (EnemyCombatantsFighting(unitPlayerIsCharging, TempConflicts) == 1) {//if this is a 1v1. We know becuase "unitPlayerIsCharging" is fighting only 1 enemy and you are only charing 1 enemy
							alone = true;
						}
					}
				}
				if (alone) {
					if (unit.InConflict) {
						if (ConflictDictConatinsUnit(unit, Charges)) {//if anyone is charging unit then you can't
							return false;
						} else if (AlliedCombatantsInConflict(unit, TempConflicts) > 1 && playersUnit.Fighting>=unit.Fighting) {//if unit isn't alone in combat so you could break them off. (but fighting must be at least as high)
							return true;
						}//else false. he is either being charged or in tempconflicts. so above covers all cases where it might return true. 
					} else {
						return true;
					}
				}//if you aren't alone you can't charge anyone else				
			} else {
				if (unit.InConflict) {
					if (ConflictDictConatinsUnit(unit, Charges)) {
						foreach (KeyValuePair<Unit, List<Unit>> ChargesItem in Charges) {//check if another unit is charging 'unit' AND OTHERS. if so; you cant charge him
							List<Unit> unitsBeingCharged = ChargesItem.Value;
							for (int j = 0; j < unitsBeingCharged.Count; j++) {
								if (unit == unitsBeingCharged[j]) {//found unit in Charges
									if (unitsBeingCharged.Count > 1) {//if charger is charging more than one unit
										return false;
									}
								}
							}
						}
						return true;//'unit' was found in Charges and his attacker is only charging him
					} else if (AlliedCombatantsInConflict(unit, TempConflicts) > 1 && playersUnit.Fighting < unit.Fighting) {
						return false;//can't break 'unit' off becuase units fighting is higher
					}
					return true;//if unit was in conflict and either alone or too low fighting to fend off the break off
				} else {
					return true;
				}
			}
			return false;
		}
		private int OccurancesInChargesValues(Unit unit) {
			int attackers = 0;
			foreach (KeyValuePair<Unit, List<Unit>> ChargesItem in Charges) {
				for (int i = 0; i < ChargesItem.Value.Count; i++) {
					if (unit == ChargesItem.Value[i]) {
						attackers++;
					}
				}
			}
			return attackers;
		}
		private int AlliedCombatantsInConflict(Unit unit, Dictionary<Unit, List<Unit>> ConflictDict) {//unit himself counts as a combatant
			if (ConflictDict.ContainsKey(unit)) {
				return 1;
			}
			foreach (KeyValuePair<Unit, List<Unit>> ConflictDictItem in ConflictDict) {
				for (int i = 0; i < ConflictDictItem.Value.Count; i++) {
					if (unit == ConflictDictItem.Value[i]) {
						return ConflictDictItem.Value.Count;
					}
				}
			}
			return 0;//this means they weren't found in any conflict
		}
		private int EnemyCombatantsFighting(Unit unit, Dictionary<Unit, List<Unit>> ConflictDict) {
			if (ConflictDict.ContainsKey(unit)) {
				return ConflictDict[unit].Count;
			}
			foreach (KeyValuePair<Unit, List<Unit>> ConflictDictItem in ConflictDict) {
				for (int i = 0; i < ConflictDictItem.Value.Count; i++) {
					if (unit == ConflictDictItem.Value[i]) {
						return 1;
					}
				}
			}
			return 0;
		}
		private bool ConflictDictConatinsUnit(Unit unit, Dictionary<Unit, List<Unit>> ConflictDict) {
			if (ConflictDict.ContainsKey(unit)) {
				return true;
			}
			foreach (KeyValuePair<Unit, List<Unit>> ConflictDictItem in ConflictDict) {
				for (int i = 0; i < ConflictDictItem.Value.Count; i++) {
					if (unit == ConflictDictItem.Value[i]) {
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
			AddChargeToTempConficts(defendingUnit, attackingUnit);
		}
		private void CloneConflictDict(Dictionary<Unit,List<Unit>> DictToCloneTo, Dictionary<Unit, List<Unit>> DictToClone) {
			DictToCloneTo.Clear();
			foreach(KeyValuePair<Unit,List<Unit>> DictToCloneItem in DictToClone) {
				Unit unitKey = DictToCloneItem.Key;
				List<Unit> newValueList = new List<Unit>();
				List<Unit> unitList = DictToCloneItem.Value;
				for (int i = 0; i < unitList.Count; i++) {
					newValueList.Add(unitList[i]);
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

			CloneConflictDict(TempConflicts, Conflicts);
			RollInCharges();

			if (ConflictDictConatinsUnit(defendingUnit, TempConflicts)) {//checking conflitcs and charges seperately is probalby faster
				return;//defending unit is not in tempconflict as a key or value
			}

			defendingUnit.InConflict = false;//couldn't be found in either tempconflict (which means they aren't in charges or conflits)
		}

		private void SolidifyCharges() {
			CloneConflictDict(Conflicts, TempConflicts);
			Charges.Clear();
		}

		private void RollInCharges() {
			foreach(KeyValuePair<Unit,List<Unit>> ChargeItem in Charges) {
				Unit attacker = ChargeItem.Key;
				for (int i = 0; i < ChargeItem.Value.Count; i++) {
					AddChargeToTempConficts(ChargeItem.Value[i], attacker);
				}
			}
		}

		private void AddChargeToTempConficts(Unit defendingUnit, Unit attackingUnit) {//can't tell difference between charges or attacks.
			if (TempConflicts.ContainsKey(defendingUnit)) {//joining in on a fight on defending unit. could be joining a 1v1 if more than 2 players are playing
				TempConflicts[defendingUnit].Add(attackingUnit);
				return;
			}
			if (TempConflicts.ContainsKey(attackingUnit)) {//attacking unit charges three Units (or more); or attacking unit charges two units then removes the charge then charges another unit
				TempConflicts[attackingUnit].Add(defendingUnit);
				return;
			}

			foreach (KeyValuePair<Unit, List<Unit>> TempConflictsItem in TempConflicts) {
				List<Unit> TempConflictsItemValues = TempConflictsItem.Value;
				for (int i = 0; i < TempConflictsItemValues.Count; i++) {
					if (TempConflictsItemValues[i] == attackingUnit) {
						Unit OtherUnit = TempConflictsItem.Key;
						if (TempConflictsItemValues.Count > 1) {
							throw new ArgumentException(); //attacking unit isn't alone in conflict. is a legal charge?
						} else {//attacking unit is alone in conflict and is adding another charge, so attacker becomes new key and both other units become values
							TempConflicts.Remove(OtherUnit);
							TempConflicts[attackingUnit] = new List<Unit> { OtherUnit, defendingUnit };
						}
						return;
					} else if (TempConflictsItemValues[i] == defendingUnit) {
						Unit OtherUnit = TempConflictsItem.Key;
						if (TempConflictsItemValues.Count > 1) {//defender isn't alone in conflict. so attacker splits off defender into a 1v1
							TempConflicts[OtherUnit].Remove(defendingUnit);
							TempConflicts[defendingUnit] = new List<Unit>() { attackingUnit };
						} else {//defender is alone in conflict so attacker jumps in
							TempConflicts.Remove(OtherUnit);
							TempConflicts[defendingUnit] = new List<Unit> { OtherUnit, attackingUnit };//i think this is only possible with > 2 players
						}
						return;
					}
				}
			}
			TempConflicts[defendingUnit] = new List<Unit>() { attackingUnit };//both attacker and defender are new to conflict

			////TODO: Add Three Way combat (highest combat possible though not possible with the block model)
			////A and B are fighting and C attacks both. D attacks A. It matters if A or B are key to determine if its A,B,C,D or if its A,D B,C					
		}
		public void ResolveAllConflicts() {
			foreach (KeyValuePair<Unit, List<Unit>> ConflictsItem in Conflicts) {
				ResolveConflict(ConflictsItem);
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