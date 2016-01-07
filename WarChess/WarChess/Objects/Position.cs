using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarChess.Objects {
	public class Position {
		public Position() {
			this.Row = -1;
			this.Column = -1;
		}
		public Position(int Row, int Col) {
			this.Row = Row;
			this.Column = Col;
		}
		public int Row { get; set; }
		public int Column { get; set; }
		public bool IsNull() {
			return Row < 0 || Column < 0;
		}
		public bool Equals(Position p) {
			if ((object)p == null) {
				return false;
			}
			return (Row == p.Row) && (Column == p.Column);
		}
	}
}