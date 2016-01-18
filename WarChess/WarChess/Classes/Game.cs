using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.Items;

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
		public enum Phases { Move, Shoot };//TODO need end phase? 	do i need priotiry phase?
		private List<Player> Players;
		public int pointLimit { get; set; }
		private int PlayerTurnIndex = 0;
		private ConflictManager conflictManager = new ConflictManager();
		private List<KeyValuePair<Position, int>> CurrentMoveOptions;
		//private List<List<Position>> CurrentShotPathDetails;
		private Dictionary<Position, List<List<Position>>> ShotOptions;


		public bool PlaceUnit(Position position,Unit unit) {
			return BoardManager.PlaceUnit(position, unit);
		}
		public int GetBoardRows() {
			return BoardManager.GetRows();
		}
		public int GetBoardColumns() {
			return BoardManager.GetColumns();
		}
		public Terrain GetTerrainAtPos(Position position) {
			return BoardManager.GetSquareAtPos(position).Terrain;
		}
		public Unit GetUnitAtPos(Position position) {
			return BoardManager.GetUnitAtPos(position);
		}
		public Player GetCurrentPlayer() {
			return Players[PlayerTurnIndex];
		}
		public bool IsValidPlacement(Position position) {
			return BoardManager.IsValidPlacement(position);
		}
		public Unit CreateUnit(string unitName) {
			Unit tempUnit = Config.Units[unitName].unit;
			string Name = unitName;
			int Points = tempUnit.BasePoints;
			int Width = tempUnit.Width;
			int Length = tempUnit.Length;
			Config.Allegiance Allegiance = tempUnit.Allegiance;
			int Fighting = tempUnit.Fighting;
			int ShootingProficiency = tempUnit.ShootingSkill;
			int Strength = tempUnit.Strength;
			int Defense = tempUnit.BaseDefense;
			int Attack = tempUnit.Attacks;
			int Wounds = tempUnit.Wounds;
			int Might = tempUnit.Might;
			int Will = tempUnit.Will;
			int Fate = tempUnit.Fate;
			return new Unit(Name, Points, Width, Length, Allegiance, Fighting, ShootingProficiency, Strength, Defense, Attack, Wounds, Might, Will, Fate);
		}

		public bool Move(Position originalPos, Position newPos) {
			int cost = -1;
			for(int i = 0; i < CurrentMoveOptions.Count; i++) {
				if (CurrentMoveOptions[i].Key.Equals(newPos)) {
					cost = CurrentMoveOptions[i].Value;
					break;
				}
			}
			return BoardManager.MoveUnit(originalPos, newPos, cost);
		}
		public List<KeyValuePair<Position, int>> GetMoves(Position position) {//TODO don't call this right after you move a unit
			CurrentMoveOptions = BoardManager.GetMoveablePos(BoardManager.GetUnitAtPos(position));
			return CurrentMoveOptions;
		}

		//TODO avoid setup scrollview from going all over the place. 
		//TODO memory useage?
		//TODO scroll bars on grid? maybe zoom level? maybe map like AOE?

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
				if (Utils.ResolveStrike(StrickingUnits[i].Strength,struckUnit.GetDefense())) {
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
			
			if (roll == 1) {
				Trace.WriteLine(unit.Player.Name + "'s " + unit.Name + " failed to make his jump");
				unit.MovementLeft = 0;
				return position;
				//BoardManager.KillUnit(unit);//jump can wound if they jump a gap or something far
				//return null;
			}
			int initCost = 0;
			for (int i = 0; i < CurrentMoveOptions.Count; i++) {
				if (CurrentMoveOptions[i].Key.Equals(unit.Position)) {
					initCost = CurrentMoveOptions[i].Value;
					break;
				}
			}
			Position newPos = BoardManager.Jump(unit, position, initCost);
			if (roll < 6) {
				Trace.WriteLine(unit.Player.Name + "'s " + unit.Name + " made the jump but cost all his movement");
				unit.MovementLeft = 0;
			} else {
				Trace.WriteLine(unit.Player.Name + "'s " + unit.Name + " easily made the jump and cost no extra");
				//if roll==6 then leave their movement amount alone
			}
			return newPos;
		}
		public List<List<Position>> GetShotPathDetails(Position Shooter, Position Target) {
			Unit ShootingUnit = BoardManager.GetUnitAtPos(Shooter);
			if (!ShootingUnit.HasShot) {
				if (ShotOptions.ContainsKey(Target)) {
					return ShotOptions[Target];
				}
				return BoardManager.GetShotDetails(Shooter, Target);
			}
			return new List<List<Position>>();
		}//whenever gui wants to update shot details just send them the dict values
		public List<Position> GetShotOptions(Position Shooter) {
			Unit unit = BoardManager.GetUnitAtPos(Shooter);			
			if (!unit.InConflict) {
				bool canShoot = false;
				for(int i = 0; i < unit.EquipItems.Count; i++) {
					Item item = unit.EquipItems[i].Key;
					if(item is RangedWeapon) {
						RangedWeapon rangedItem = (RangedWeapon)item;
						if((double)unit.MovementLeft / unit.MaxMoveDist >= rangedItem.MovementCost-Utils.epsilon) {
							canShoot = true;
						}
					}
				}				
				if (canShoot) {// check to make sure unit has an item to shoot with. check to make sure unit has enough movement left and hasn't already shot.
					ShotOptions = BoardManager.GetShotOptions(Shooter);
					return ShotOptions.Keys.ToList();//to display shoot buttons
				}
			}
			ShotOptions = new Dictionary<Position, List<List<Position>>>();
			return ShotOptions.Keys.ToList();
		}//when you select a unit in the shoot phase. get all shoot options. then store them in a dict. 
		public void Shoot(Position Shooter,Position Target) {
			List<List<Position>> ShotDetails = ShotOptions[Target];
			List<Position> PosPreventingShot = ShotDetails[2];
			Unit ShootingUnit = BoardManager.GetUnitAtPos(Shooter);
			if (PosPreventingShot.Count == 0 && !ShootingUnit.HasShot) {
				int roll = Utils.RollD6(1)[0];				
				ShootingUnit.HasShot = true;
				if (roll >= ShootingUnit.ShootingSkill) {//shooter hit target

					List<Position> ObstructionPos = ShotDetails[1];
					for (int i = 0; i < ObstructionPos.Count; i++) {//TODO determine if you pass obsticule based upon shootingskill?
						//roll to see if you pass each object
						roll = Utils.RollD6(1)[0];
						if (roll <= 3) {//didn't pass obj
							Unit unit = BoardManager.GetUnitAtPos(Shooter);
							Trace.WriteLine(unit.Player.Name + "'s " + unit.Name + " missed his target, (but might still hit someone else acidentally)");
							Square square = BoardManager.GetSquareAtPos(ObstructionPos[i]);
							if (!square.Terrain.IsStandable) {
								return;								
							}else if (square.Unit != Config.NullUnit && !square.Unit.InConflict) {
								Target = ObstructionPos[i];
								break;
							}else if (square.Unit.InConflict) {
								//TODO deal with conflict
								//set target based upon conflict.... this might be annoying
							}
						}
					}
					Unit struckUnit = BoardManager.GetUnitAtPos(Target);
					if (Utils.ResolveStrike(3, struckUnit.GetDefense())) {//TODO strength of shot hardcoded to 3 right now.
						struckUnit.Wounds -= 1;
						Unit unit = BoardManager.GetUnitAtPos(Shooter);
						Trace.WriteLine(unit.Player.Name + "'s " + unit.Name + " hit and wounded a unit (probably his target)");
						if (struckUnit.Wounds < 1) {
							BoardManager.KillUnit(struckUnit);							
						}
					}else {
						Unit unit = BoardManager.GetUnitAtPos(Shooter);
						Trace.WriteLine(unit.Player.Name + "'s " + unit.Name + " hit a unit (probably his target) but didn't wound him");
					}
				} else {
					Unit unit = BoardManager.GetUnitAtPos(Shooter);
					Trace.WriteLine(unit.Player.Name + "'s " + unit.Name + " missed his target");
				}
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

		private Phases NextPhase() {
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