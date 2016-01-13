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
		private List<Button> attackButtons = new List<Button>();
		private List<Rectangle> conflictRectangles = new List<Rectangle>();
		private List<Button> JumpButtons = new List<Button>();

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
			UnitGrid.Children.Clear();//think i need this
			UnitGrid.RowDefinitions.Clear();
			UnitGrid.ColumnDefinitions.Clear();
			ColumnDefinition gridCol = new ColumnDefinition();
			gridCol.Width = new GridLength(75);
			UnitGrid.ColumnDefinitions.Add(gridCol);
			ColumnDefinition gridCol1 = new ColumnDefinition();
			gridCol1.Width = new GridLength(45);
			UnitGrid.ColumnDefinitions.Add(gridCol1);
			UnitGrid.Height = UnitCount.Count * 25;
			for (int i = 0; i < UnitCount.Count; i++) {
				RowDefinition gridRow = new RowDefinition();
				gridRow.Height = new GridLength(25);
				UnitGrid.RowDefinitions.Add(gridRow);

				Label l2 = new Label();
				{
					l2.Width = 45;
					l2.Height = 25;
					l2.Foreground = new SolidColorBrush(Colors.White);
					l2.Content = UnitCount[i].Value;
					l2.FontSize = 10;
					l2.VerticalAlignment = VerticalAlignment.Center;
					l2.HorizontalAlignment = HorizontalAlignment.Center;
					l2.VerticalContentAlignment = VerticalAlignment.Center;
					l2.HorizontalContentAlignment = HorizontalAlignment.Center;
				}

				Grid.SetRow(l2, i);
				Grid.SetColumn(l2, 1);
				UnitGrid.Children.Add(l2);

				Button b2 = new Button();
				{
					b2.Width = 75;
					b2.Height = 25;
					b2.Foreground = new SolidColorBrush(Colors.Black);
					b2.Content = UnitCount[i].Key;
					b2.FontSize = 10;
					b2.VerticalAlignment = VerticalAlignment.Center;
					b2.HorizontalAlignment = HorizontalAlignment.Center;
					b2.VerticalContentAlignment = VerticalAlignment.Center;
					b2.HorizontalContentAlignment = HorizontalAlignment.Center;
				}
				Grid.SetRow(b2, i);
				Grid.SetColumn(b2, 0);
				UnitGrid.Children.Add(b2);
				b2.Click += dynClick;
				b2.Tag = l2;
			}
        }

		private void InitializeBoardGui(int rows, int cols) {
            grid.Width = cols * 75;
            grid.Height = rows * 100;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.ShowGridLines = true;
            grid.Background = new SolidColorBrush(Colors.Blue);

            //grid.RowDefinitions.Clear();
            for (int i = 0; i < rows; i++) {
                RowDefinition gridRow1 = new RowDefinition();
                gridRow1.Height = new GridLength(100);
                grid.RowDefinitions.Add(gridRow1);
            }
            //grid.ColumnDefinitions.Clear();
            for (int i = 0; i < cols; i++) {
                ColumnDefinition gridCol1 = new ColumnDefinition();
                gridCol1.Width = new GridLength(75);
                grid.ColumnDefinitions.Add(gridCol1);
            }
            labels = new List<List<Label>>();
            for (int i = 0; i < rows; i++) {
                List<Label> rowlabels = new List<Label>();
                for (int j = 0; j < cols; j++) {
                    Label l1 = new Label();
                    {
                        l1.Width = 30;
                        l1.Height = 25;
                        l1.Foreground = new SolidColorBrush(Colors.White);
                        l1.Content = i.ToString() + ", " + j.ToString();
                        l1.Margin = new Thickness(0, 0, 0, 0);
                        l1.VerticalAlignment = VerticalAlignment.Top;
                        l1.HorizontalAlignment = HorizontalAlignment.Left;
                    }
                    Grid.SetRow(l1, i);
                    Grid.SetColumn(l1, j);
                    grid.Children.Add(l1);
                    Label l2 = new Label();
                    {
                        l2.Width = 40;
                        l2.Height = 25;
                        l2.Foreground = new SolidColorBrush(Colors.White);
                        l2.Margin = new Thickness(0, 0, 0, 0);
                        l2.VerticalAlignment = VerticalAlignment.Center;
                        l2.HorizontalAlignment = HorizontalAlignment.Center;

                    }
                    rowlabels.Add(l2);					
					Grid.SetRow(l2, i);
                    Grid.SetColumn(l2, j);
                    grid.Children.Add(l2);
                }
                labels.Add(rowlabels);
            }
			UpdateAllSquares();
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
		private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
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
				if (Game.Phase == Game.Phases.Move) {					
					if (SelectedPos!=null && SelectedPos.Equals(position)) {//deslecting
						labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
						SelectedPos = null;
						RemoveChargeOptions();
						RemoveMoveOptions();
					} else if (Game.GetUnitAtPos(position).Player == Game.GetCurrentPlayer()) {//selecting your unit
						if (SelectedPos != null) {//previously had a different unit selected
							labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
							RemoveChargeOptions();
							RemoveMoveOptions();
							RemoveJumpOptions();
						}
						SelectedPos = position;	
						labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
						DisplayChargeOptions(SelectedPos);
						DisplayMoveOptions(SelectedPos);
						DisplayJumpOptions(SelectedPos);
					} else if (SelectedPos != null) {//moving your unit
						perfmove(position);
					}
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
				//labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Black);
				RemoveChargeOptions();				
				DisplayChargeOptions(SelectedPos);
				RemoveMoveOptions();
				DisplayMoveOptions(SelectedPos);
				RemoveJumpOptions();
				DisplayJumpOptions(SelectedPos);
				labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Black);//HACK
			}
		}
		private void RemoveJumpOptions() {
			for (int i = 0; i < JumpButtons.Count; i++) {
				grid.Children.Remove(JumpButtons[i]);
			}
			JumpButtons.Clear();
		}
		private void DisplayJumpOptions(Position position) {
			List<Position> JumpOptions = Game.GetJumpablePos(position);
			for (int i = 0; i < JumpOptions.Count; i++) {//positions you can attack
				Button b = new Button();
				{
					b.Foreground = new SolidColorBrush(Colors.Black);
					b.Content = "Jump";
					b.Margin = new Thickness(15, 30, 15, 30);
				}
				Grid.SetRow(b, JumpOptions[i].Row);
				Grid.SetColumn(b, JumpOptions[i].Column);
				grid.Children.Add(b);
				b.Click += JumpTerrain;
				JumpButtons.Add(b);
			}
		}
		private void JumpTerrain(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position position = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			SelectedPos = Game.Jump(Game.GetUnitAtPos(SelectedPos),position);
			if (SelectedPos != null) {
				UpdateAllSquares();//also get new movement options?
				RemoveChargeOptions();
				DisplayChargeOptions(SelectedPos);
				RemoveMoveOptions();
				DisplayMoveOptions(SelectedPos);
				DisplayTempConflicts();
				RemoveJumpOptions();
				DisplayJumpOptions(SelectedPos);
			}
		}
		private void RemoveMoveOptions() {
			UpdateAllSquares();
			//for (int i = 0; i < labels.Count; i++) {
			//	for (int j = 0; j < labels[i].Count; j++) {
			//		Position position = new Position(i, j);
			//		Color color = Colors.Green;
			//		if(Game.GetUnitAtPos(position).Player==Game.GetCurrentPlayer()
			//		labels[i][j].Background = null;
			//	}
			//}
		}
		private void DisplayMoveOptions(Position position) {
			List<KeyValuePair<Position,int>> moves = Game.GetMoves(position);
			for (int i = 0; i < moves.Count; i++) {
				int cost = moves[i].Value;
				Position movePosition = moves[i].Key;
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
		private void RemoveChargeOptions() {
			for(int i = 0; i < attackButtons.Count; i++) {
				grid.Children.Remove(attackButtons[i]);
			}
			attackButtons.Clear();
		}
		private void DisplayChargeOptions(Position position) {
			List<List<Position>> positions = Game.GetPossibleAttackPos(position);//should only ever be 2 lists in possibleattacks
			List<Position> positionsattacked = positions[0];
			List<Position> attackablepositions = positions[1];
			for (int i = 0; i < positionsattacked.Count; i++) {//positions you attacked earlier this turn
				
				Button b = new Button();
				{
					b.Foreground = new SolidColorBrush(Colors.Black);
					b.Content = "Cancel";
					b.Margin = new Thickness(15, 30, 15, 30);
				}
				Grid.SetRow(b, positionsattacked[i].Row);
				Grid.SetColumn(b, positionsattacked[i].Column);
				grid.Children.Add(b);
				b.Click += CancelCharge;
				attackButtons.Add(b);
			}
			for (int i = 0; i < attackablepositions.Count; i++) {//positions you can attack
				Button b = new Button();
				{
					b.Foreground = new SolidColorBrush(Colors.Black);
					b.Content = "Attack";
					b.Margin = new Thickness(15, 30, 15, 30);
				}
				Grid.SetRow(b, attackablepositions[i].Row);
				Grid.SetColumn(b, attackablepositions[i].Column);
				grid.Children.Add(b);
				b.Click += ChargeUnit;
				attackButtons.Add(b);
			}
		}
		private void ChargeUnit(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.AddCharge(Game.GetUnitAtPos(defendingPosition), Game.GetUnitAtPos(SelectedPos));
			RemoveChargeOptions();
			DisplayChargeOptions(SelectedPos);
			RemoveMoveOptions();
			//DisplayMoveOptions(SelectedPos);
			DisplayTempConflicts();
			RemoveJumpOptions();
			//DisplayJumpOptions(SelectedPos);
		}
		private void CancelCharge(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.RemoveCharge(Game.GetUnitAtPos(defendingPosition), Game.GetUnitAtPos(SelectedPos));
			RemoveChargeOptions();
			DisplayChargeOptions(SelectedPos);
			RemoveMoveOptions();
			DisplayMoveOptions(SelectedPos);
			DisplayTempConflicts();
			RemoveJumpOptions();
			DisplayJumpOptions(SelectedPos);
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
					labelatpos.Background = new SolidColorBrush(Colors.Green);
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
			UpdateAllSquares();
			RemoveChargeOptions(); //this only need to happen at the end of a move phase
			RemoveMoveOptions();
			SelectedPos = null;
			DisplayTempConflicts();
			//labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);//need to clear selected unit display?
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
		/////////////////////////////////////////////////////////////////
	}
}