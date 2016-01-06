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
		private List<List<Label>> labels;//TODO see if i can get rid of alot of these
		private Button lastButton = null;//TODO should be able to be moved
		private Label lastUnitCountLabel = null;//TODO should be able to be moved
		private Position lastGridPositionClicked;
		private bool isselected = false;
		Dictionary<Button, Label> UnitPlacementBtnLblMap;//TODO should this be dict or list?
													//i only need this before the board is set. So when the main window is broken into two windows break this off

		public MainWindow(Game Game) {
			InitializeComponent();
			this.Game = Game;
			int rows = this.Game.Board.Rows;
			int cols = this.Game.Board.Columns;
			PlayerLabel.Content = this.Game.GetCurrentPlayer().Name;
			InitializeBoardGui(rows, cols);
			PopulateUnitPlacementGrid(this.Game.GetCurrentPlayer().UnitsToPlace.ToList());
		}
		private void PopulateUnitPlacementGrid(List<KeyValuePair<string, int>> UnitCount) {
			UnitGrid.Children.Clear();//think i need this
			UnitGrid.RowDefinitions.Clear();
			UnitGrid.ColumnDefinitions.Clear();
			UnitPlacementBtnLblMap = new Dictionary<Button, Label>();
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

				Button b2 = new Button();
				{
					b2.Width = 75;
					b2.Height = 25;
					b2.Foreground = new SolidColorBrush(Colors.Black);
					b2.Content = UnitCount[i].Key;
					b2.FontSize = 10;
					//l2.Margin = new Thickness(0, 0, 0, 0);
					b2.VerticalAlignment = VerticalAlignment.Center;
					b2.HorizontalAlignment = HorizontalAlignment.Center;
					b2.VerticalContentAlignment = VerticalAlignment.Center;
					b2.HorizontalContentAlignment = HorizontalAlignment.Center;
				}
				Grid.SetRow(b2, i);
				Grid.SetColumn(b2, 0);
				UnitGrid.Children.Add(b2);
				b2.Click += dynClick;				

				Label l2 = new Label();
				{
					l2.Width = 45;
					l2.Height = 25;
					l2.Foreground = new SolidColorBrush(Colors.White);
					l2.Content = UnitCount[i].Value;
					l2.FontSize = 10;
					//l2.Margin = new Thickness(0, 0, 0, 0);
					l2.VerticalAlignment = VerticalAlignment.Center;
					l2.HorizontalAlignment = HorizontalAlignment.Center;
					l2.VerticalContentAlignment = VerticalAlignment.Center;
					l2.HorizontalContentAlignment = HorizontalAlignment.Center;
				}

				Grid.SetRow(l2, i);
				Grid.SetColumn(l2, 1);
				UnitGrid.Children.Add(l2);
				UnitPlacementBtnLblMap[b2]= l2;
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
			lastGridPositionClicked = new Position(-1, -1);
        }

		private void dynClick(object sender, RoutedEventArgs e) {
			lastButton = (Button)sender;
			lastUnitCountLabel = UnitPlacementBtnLblMap[lastButton];
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
			Unit unitatpos = Game.Board.GetSquareAtPos(position).Unit;
			Namelabel.Content = unitatpos.Name;
			Pointslabellbl.Content = unitatpos.Points;
			Strengthlabellbl.Content = unitatpos.Strength;
			Defenselabellbl.Content = unitatpos.Defense;
			if (unitatpos is NullUnit || unitatpos==null) {
				UnitPlayerLbl.Content = "None";
			} else {				
				UnitPlayerLbl.Content = unitatpos.Player.Name;
			}
			


			if (Game.IsInSetup) {
				if (lastButton != null) {
					setguy(position);
				}
			} else {
				if (Game.Phase == Game.Phases.Move) {
					if (lastGridPositionClicked != null && isselected) {
						perfmove(position);
					} else {
						isselected = true;
						lastGridPositionClicked = position;
						labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
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
					int count = int.Parse(lastUnitCountLabel.Content.ToString());
					lastUnitCountLabel.Content = count - 1;

					if (Game.GetCurrentPlayer().HasAnyUnitsLeftToPlace()) {
						return;
					}
					//if done placing
					
					this.Game.EndTurn();
					UnitGrid.Children.Clear();
					PlayerLabel.Content = Game.GetCurrentPlayer().Name;
					UpdateAllSquares();
					if (this.Game.IsInSetup) {
						PopulateUnitPlacementGrid(Game.GetCurrentPlayer().UnitsToPlace.ToList());
					} else {
						PhaseLabel.Content = this.Game.Phase;
						MainGrid.Children.Remove(UnitGridScroller);
					}
				}	
			}
        }
		private void perfmove(Position position) {
			Game.Move(lastGridPositionClicked, position);
			UpdateSquare(lastGridPositionClicked);
			UpdateSquare(position);
			isselected = false;
		}

		private void UpdateSquare(Position position) {
			Unit unitatpos = Game.Board.GetSquareAtPos(position).Unit;
			Label labelatpos = labels[position.Row][position.Column];
			labelatpos.Content = unitatpos.Name;
			if(!(unitatpos is NullUnit)) {
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
		}
	}
}

        //private void button_Click(object sender, RoutedEventArgs e) {
        //    label.Content = "hello";
        //    Button btn = new Button();{
        //        btn.Name = "mybutton";
        //        btn.Height = 20;
        //        btn.Width = 50;
        //        btn.Foreground = new SolidColorBrush(Colors.White);
        //        btn.Content = "btnnum" + num.ToString();
        //        btn.Tag = num;
        //        btn.Content = "Browse-" + num.ToString();
        //        btn.Margin = new Thickness(0, 0, 0, 0);
        //        //btn.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
        //    }
        //    Grid.SetRow(btn, 0);
        //    Grid.SetColumn(btn, num);
        //    grid.Children.Add(btn);
        //    btn.Click += btn_Click;
        //    num++;
        //}    
        //void btn_Click(object sender, RoutedEventArgs e) {
        //    label.Content = "works?";
        //}