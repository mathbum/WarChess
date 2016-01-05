﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.TerrainObjs;

namespace WarChess.Objects {
	public class Board {//TODO have flyweight pattern for terrain objs and maybe squares if i can think of how
		List<List<Square>> board;
		public int Rows { get; private set; }
		public int Columns { get; private set; }
		private NullUnit NullUnit;

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
		public void SetUnit(Position position, Unit unit) {
			this.board[position.Row][position.Column].Unit = unit;
		}
		public bool isValidMove(Position originalPos, Position newPos) {//TODO fix this
			if (originalPos.Equals(newPos)||board[newPos.Row][newPos.Column].Unit!=this.NullUnit) {
				return false;
			}
			return true;
		}//TODO should i have a full refresh function
		public void MoveUnit(Position originalPos,Position newPos) {
			if (isValidMove(originalPos, newPos)) {
				Unit tempUnit = GetSquareAtPos(originalPos).Unit;
				SetUnit(newPos, tempUnit);
				SetUnit(originalPos, this.NullUnit);
			}
		}
		public Square GetSquareAtPos(Position position) {
			return board[position.Row][position.Column];
		}
	}
}
