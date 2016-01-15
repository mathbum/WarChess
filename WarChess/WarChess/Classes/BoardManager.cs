using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class BoardManager {//TODO have flyweight pattern for terrain objs and maybe squares if i can think of how
		private Board Board;
		private Dictionary<Position, KeyValuePair<Position, int>> Moves = new Dictionary<Position, KeyValuePair<Position, int>>();
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
		public bool MoveUnit(Position originalPos, Position newPos, int cost) {
			bool isValidMove = Board.MoveUnit(originalPos, newPos, cost);
			if (isValidMove) {

				if (Moves.ContainsKey(originalPos)) {
					Moves[newPos] = new KeyValuePair<Position, int>(Moves[originalPos].Key, cost);
					Moves.Remove(originalPos);
				} else {
					Moves[newPos] = new KeyValuePair<Position, int>(originalPos, cost);
				}
			}
			return isValidMove;
		}
		public void KillUnit(Unit unit) {
			Board.KillUnit(unit);
		}
		public void SolidifyMoves() {
			foreach (KeyValuePair<Position, KeyValuePair<Position, int>> move in Moves) {
				Unit unit = Board.GetUnitAtPos(move.Key);
				unit.MovementLeft = unit.MovementLeft - move.Value.Value;
			}
			Moves.Clear();
		}
		public void ResetAllMoveability() {
			Board.ResetAllMoveability();
		}
		public Position Jump(Unit unit, Position position, int initCost) {
			Moves.Remove(unit.Position);
			return Board.Jump(unit, position, initCost);
		}
		public List<Position> GetJumpablePos(Position position) {
			int initCost = 0;
			if (Moves.ContainsKey(position)) {//if they moved to get here
				initCost = Moves[position].Value;
			}
			return Board.GetJumpablePos(position, initCost);
		}

		//TODO can a unit charge when they jumped or climbed?

		public List<KeyValuePair<Position, int>> GetMoveablePos(Unit unit) {
			Position originalUnitPos = unit.Position;
			if (Moves.ContainsKey(originalUnitPos)) {//if the unit moved this turn
				originalUnitPos = Moves[originalUnitPos].Key;//make the function base it upon their original position
			}
			List<KeyValuePair<Position, int>> PossibleMoves = new List<KeyValuePair<Position, int>>();
			if (unit.InConflict) {
				return PossibleMoves;
			}
			List<List<int>> Distances = FindDistancesHelper(Board, unit, originalUnitPos);
			for (int i = 0; i < Distances.Count; i++) {
				for (int j = 0; j < Distances[i].Count; j++) {
					int Distance = Distances[i][j];
					Position position = new Position(i, j);
					Unit unitAtPos = Board.GetUnitAtPos(position);
					if (Distance != -1 && (unitAtPos == Config.NullUnit || unitAtPos == unit)) {
						PossibleMoves.Add(new KeyValuePair<Position, int>(position, Distance));
					}
				}
			}
			return PossibleMoves;
		}
		private List<List<int>> FindDistancesHelper(Board board, Unit unit, Position position) {
			List<List<int>> distances = new List<List<int>>();
			for (int i = 0; i < board.Rows; i++) {
				List<int> row = new List<int>();
				for (int j = 0; j < board.Columns; j++) {
					if (position.Row == i && position.Column == j) {
						row.Add(0);
					} else {
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
			for (int i = 0; i < surrpos.Count; i++) {
				int row = surrpos[i].Row;
				int col = surrpos[i].Column;
				Square newSquare = board.GetSquareAtPos(surrpos[i]);
				if (!newSquare.Terrain.IsStandable || (newSquare.Unit != Config.NullUnit && newSquare.Unit.Player != unit.Player)) {
					continue;
				}
				int newCost = currentCost + newSquare.Terrain.Speed;
				if (((newCost != -1 && distances[row][col] == -1) || newCost < distances[row][col]) && newCost <= unit.MovementLeft) {
					distances[row][col] = newCost;
					distances = FindDistances(board, distances, unit, surrpos[i]);
				}
			}
			return distances;
		}
		public List<Position> GetPossibleAttackPos(Position position, Player player) {
			List<Position> possibleattackpos = new List<Position>();
			List<Position> possiblepos = Board.GetSurroundingPos(position);
			for (int i = 0; i < possiblepos.Count; i++) {
				Unit unit = Board.GetUnitAtPos(possiblepos[i]);
				if (unit != Config.NullUnit && unit.Player != player) {
					possibleattackpos.Add(possiblepos[i]);
				}
			}
			return possibleattackpos;
		}
		public Dictionary<Position, List<List<Position>>> GetShotOptions(Position Shooter) {
			return Board.GetShotOptions(Shooter);
		}
	}
}