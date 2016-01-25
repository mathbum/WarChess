using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarChess.Objects.Items;

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
			if (GetUnitAtPos(originalPos).InConflict) {//if unit is in conflict
				return false;
			}
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
		public void ResetUnitTempStats() {
			for(int i = 0; i < Rows; i++) {
				for(int j=0;j< Columns; j++) {
					Unit unit = GetUnitAtPos(new Position(i, j));
					unit.MovementLeft = unit.MaxMoveDist;
					unit.HasShot = false;
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
						//unit = null;//this a proper way to destroy the unit object?
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
		public bool IsValidPlacement(Position position) {//TODO finish this. with legal placement areas etc...
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
		public Dictionary<Position,List<List<Position>>> GetShotOptions(Position Shooter) {
			Unit unit = GetUnitAtPos(Shooter);
			Dictionary<Position, List<List<Position>>> ShotOptions = new Dictionary<Position, List<List<Position>>>();
			RangedWeapon rangedWeapon = unit.GetRangedWeapon();
			if (rangedWeapon != null) {//should always be true. because game should only forward requests of units with a ranged weapon
				for (int i = 0; i < Rows; i++) {
					for (int j = 0; j < Columns; j++) {
						Square square = board[i][j];
						if (square.Unit != Config.NullUnit && square.Unit.Player != unit.Player) {
							Position pos = new Position(i, j);
							List<List<Position>> ShotDetails = GetShotPathDetails(Shooter, pos);
							if (ShotDetails[2].Count == 0 && rangedWeapon.Range >= Shooter.Distance(pos) - Utils.epsilon) {
								ShotOptions[pos] = ShotDetails;
							}
						}
					}
				}
				return ShotOptions;
			}
			throw new ArgumentException();
		}
		//TODO can't shoot if path is obstrubted by obsticule (or unit) or conflict (if good), out of range. Also can shoot if friendly is directly next to you (just like terrain) (rest is taken care of by game)
		public List<List<Position>> GetShotPathDetails(Position Shooter, Position Target) {//three lists, first is good positions you can shoot over, second is obj in way pos, third is restrictions pos
			List<Position> goodPos = new List<Position>();
			List<Position> objInWayPos = new List<Position>();
			List<Position> badPos = new List<Position>();
			List<List<Position>> PathDetails = new List<List<Position>>() { goodPos, objInWayPos, badPos };

			List<Position> path = GetShotPath(Shooter, Target);			
			Unit ShootingUnit = GetUnitAtPos(Shooter);
			for (int i = 0; i < path.Count; i++) {
				Square square = GetSquareAtPos(path[i]);
				if (!square.Terrain.SeeThrough || (ShootingUnit.Allegiance == Config.Allegiance.Good && ((i != 0 &&  square.Unit.Player == ShootingUnit.Player) || square.Unit.InConflict))) {//TODO or you are a good unit shooing into a conflict an ALLY IS IN
					badPos.Add(path[i]);//if the terrain isn't see through, or if you are good and an ally unit is in way or someone in conflict
				} else if ((!square.Terrain.IsStandable || (square.Unit != Config.NullUnit && !path[i].Equals(Target)) || (ShootingUnit.Allegiance==Config.Allegiance.Evil && square.Unit.InConflict)) && i != 0) {
					objInWayPos.Add(path[i]);//if terrain is in way, or a unit is in way. (either enemy unit or you are evil [we know this bc of previous statement]) and it isn't the square in front of you
				} else {
					goodPos.Add(path[i]);
				}
			}
			return PathDetails;
		}
		//RayCasting
		//http://www.saltgames.com/article/lineOfSight/		http://www.saltgames.com/articles/lineOfSight/lineOfSightDemo.js this is what i did
		//http://www.roguebasin.com/index.php?title=Raycasting_in_python
		//FOV
		//http://www.roguebasin.com/index.php?title=Eligloscode

		//http://www.cs.yorku.ca/~amana/research/grid.pdf
		private List<Position> GetShotPath(Position Shooter, Position Target) {
			int rowDiff = Target.Row - Shooter.Row;
			int colDiff = Target.Column - Shooter.Column;
			List<Position> path = new List<Position>();
			if (rowDiff == 0 || colDiff == 0 || Math.Abs(rowDiff) == Math.Abs(colDiff)) {//shooting straight up/down or left/right or diag						
				int rowSign = Math.Sign(rowDiff);//if positive then shooting up, if negative then shooing down
				int colSign = Math.Sign(colDiff);//if positive then shooting left, if negative then shooting right
				Position currentPos = Shooter;
				for (;;) {
					currentPos = new Position(currentPos.Row + rowSign, currentPos.Column + colSign);
					path.Add(currentPos);
					if (currentPos.Equals(Target)) {
						break;
					}
					
				}
			} else {
				// change from step to step
				double tValue, tForNextBorderX, tForNextBorderY;
				int xGrid, yGrid;

				// constant throughout raycast
				double xDiff, yDiff, tForOneX, tForOneY;
				int xStep, yStep;

				double xEnd = Target.Column + .5;//add .5 so the los is from center of square
				double xStart = Shooter.Column + .5;
				double yEnd = Target.Row + .5;
				double yStart = Shooter.Row + .5;

				xDiff = xEnd - xStart;
				yDiff = yEnd - yStart;
				tForOneX = Math.Abs(1.0 / xDiff);
				tForOneY = Math.Abs(1.0 / yDiff);
				yStep = Math.Sign(yDiff);
				xStep = Math.Sign(xDiff);

				tValue = 0;
				xGrid = (int) Math.Floor(xStart);
				yGrid = (int) Math.Floor(yStart);

				double fracStartPosX = xStart - Math.Floor(xStart);
				if (xDiff > 0) {
					tForNextBorderX = (1 - fracStartPosX) * tForOneX;
				} else {
					tForNextBorderX = fracStartPosX * tForOneX;
				}

				double fracStartPosY = yStart - Math.Floor(yStart);
				if (yDiff > 0) {
					tForNextBorderY = (1 - fracStartPosY) * tForOneY;
				} else {
					tForNextBorderY = fracStartPosY * tForOneY;
				}
				while (tValue <= 1.0) {
					path.Add(new Position((int)yGrid, (int)xGrid));
					//if (Math.Abs(tForNextBorderX - tForNextBorderY) < Utils.epsilon) {
					//	// diagonal step (normally not included in a raycast)
					//	tValue = tForNextBorderX;
					//	tForNextBorderX += tForOneX;
					//	tForNextBorderY += tForOneY;
					//	xGrid += xStep;
					//	yGrid += yStep;
					/*} else */
					if (tForNextBorderX <= tForNextBorderY) {
						// step in x
						tValue = tForNextBorderX;
						tForNextBorderX += tForOneX;
						xGrid += xStep;
					} else {
						// step in y
						tValue = tForNextBorderY;
						tForNextBorderY += tForOneY;
						yGrid += yStep;
					}
				}
				path.Remove(path[0]);//remove shooter from the list

				////*********************RayCast 1**********************
				////int x1 = Target.Column;
				////int y1 = Target.Row;
				////int x0 = Shooter.Column;
				////int y0 = Shooter.Row;
				////double xDist = Math.Abs(x1 - x0);
				////double yDist = -Math.Abs(y1 - y0);
				////int xStep = (x0 < x1 ? +1 : -1);
				////int yStep = (y0 < y1 ? +1 : -1);
				////double error = xDist + yDist - .5;
				//////xDist -= .5;
				////if (Math.Abs(xDist) < Math.Abs(yDist)) {
				////	error += 1;
				////	//xDist += .5;
				////	//yDist += .5;
				////}

				////path.Add(new Position(y0, x0));

				////while (x0 != x1 || y0 != y1) {
				////	if (2 * error - yDist > xDist - 2 * error) {//going to the major for too long... why?
				////												// horizontal step
				////		error += yDist;
				////		x0 += xStep;
				////	} else {
				////		// vertical step
				////		error += xDist;
				////		y0 += yStep;
				////	}

				////	path.Add(new Position(y0, x0));
				////}
				////************************END RayCast 1*********************

				////**************************Bresenham*******************
				////int x2 = Target.Column;
				////int y2 = Target.Row;
				////int x = Shooter.Column;
				////int y = Shooter.Row;
				////int cDiff = x2 - x;
				////int rDiff = y2 - y;
				////rowSign = Math.Sign(rDiff);
				////colSign = Math.Sign(cDiff);
				////int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
				////dx1 = colSign;
				////dy1 = rowSign;
				////dx2 = colSign;
				////int longest = Math.Abs(cDiff);
				////int shortest = Math.Abs(rDiff);
				////int count = 0;
				////if (longest < shortest) {//can't be equal, otherwise <=
				////	longest = Math.Abs(rDiff);
				////	shortest = Math.Abs(cDiff);
				////	dy2 = rowSign;
				////	dx2 = 0;
				////}
				////List<Position> diags = new List<Position>();
				////int numerator = longest >> 1;
				////for (int i = 0; i <= longest; i++) {
				////	path.Add(new Position(y, x));
				////	count++;
				////	numerator += shortest;
				////	if (numerator >= longest) {
				////		numerator -= longest;
				////		x += dx1;
				////		y += dy1;
				////	} else {//can't ever make diag move
				////		x += dx2;
				////		y += dy2;
				////	}
				////}
				////**************************END Bresenham*******************

				////***********************************My Alg 2**********************
				//double slope = (double)(colDiff + colSign) / (double)(rowDiff + rowSign);
				//int count = 1;
				//int majorRow = 0;
				//int majorCol = colSign;
				//int minorRow = rowSign;
				//int minorCol = 0;
				//double req = (double)(rowDiff + rowSign) / (double)(colDiff + colSign);
				//int minorMoves = Math.Abs(rowDiff);
				//int majorMoves = Math.Abs(colDiff);
				//int minorMovesMade = 1;
				//Position currentPos = Shooter;
				//if (Math.Abs(rowDiff) > Math.Abs(colDiff - colSign)) {
				//	//slope = (double)(rowDiff + rowSign)/ (double)(colDiff + colSign);
				//	majorRow = rowSign;
				//	majorCol = 0;
				//	minorRow = 0;
				//	minorCol = colSign;
				//	minorMoves = Math.Abs(colDiff);
				//	majorMoves = Math.Abs(rowDiff);
				//}

				//for (;;) {
				//	if (slope * count >= req) {
				//		minorMovesMade++;
				//		req = req * minorMovesMade;
				//		currentPos = new Position(currentPos.Row - minorRow, currentPos.Column - minorCol);
				//	} else {
				//		currentPos = new Position(currentPos.Row - majorRow, currentPos.Column - majorCol);
				//	}
				//	path.Add(currentPos);
				//	count++;
				//	if (req > majorMoves) {
				//		break;
				//	}
				//}
				////***********************************END My Alg 2**********************

				//********************My Alg Discrete*******************
				////	int numOfMinorMoves = Math.Abs(rowDiff);//default values to of left/right is the major movement (target is further left/right than up/down)				
				////	int slope = Math.Abs(colDiff / rowDiff);
				////	int remainder = Math.Abs(colDiff % rowDiff);
				////	int majorRow = 0;
				////	int majorCol = colSign;
				////	int minorRow = rowSign;
				////	int minorCol = 0;

				////	if (Math.Abs(rowDiff) > Math.Abs(colDiff - colSign)) {
				////		numOfMinorMoves = Math.Abs(colDiff);
				////		slope = Math.Abs(rowDiff / colDiff);
				////		remainder = Math.Abs(rowDiff % colDiff);
				////		majorRow = rowSign;
				////		majorCol = 0;
				////		minorRow = 0;
				////		minorCol = colSign;
				////	}
				////	Position currentPos = Shooter;
				////	for (int i = 0; i < numOfMinorMoves; i++) {
				////		int currentSlope = slope;
				////		if ((numOfMinorMoves - remainder) <= i) {
				////			currentSlope = slope + 1;
				////			remainder--;
				////		}
				////		//else if (i == 0 && currentSlope != 1) {
				////		//	currentSlope--;
				////		//}
				////		for (int j = 0; j < currentSlope; j++) {
				////			currentPos = new Position(currentPos.Row - majorRow, currentPos.Column - majorCol);
				////			path.Add(currentPos);
				////			if (j == currentSlope / 2) {
				////				currentPos = new Position(currentPos.Row - minorRow, currentPos.Column - minorCol);
				////				path.Add(currentPos);
				////			}
				////		}
				////	}
				////}
				//******************END My Alg 1***************
			}
			return path;
		}
	}
}
