using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.TerrainObjs;

namespace WarChess.Objects {
	public class Board {
		public List<List<Square>> board { get; private set; }
		public int Rows { get; private set; }
		public int Columns { get; private set; }

		public Board(int rows, int cols) {//TODO add ability to set up a board from something other than a blank slate
			board = new List<List<Square>>();
			for (int i = 0; i < rows; i++) {
				List<Square> row = new List<Square>();
				for (int j = 0; j < cols; j++) {
					row.Add(new Square(new Grass(), Config.NullUnit));//TODO flyweight the terrain
				}
				board.Add(row);
			}
			Rows = rows;
			Columns = cols;
		}
		private bool isValidMove(Position originalPos, Position newPos) {//TODO finish this
			//if (Player!= GetSquareAtPos(originalPos).Unit.Player) {
				//	return false;
			//}
			if (GetUnitAtPos(newPos) != Config.NullUnit) {//TODO if you tried to move onto another unit or move to where the unit was. Maybe this should just be disallowed by gui?
				return false;//TODO gui tod disallows you to move to your same position?
			}
			if (GetUnitAtPos(originalPos).InConflict) {//if unit is in conflict
				return false;
			}
			return true;
		}
		public bool MoveUnit(Position originalPos, Position newPos) {
			if (isValidMove(originalPos, newPos)) {
				Unit tempUnit = GetUnitAtPos(originalPos);
				SetUnit(newPos, tempUnit);
				SetUnit(originalPos, Config.NullUnit);
				tempUnit.Position = newPos;
				return true;
			}
			return false;
		}
		public void KillUnit(Unit unit) {
			for (int i = 0; i < board.Count; i++) {
				for (int j = 0; j < board[i].Count; j++) {
					if (board[i][j].Unit == unit) {
						SetUnit(new Position(i, j), Config.NullUnit);
						//Trace.WriteLine("Killed: " + unit.Name + " at pos: " + i + ", " + j);
						unit = null;//this a proper way to destroy the unit object?
					}
				}
			}
		}
		private void SetUnit(Position position, Unit unit) {
			board[position.Row][position.Column].Unit = unit;
			unit.Position = position;
		}
		public bool PlaceUnit(Position position, Unit unit) {
			bool isValidPlacement = IsValidPlacement(position);
			if (isValidPlacement) {
				SetUnit(position, unit);
			}
			return isValidPlacement;
		}
		private bool IsValidPlacement(Position position) {//TODO finish this. with legal placement areas etc...
			if (board[position.Row][position.Column].Unit == Config.NullUnit) {
				return true;
			}
			return false;
		}
		public Unit GetUnitAtPos(Position position) {
			return GetSquareAtPos(position).Unit;
		}
		public Square GetSquareAtPos(Position position) {
			//if (position.Row >= Rows || position.Column >= Columns) {
			//	return null;
			//}
			return board[position.Row][position.Column];
		}

		public List<Position> GetSurroundingPos(Position position) {
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
