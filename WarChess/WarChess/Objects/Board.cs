using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Board {
		public List<List<Square>> board { get; private set; }
		public int Rows { get; private set; }
		public int Columns { get; private set; }

		public Board(int rows, int cols) {
			board = new List<List<Square>>();
			for (int i = 0; i < rows; i++) {
				List<Square> row = new List<Square>();
				for (int j = 0; j < cols; j++) {
					row.Add(new Square(Config.TerrainObjs[' '], Config.NullUnit));
				}
				board.Add(row);
			}
			Rows = rows;
			Columns = cols;
		}
		public Board(List<string> BarrenBoard) {
			board = new List<List<Square>>();
			Rows = BarrenBoard.Count;
			Columns = BarrenBoard[0].Length;//TODO check to make sure at leaset one row?
			for (int i = 0; i < Rows; i++) {
				List<Square> row = new List<Square>();
				for (int j = 0; j < Columns; j++) {
					row.Add(new Square(Config.TerrainObjs[BarrenBoard[i][j]], Config.NullUnit));
				}
				board.Add(row);
			}
		}
		private bool isValidMove(Position originalPos, Position newPos, int cost) {//TODO finish this
			////if (Player!= GetSquareAtPos(originalPos).Unit.Player) {
			//	//	return false;
			////}
			Square square = GetSquareAtPos(newPos);//this is duplicate code
			if (/*square.Unit != Config.NullUnit || */!square.Terrain.IsStandable || cost==-1 || GetUnitAtPos(originalPos).MovementLeft<cost) {
				return false;//if you tried moving onto non standable terrain. if the cost to get there is out of reach, or if you don't have enough moveleft.
			}
			//if (GetUnitAtPos(originalPos).InConflict) {//if unit is in conflict
			//	return false;//this happen?
			//}
			return true;
		}
		public bool MoveUnit(Position originalPos, Position newPos, int cost) {
			if (isValidMove(originalPos, newPos, cost)) {
				Unit tempUnit = GetUnitAtPos(originalPos);
				SetUnit(newPos, tempUnit);
				board[originalPos.Row][originalPos.Column].Unit = Config.NullUnit;
				return true;
			}
			return false;
		}
		public Position GetNextPosInDirection(Position p1,Position p2) {
			int RowDiff = p2.Row > p1.Row?1: (p2.Row < p1.Row?-1:0);//force it to only move 1 square
			int ColumnDiff = p2.Column > p1.Column?1: (p2.Column < p1.Column?-1:0);
			int newRow = p2.Row + RowDiff;
			int newCol = p2.Column + ColumnDiff;
			if(newRow>=0 && newRow<Rows && newCol>=0 && newCol < Columns) {
				return new Position(newRow,newCol);
			}
			return null;
		}
		public void ResetAllMoveability() {
			for(int i = 0; i < Rows; i++) {
				for(int j=0;j< Columns; j++) {
					Unit unit = GetUnitAtPos(new Position(i, j));
					unit.MovementLeft = unit.MaxMoveDist;
				}
			}
		}
		public Position Jump(Unit unit,Position position,int initCost) {//any validation?
			Position nextPos = GetNextPosInDirection(unit.Position, position);
			if (nextPos != null) {
				int cost = initCost + GetSquareAtPos(position).Terrain.Speed + GetSquareAtPos(nextPos).Terrain.Speed;
				bool worked = MoveUnit(unit.Position, nextPos, cost);
				if (!worked) {//if the move for some reason fails
					throw new ArgumentException();
				}
				unit.MovementLeft = unit.MovementLeft - cost;				
			}
			return nextPos;
		}
		public List<Position> GetJumpablePos(Position position,int initCost) {
			List<Position> possiblePos = GetSurroundingPos(position);
			List<Position> JumpablePos = new List<Position>();
			Unit unit = GetUnitAtPos(position);
			for (int i = 0; i < possiblePos.Count; i++) {//if the square after it in same direction is standable and has no units. and unit has enough move left. then you can jump it
				Square square = GetSquareAtPos(possiblePos[i]);
				if (square.Terrain.IsJumpable) {
					Position pos = GetNextPosInDirection(position, possiblePos[i]);
					if (pos != null) {
						Square nextSquare = GetSquareAtPos(pos);
						int cost = square.Terrain.Speed + nextSquare.Terrain.Speed + initCost;
						if (unit.MovementLeft >= cost && nextSquare.Terrain.IsStandable && nextSquare.Unit == Config.NullUnit) {
							//TODO give horses buff on 1 deep jumps and able to make two deep jumps and large units can walk over single deep jumpable terrain
							JumpablePos.Add(possiblePos[i]);
						}
					}
				}
			}
			return JumpablePos;
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
			Square square = GetSquareAtPos(position);
			if (square.Unit != Config.NullUnit || !square.Terrain.IsStandable) {
				return false;
			}
			return true;
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
