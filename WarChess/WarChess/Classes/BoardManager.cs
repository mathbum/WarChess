using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.TerrainObjs;

namespace WarChess.Objects {//TODO become board manager, board object, movement manager?
	public class BoardManager {//TODO have flyweight pattern for terrain objs and maybe squares if i can think of how
		private Board Board;//TODO make temp board variable that just replaces the main one after the player ends their turn
		public BoardManager(int rows, int cols) {
			Board = new Board(rows, cols);
		}
		public BoardManager(Board Board) {
			this.Board = Board;
		}
		public int GetRows() {
			return Board.Rows;
		}
		public int GetColumns() {
			return Board.Columns;
		}		
		public bool PlaceUnit(Position position, Unit unit) {
			return Board.PlaceUnit(position, unit);
		}
		public Unit GetUnitAtPos(Position position) {
			return Board.GetUnitAtPos(position);
		}
		public Square GetSquareAtPos(Position position) {
			return Board.GetSquareAtPos(position);
		}
		public bool MoveUnit(Position originalPos, Position newPos) {
			return Board.MoveUnit(originalPos, newPos);
		}
		public void KillUnit(Unit unit) {
			Board.KillUnit(unit);
		}

		//TODO can a unit change a move when the jumped or climbed?
		//TODO dictionary mapping of units to position?

		//TODO make cancelable moves.
		//TODO units can move as far as many times as the want beucase you are basing it off of their current position which is updated on moves.
		public List<KeyValuePair<Position, int>> GetMoveablePos(Unit unit) {
			List<KeyValuePair<Position, int>> PossibleMoves = new List<KeyValuePair<Position, int>>();
			if (unit.InConflict) {
				return PossibleMoves;
			}
			List<List<int>> Distances = FindDistancesHelper(Board, unit);
			for (int i = 0; i < Distances.Count; i++) {
				for (int j = 0; j < Distances[i].Count; j++) {
					int Distance = Distances[i][j];
					Position position = new Position(i, j);
					if (Distance!=-1 && Board.GetUnitAtPos(position)==Config.NullUnit && !unit.Position.Equals(position)) {
						PossibleMoves.Add(new KeyValuePair<Position,int>(position,Distance));
					}
				}
			}
			return PossibleMoves;
		}

		private List<List<int>> FindDistancesHelper(Board board,Unit unit) {
			Position position = unit.Position;
			List<List<int>> distances = new List<List<int>>();
			for(int i = 0; i < board.Rows; i++) {
				List<int> row = new List<int>();
				for(int j = 0; j < board.Columns; j++) {
					if (position.Row == i && position.Column == j) {
						row.Add(0);
					}else {
						row.Add(-1);
					}
				}
				distances.Add(row);
			}
			return FindDistances(board, distances, unit, position);
		}
		private List<List<int>> FindDistances(Board board, List<List<int>> distances, Unit unit, Position CurrentPosition) {
			List<Position> surrpos = board.GetSurroundingPos(CurrentPosition);
			int currentCost = distances[CurrentPosition.Row][CurrentPosition.Column];
			for(int i = 0; i < surrpos.Count; i++) {
				int row = surrpos[i].Row;
				int col = surrpos[i].Column;
				Square newSquare = board.GetSquareAtPos(surrpos[i]);
				if (newSquare.Terrain.Speed == -1 || (newSquare.Unit != Config.NullUnit && newSquare.Unit.Player!=unit.Player)) {
					continue;
				}
				int newCost = currentCost + newSquare.Terrain.Speed;
				if (((newCost != -1 && distances[row][col]==-1) || newCost<distances[row][col])&&newCost<=unit.MaxMoveDist) {
					distances[row][col] = newCost;
					distances = FindDistances(board, distances, unit, surrpos[i]);
				}
			}
			return distances;
		}
		public List<Position> GetPossibleAttackPos(Position position,Player player) {
			List<Position> possibleattackpos = new List<Position>();
			List<Position> possiblepos = Board.GetSurroundingPos(position);
			for(int i = 0; i < possiblepos.Count; i++) {
				Unit unit = Board.GetUnitAtPos(possiblepos[i]);
				if (unit!=Config.NullUnit && unit.Player != player) {
					possibleattackpos.Add(possiblepos[i]);
				}
			}
			return possibleattackpos;
		}
	}
}
