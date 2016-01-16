using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarChess.Objects;

namespace Project1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

		private Game Game;
		private List<List<Label>> labels;
		private Button lastButton = null;//TODO should be able to be moved
		private Position SelectedPos = null;
		private List<Button> actionButtons = new List<Button>();
		private List<Rectangle> conflictRectangles = new List<Rectangle>();

		public MainWindow(Game Game) {
			InitializeComponent();
			this.Game = Game;
			int rows = this.Game.GetBoardRows();
			int cols = this.Game.GetBoardColumns();
			PlayerLabel.Content = this.Game.GetCurrentPlayer().Name;
			InitializeBoardGui(rows, cols);
			PopulateUnitPlacementGrid(this.Game.GetCurrentPlayer().UnitsToPlace.ToList());
		}
		private void PopulateUnitPlacementGrid(List<KeyValuePair<string, int>> UnitCount) {
			UnitGrid.Children.Clear();
			UnitGrid.RowDefinitions.Clear();
			UnitGrid.ColumnDefinitions.Clear();
			int unitButtonWidth = 75;
			int height = 25;
			int labelWidth = 45;
			ColumnDefinition gridCol = new ColumnDefinition();
			gridCol.Width = new GridLength(unitButtonWidth);
			UnitGrid.ColumnDefinitions.Add(gridCol);
			ColumnDefinition gridCol1 = new ColumnDefinition();
			gridCol1.Width = new GridLength(labelWidth);
			UnitGrid.ColumnDefinitions.Add(gridCol1);
			UnitGrid.Height = UnitCount.Count * height;
			for (int i = 0; i < UnitCount.Count; i++) {
				RowDefinition gridRow = new RowDefinition();
				gridRow.Height = new GridLength(height);
				UnitGrid.RowDefinitions.Add(gridRow);

				Label l2 = CreateLabel(labelWidth, height, UnitCount[i].Value.ToString(), UnitGrid, i, 1);
				Button b2 = CreateButton(unitButtonWidth, height, UnitCount[i].Key, dynClick, UnitGrid, i, 0);
				b2.Tag = l2;
			}
        }

		private void InitializeBoardGui(int rows, int cols) {
			int width = 75;
			int height = 100;
            grid.Width = cols * width;
            grid.Height = rows * height;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.ShowGridLines = true;
            grid.Background = new SolidColorBrush(Colors.Blue);

            //grid.RowDefinitions.Clear();
            for (int i = 0; i < rows; i++) {
                RowDefinition gridRow1 = new RowDefinition();
                gridRow1.Height = new GridLength(height);
                grid.RowDefinitions.Add(gridRow1);
            }
            //grid.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++) {
                ColumnDefinition gridCol1 = new ColumnDefinition();
                gridCol1.Width = new GridLength(width);
                grid.ColumnDefinitions.Add(gridCol1);
            }
			for (int i = 0; i < rows; i++) {
				for (int j = 0; j < cols; j++) {
					Image image = new Image();
					{
						image.Source = Game.GetTerrainAtPos(new Position(i, j)).Image;//TODO make this more eff?
						image.Stretch = Stretch.Fill;
					}
					Grid.SetRow(image, i);
					Grid.SetColumn(image, j);
					grid.Children.Add(image);
				}
			}

			labels = new List<List<Label>>();
            for (int i = 0; i < rows; i++) {
                List<Label> rowlabels = new List<Label>();
                for (int j = 0; j < cols; j++) {
					Label positionLbl = CreateLabel(30, 25, i.ToString() + ", " + j.ToString(), grid, i, j);
					positionLbl.VerticalAlignment = VerticalAlignment.Top;
					positionLbl.HorizontalAlignment = HorizontalAlignment.Left;
					Label l2 = CreateLabel(40, 25, "", grid, i, j);
                    rowlabels.Add(l2);					
                }
                labels.Add(rowlabels);
            }
			UpdateAllSquares();
        }
		public Button CreateButton(int width, int height, string content,RoutedEventHandler ClickEvent, Grid grid = null,int row = -1, int column = -1) {
			Button b = new Button(); {			
				b.Width = width;
				b.Height = height;
				b.Foreground = new SolidColorBrush(Colors.Black);
				b.Content = content;
				b.FontSize = 10;
				b.VerticalAlignment = VerticalAlignment.Center;
				b.HorizontalAlignment = HorizontalAlignment.Center;
				b.VerticalContentAlignment = VerticalAlignment.Center;
				b.HorizontalContentAlignment = HorizontalAlignment.Center;
			};
			b.Click += ClickEvent;
			if(grid!=null && row!=-1 && column != -1) {
				Grid.SetRow(b, row);
				Grid.SetColumn(b, column);
				grid.Children.Add(b);
			}
			return b;
		}
		public Label CreateLabel(int width, int height, string content, Grid grid = null, int row = -1, int column = -1) {
			Label lbl = new Label();
			{
				lbl.Width = width;
				lbl.Height = height;
				lbl.Foreground = new SolidColorBrush(Colors.White);
				lbl.Content = content;
				lbl.FontSize = 10;
				lbl.VerticalAlignment = VerticalAlignment.Center;
				lbl.HorizontalAlignment = HorizontalAlignment.Center;
				lbl.VerticalContentAlignment = VerticalAlignment.Center;
				lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
			}
			if (grid != null && row != -1 && column != -1) {
				Grid.SetRow(lbl, row);
				Grid.SetColumn(lbl, column);
				grid.Children.Add(lbl);
			}
			return lbl;
		}

		private void dynClick(object sender, RoutedEventArgs e) {
			lastButton = (Button)sender;
		}
		private Position GetPosOfClickedCell() {
			var point = Mouse.GetPosition(grid);

			int row = 0;
			int col = 0;
			double accumulatedHeight = 0.0;
			double accumulatedWidth = 0.0;

			// calc row mouse was over
			foreach (var rowDefinition in grid.RowDefinitions) {
				accumulatedHeight += rowDefinition.ActualHeight;
				if (accumulatedHeight >= point.Y)
					break;
				row++;
			}

			// calc col mouse was over
			foreach (var columnDefinition in grid.ColumnDefinitions) {
				accumulatedWidth += columnDefinition.ActualWidth;
				if (accumulatedWidth >= point.X)
					break;
				col++;
			}
			return new Position(row, col);
		}
		private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {//TODO fix problem where clicking a button runs this and the button code
			//if (e.ClickCount == 2) { // for double-click, remove this condition if only want single click

			//MessageBox.Show(string.Format("Grid clicked at row {0}, column {1}", row, col));
			// row and col now correspond Grid's RowDefinition and ColumnDefinition mouse was over when double clicked!
			Position position = GetPosOfClickedCell();
			UpdatePreview(position);
			//TODO only show preview for non null units
			if (Game.IsInSetup) {
				if (lastButton != null) {
					setguy(position);
				}
			} else {
				//Position lastSelectedPos = SelectedPos==null?null:new Position(SelectedPos.Row, SelectedPos.Column);
				//if (SelectedPos != null && SelectedPos.Equals(position)) {//deslecting
				//	labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
				//	SelectedPos = null;
				//} else if (Game.GetUnitAtPos(position).Player == Game.GetCurrentPlayer()) {//selecting your unit
				//	if (SelectedPos != null) {//previously had a different unit selected
				//		labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
				//	}
				//	SelectedPos = position;
				//	labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
				//} 				
				if (Game.Phase == Game.Phases.Move) {
					//if (lastSelectedPos != null) {//you used to have something selected
					//	RemoveGuiOptions();
					//}
					//if (SelectedPos != null) {
					//	if (SelectedPos.Equals(position)) {//selecting your unit
					//		DisplayGuiOptions();
					//	} else if (!SelectedPos.Equals(position)) {//moving your unit
					//		perfmove(position);//TODO trying to move a unit to a null position because it is supposed to be a jump.
					//}
					if (SelectedPos != null && SelectedPos.Equals(position)) {//deslecting
						labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
						SelectedPos = null;
						RemoveGuiOptions();
					} else if (Game.GetUnitAtPos(position).Player == Game.GetCurrentPlayer()) {//selecting your unit
						if (SelectedPos != null) {//previously had a different unit selected
							labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
							RemoveGuiOptions();
						}
						SelectedPos = position;
						labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
						DisplayGuiOptions();
					} else if (SelectedPos != null) {//moving your unit
						perfmove(position);//TODO trying to move a unit to a null position because it is supposed to be a jump.
					}
				}else if(Game.Phase == Game.Phases.Shoot) {
					if (SelectedPos != null && SelectedPos.Equals(position)) { //descecting your unit
						SelectedPos = null;
						RemoveGuiOptions();//shouldn't go through code to removemoveoptions
					} else if (Game.GetUnitAtPos(position).Player == Game.GetCurrentPlayer()) {//selecting your unit
						if (SelectedPos != null) {//previously had a different unit selected
							labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
							RemoveGuiOptions();
						}
						SelectedPos = position;
						labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);

						ShowShotOptions();
					} else if (SelectedPos != null) {
						//UpdateAllSquares();//no
						//ShowShot(position);
					}
				}			
				//} else if(Game.Phase == Game.Phases.Shoot) {
				//	if (lastSelectedPos != null && lastSelectedPos.Equals(position)) {//deslected
				//		//RemoveGuiOptions();
				//	} else if (SelectedPos.Equals(position)) {//selecting your unit
				//		//if (lastSelectedPos != null) {//previously had a different unit selected
				//		//	RemoveGuiOptions();
				//		//}
				//		//DisplayGuiOptions();
				//	} else if (!SelectedPos.Equals(position)) {//Clicked On Someone To Shoot
				//		ShowShot(position);
				//	}
				//}
			}
        }
		private void ShowShotOptions() {
			//make this like displayattack options
			List<Position> ShotOptions = Game.GetShotOptions(SelectedPos);
			for (int i = 0; i < ShotOptions.Count; i++) {
				Position pos = ShotOptions[i];
				Button b = CreateButton(45, 40, "Shoot", ShootTarget, grid, pos.Row, pos.Column);
				actionButtons.Add(b);
			}
		}
		private void ShootTarget(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position position = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.Shoot(SelectedPos, position);
			RemoveGuiOptions();//shouldn't go through code to removemoveoptions
			labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
		}
		private void ShowShot(Position Target) {
			List<List<Position>> ShotPathDetails = Game.GetShotPathDetails(SelectedPos,Target);//should always be len 3, good shot pos, iffy shot pos, bad shot pos
			Position pos;
			Color color;
			for(int i = 0; i < ShotPathDetails.Count; i++) {
				for(int j = 0; j < ShotPathDetails[i].Count; j++) {
					pos = ShotPathDetails[i][j];
					if (i == 0) {
						color = Colors.Blue;
					}else if (i == 1) {
						color = Colors.Yellow;
					}else{
						color = Colors.Violet;
					}
					labels[pos.Row][pos.Column].Background = new SolidColorBrush(color);
				}
			}
		}
        private void setguy(Position position) {
			if (Game.GetCurrentPlayer().HasUnitLeftToPlace(lastButton.Content.ToString())) { 
				Unit u = Game.CreateUnit(lastButton.Content.ToString());
				u.Player = Game.GetCurrentPlayer();
				if (Game.PlaceUnit(position,u)) {//if it was a legal placement of the unit					
					UpdateSquare(position);
					Label lastUnitCountLabel = (Label) lastButton.Tag;
					int count = int.Parse(lastUnitCountLabel.Content.ToString());
					lastUnitCountLabel.Content = count - 1;

					if (Game.GetCurrentPlayer().HasAnyUnitsLeftToPlace()) {
						return;
					}
					//if done placing					
					Game.EndTurn();
					UnitGrid.Children.Clear();
					PlayerLabel.Content = Game.GetCurrentPlayer().Name;
					UpdateAllSquares();
					if (this.Game.IsInSetup) {
						PopulateUnitPlacementGrid(Game.GetCurrentPlayer().UnitsToPlace.ToList());
						lastButton = null;
					} else {
						EndTurnButton.IsEnabled = true;
						PhaseLabel.Content = this.Game.Phase;
						MainGrid.Children.Remove(UnitGridScroller);
					}
				}	
			}
        }
		private void perfmove(Position position) {
			bool succ = Game.Move(SelectedPos, position);
			if (succ) {
				UpdateSquare(SelectedPos);
				UpdateSquare(position);
				SelectedPos = position;
				RemoveGuiOptions();
				DisplayGuiOptions();
				labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Black);
			}
		}
		private void DisplayJumpOptions(Position position) {
			List<Position> JumpOptions = Game.GetJumpablePos(position);
			for (int i = 0; i < JumpOptions.Count; i++) {//positions you can jump
				actionButtons.Add(CreateButton(45, 40, "Jump", JumpTerrain, grid, JumpOptions[i].Row, JumpOptions[i].Column));
			}
		}
		private void RemoveGuiOptions() {
			RemoveActionOptions();
			RemoveMoveOptions();
		}
		private void DisplayGuiOptions() {
			if (SelectedPos != null) {
				DisplayChargeOptions(SelectedPos);
				DisplayMoveOptions(SelectedPos);
				DisplayJumpOptions(SelectedPos);
			}
		}
		private void JumpTerrain(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position position = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			SelectedPos = Game.Jump(Game.GetUnitAtPos(SelectedPos),position);
			if (SelectedPos != null) {
				RemoveGuiOptions();
				DisplayGuiOptions();
			} else {//unit has died
				RemoveGuiOptions();
			}
		}
		private void RemoveMoveOptions() {
			UpdateAllSquares();
		}
		private void DisplayMoveOptions(Position position) {
			List<KeyValuePair<Position,int>> moves = Game.GetMoves(position);
			for (int i = 0; i < moves.Count; i++) {
				Position movePosition = moves[i].Key;
				if (movePosition.Equals(position)) {
					continue;
				}
				int cost = moves[i].Value;				
				Color color = Colors.Violet;//nullish
				if (cost == 0) {
					color = Colors.White;
				}else if (cost == 1) {
					color = Colors.GreenYellow;
				}else if (cost == 2) {
					color = Colors.Orange;
				}else if (cost == 3) {
					color = Colors.OrangeRed;
				}
				labels[movePosition.Row][movePosition.Column].Background = new SolidColorBrush(color);
			}
		}
		private void RemoveActionOptions() {
			for(int i = 0; i < actionButtons.Count; i++) {
				grid.Children.Remove(actionButtons[i]);
			}
			actionButtons.Clear();
		}
		private void DisplayChargeOptions(Position position) {
			List<List<Position>> positions = Game.GetPossibleAttackPos(position);//should only ever be 2 lists in possibleattacks
			List<Position> positionsattacked = positions[0];
			List<Position> attackablepositions = positions[1];
			for (int i = 0; i < positionsattacked.Count; i++) {//positions you attacked earlier this turn
				actionButtons.Add(CreateButton(45, 40, "Cancel", CancelCharge, grid, positionsattacked[i].Row, positionsattacked[i].Column));
			}
			for (int i = 0; i < attackablepositions.Count; i++) {//positions you can attack
				actionButtons.Add(CreateButton(45, 40, "Attack", ChargeUnit, grid, attackablepositions[i].Row, attackablepositions[i].Column));
			}
		}
		private void ChargeUnit(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.AddCharge(Game.GetUnitAtPos(defendingPosition), Game.GetUnitAtPos(SelectedPos));
			RemoveGuiOptions();
			DisplayChargeOptions(SelectedPos);//can't jump or move
			DisplayTempConflicts();
		}
		private void CancelCharge(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.RemoveCharge(Game.GetUnitAtPos(defendingPosition), Game.GetUnitAtPos(SelectedPos));
			RemoveGuiOptions();
			DisplayGuiOptions();
			DisplayTempConflicts();
		}

		private void UpdatePreview(Position position) {
			Unit unitatpos = Game.GetUnitAtPos(position);
			Namelabel.Content = unitatpos.Name;
			Pointslabellbl.Content = unitatpos.Points;
			Strengthlabellbl.Content = unitatpos.Strength;
			Defenselabellbl.Content = unitatpos.Defense;
			if (unitatpos == Config.NullUnit || unitatpos == null) {
				UnitPlayerLbl.Content = "None";
			} else {
				UnitPlayerLbl.Content = unitatpos.Player.Name;
			}
			SolidColorBrush color = new SolidColorBrush(Colors.Red);
			string Conflictlbl = "In Conflict";//TODO don't hardcode this
			if (!unitatpos.InConflict) {
				Conflictlbl = "Not In Conflict";
				color = new SolidColorBrush(Colors.Green);
			}
			InConflictLbl.Content = Conflictlbl;
			InConflictLbl.Background = color;
		}


		private void UpdateSquare(Position position) {
			Unit unitatpos = Game.GetUnitAtPos(position);
			Label labelatpos = labels[position.Row][position.Column];
			labelatpos.Content = unitatpos.Name;

			if(!(unitatpos == Config.NullUnit)) {
				if (unitatpos.Player == Game.GetCurrentPlayer()) {
					if (SelectedPos!=null && SelectedPos == position) {
						labelatpos.Background = new SolidColorBrush(Colors.Black);
					} else {
						labelatpos.Background = new SolidColorBrush(Colors.Green);
					}
				} else {
					labelatpos.Background = new SolidColorBrush(Colors.Red);
				}
			} else {
				labelatpos.Background = null;
			}
		}

		private void UpdateAllSquares() {
			Position position = new Position();
			for(int i = 0; i < labels.Count; i++) {
				position.Row = i;
				for (int j = 0; j < labels[i].Count; j++) {
					position.Column = j;
					UpdateSquare(position);
				}
			}
		}

		private void EndTurn_Click(object sender, RoutedEventArgs e) {
			Game.EndTurn();
			PlayerLabel.Content = Game.GetCurrentPlayer().Name;
			PhaseLabel.Content = Game.Phase;
			RemoveGuiOptions();//this also updates all squares
			SelectedPos = null;
			DisplayTempConflicts(); //TODO remove this. It is only here to remove tempconflicts at the begining of a new round
		}

		/// <summary>
		/// ////////////////////////////////////////////
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		private void ClearConflictsDisplay() {
			for(int i = 0; i < conflictRectangles.Count; i++) {
				grid.Children.Remove(conflictRectangles[i]);
			}
			conflictRectangles.Clear();
		}
		private void DisplayTempConflicts() {
			ClearConflictsDisplay();
			foreach(KeyValuePair<Unit,List<Unit>> conflictItem in Game.GetConflicts()) { 
				Position defenderpos = conflictItem.Key.Position;
				for (int j = 0; j < conflictItem.Value.Count; j++) {					
					Rectangle rect = new Rectangle();
					{
						rect.Stroke = new SolidColorBrush(Colors.Black);
						rect.Margin = new Thickness(10, 20, 10, 20);						
					}
					Position attackerpos = conflictItem.Value[j].Position;
					
					if (defenderpos.Row > attackerpos.Row) {//attacker is above
						rect.SetValue(Grid.RowSpanProperty, 2);
						Grid.SetRow(rect, attackerpos.Row);
						Grid.SetColumn(rect, attackerpos.Column);
					} else if (defenderpos.Column < attackerpos.Column) {//attacker is to the right
						rect.SetValue(Grid.ColumnSpanProperty, 2);
						Grid.SetRow(rect, defenderpos.Row);
						Grid.SetColumn(rect, defenderpos.Column);
					} else if (defenderpos.Row < attackerpos.Row) {//attacker is below
						rect.SetValue(Grid.RowSpanProperty, 2);
						Grid.SetRow(rect, defenderpos.Row);
						Grid.SetColumn(rect, defenderpos.Column);
					} else if (defenderpos.Column > attackerpos.Column) {//attacker is to the left
						rect.SetValue(Grid.ColumnSpanProperty, 2);
						Grid.SetRow(rect, attackerpos.Row);
						Grid.SetColumn(rect, attackerpos.Column);
					}
					grid.Children.Add(rect);
					conflictRectangles.Add(rect);
				}
			}							
		}
		private Position lastHover = null;
		private void grid_MouseMove(object sender, MouseEventArgs e) {//want to also be able to check shots in the movement phase. 
			if (Game.Phase == Game.Phases.Shoot) {
				if (SelectedPos != null) {
					Position pos = GetPosOfClickedCell();
					if ((lastHover == null || !lastHover.Equals(pos)) && !SelectedPos.Equals(pos)) {//also want to see if i have a good shot on my target buy its covered by button
						lastHover = pos;
						UpdateAllSquares();
						ShowShot(pos);
					}
				}
			}
		}
		/////////////////////////////////////////////////////////////////
	}
}