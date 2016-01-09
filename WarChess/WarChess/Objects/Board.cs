using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.TerrainObjs;

namespace WarChess.Objects {
	public class Board {//TODO have flyweight pattern for terrain objs and maybe squares if i can think of how
		public List<List<Square>> board;//TODO make temp board variable that just replaces the main one after the player ends their turn
		public int Rows { get; private set; }
		public int Columns { get; private set; }
		public NullUnit NullUnit { get; private set; }

		//TODO make a function to set the distance cost for squares surrounding current square. params = listlist int, pos (of sqquare), unit (for the possibility that some units have bonuses on certain terrain)
		//TODO then make a recursive algo (starting all dists to -1) that sets the calculated dist for all squares
		//TODO add pos to units?
		//TODO can a unit change a move when the jumped or climbed?

		public Board(int rows,int cols) {//TODO is it possible for a board to be created without nullunits everywhere?
			this.NullUnit = new NullUnit();
			board = new List<List<Square>>();
			for(int i=0;i<rows; i++) {
				List<Square> row = new List<Square>();
				for (int j = 0; j < cols; j++) {
					row.Add(new Square(NullUnit));
				}
				board.Add(row);
			}
			this.Rows = rows;
			this.Columns = cols;
		}
		public bool PlaceUnit(Position position,Unit unit) {
			bool isValidPlacement = IsValidPlacement(position, unit);
			if (isValidPlacement) {
				SetUnit(position, unit);
			}
			return isValidPlacement;
		}
		private bool IsValidPlacement(Position position,Unit unit) {//TODO finish this
			if (board[position.Row][position.Column].Unit != this.NullUnit) {
				return false;
			}
			return true;
		}
		private void SetUnit(Position position, Unit unit) {
			this.board[position.Row][position.Column].Unit = unit;
		}
		private bool isValidMove(Position originalPos, Position newPos) {//TODO finish this
			//if(Player!= GetSquareAtPos(originalPos).Unit.Player) {
			//	return false;
			//}
			if (GetSquareAtPos(newPos).Unit != this.NullUnit) {//TODO if you tried to move onto another unit or move to where the unit was. Maybe this should just be disallowed by gui?
				return false;//TODO gui tod disallows you to move to your same position?
			}
			if (GetSquareAtPos(originalPos).Unit.InConflict) {//if unit is in conflict
				return false;
			}
			return true;
		}
		public bool MoveUnit(Position originalPos,Position newPos) {//,Player Player) {
			if (isValidMove(originalPos, newPos)) {//,Player)) {
				Unit tempUnit = GetSquareAtPos(originalPos).Unit;
				SetUnit(newPos, tempUnit);
				SetUnit(originalPos, NullUnit);
				return true;
			}
			return false;
		}
		public void KillUnit(Unit unit) {
			for(int i = 0; i < board.Count; i++) {
				for (int j = 0; j < board[i].Count; j++) {
					if (board[i][j].Unit == unit) {
						SetUnit(new Position(i, j), NullUnit);
						Trace.WriteLine("Killed: " + unit.Name + " at pos: " +i+", "+j);
					}
				}
			}
			
		}
		public Square GetSquareAtPos(Position position) {
			//if (position.Row >= Rows || position.Column >= Columns) {
			//	return null;
			//}
			return board[position.Row][position.Column];
		}
		public List<Position> GetPossibleAttackPos(Position position,Player player) {
			List<Position> possibleattackpos = new List<Position>();
			List<Position> possiblepos = GetSurroundingPos(position);
			for(int i = 0; i < possiblepos.Count; i++) {
				Unit unit = GetSquareAtPos(possiblepos[i]).Unit;
				if (unit!=NullUnit && unit.Player != player) {
					possibleattackpos.Add(possiblepos[i]);
				}
			}
			return possibleattackpos;
		}
		private List<Position> GetSurroundingPos(Position position) {
			List<Position> surroundingSquares = new List<Position>();
			if (position.Row - 1 >= 0) {//up
				surroundingSquares.Add(new Position(position.Row - 1, position.Column));
			}
			if (position.Column + 1 < Columns) {//right
				surroundingSquares.Add(new Position(position.Row, position.Column + 1));
			}
			if (position.Row + 1 < Rows) {//down
				surroundingSquares.Add(new Position(position.Row + 1, position.Column));
			}
			if (position.Column - 1 >= 0) {//left
				surroundingSquares.Add(new Position(position.Row, position.Column - 1));
			}
			return surroundingSquares;
		}
	}
}
