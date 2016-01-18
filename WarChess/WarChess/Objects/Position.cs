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
		public double Distance(Position pos) {
			int rowDiff = Math.Abs(Row - pos.Row);
			int colDiff = Math.Abs(Column - pos.Column);
			return Math.Sqrt(Math.Pow(rowDiff,2) + Math.Pow(colDiff, 2));
		}
		public override bool Equals(Object p) {
			if ((object)p == null) {
				return false;
			}
			Position pos = (Position)p;
			return (Row == pos.Row) && (Column == pos.Column);
		}
		public override int GetHashCode() {
			unchecked // Overflow is fine, just wrap
			{
				int hash = 17;
				// Suitable nullity checks etc, of course :)
				hash = hash * 29 + Row.GetHashCode();
				hash = hash * 29 + Column.GetHashCode();
				return hash;
			}
			//return base.GetHashCode();
		}
	}
}