using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Board {
		List<List<Square>> board;
		public int rows { get; private set; }
		public int cols { get; private set; }

		public Board(int rows,int cols) {
			board = new List<List<Square>>();
			for(int i=0;i<rows; i++) {
				List<Square> row = new List<Square>();
				for (int j = 0; j < cols; j++) {
					row.Add(new Square());
				}
				board.Add(row);
			}
			this.rows = rows;
			this.cols = cols;
		}
		public void SetUnit(int row, int col, Unit unit) {
			this.board[row][col].Unit = unit;
		}
		//public bool isValidMove() {
		//	return false;
		//}
	}
}
