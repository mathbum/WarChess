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
		private bool finishsetup = false;//TODO should be able to replace this with just using the current phase. but again this wont be needed once setup is moved to a seperate window
		private Button lastButton = null;//TODO should be able to be moved
		private Label lastUnitCountLabel = null;//TODO should be able to be moved
		private Position lastGridPositionClicked;
		private bool isselected = false;
		Dictionary<Button, Label> UnitPlacementList;//TODO should this be dict or list?
		//i only need this before the board is set. So when the main window is broken into two windows break this off

		public MainWindow(Game Game,List<KeyValuePair<string,int>> UnitCount) {
            InitializeComponent();
			this.Game = Game;
			int rows = this.Game.Board.Rows;
			int cols = this.Game.Board.Columns;
			init(rows,cols);

			UnitPlacementList = new Dictionary<Button, Label>();
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
				UnitPlacementList[b2]= l2;
			}
        }

		private void init(int rows, int cols) {
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
                        //l2.Content = "blank";
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
			UpdateAllLabels();
			lastGridPositionClicked = new Position(-1, -1);
        }

		private void dynClick(object sender, RoutedEventArgs e) {
			lastButton = (Button)sender;
			lastUnitCountLabel = UnitPlacementList[lastButton];
		}
		private Position GetRowColOfClickedCell() {
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
			Position position = GetRowColOfClickedCell();

			if (!finishsetup) {
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
        private void setguy(Position position) {//TODO can't place one guy over another. if so return the guy you overwrote back to your "hand"?
			if (Int32.Parse(lastUnitCountLabel.Content.ToString()) > 0) {
				this.Game.Board.SetUnit(position, Config.GoodUnits[lastButton.Content.ToString()]);
				UpdateLabel(position);
				int count = Int32.Parse(lastUnitCountLabel.Content.ToString());
				lastUnitCountLabel.Content = count - 1;

				bool isdoneplacing= true;
				List<KeyValuePair<Button, Label>> dictList = UnitPlacementList.ToList();
				for(int i = 0; i < dictList.Count; i++) {
					if (Int32.Parse(dictList[i].Value.Content.ToString()) > 0) {
						isdoneplacing = false;
					}
				}
				if (isdoneplacing) { 
					finishsetup = true;
					Game.Phase = Game.Phases.Priority;//TODO this shouldn't be hardcoded
					PhaseLabel.Content = Game.Phase;
				}
			}
        }
		private void perfmove(Position position) {
			this.Game.Board.MoveUnit(lastGridPositionClicked, position);
			UpdateLabel(lastGridPositionClicked);
			UpdateLabel(position);
			isselected = false;
			labels[lastGridPositionClicked.Row][lastGridPositionClicked.Column].Background = null;
		}

		private void UpdateLabel(Position position) {
			labels[position.Row][position.Column].Content = this.Game.Board.GetSquareAtPos(position).Unit.Name;
		}

		private void UpdateAllLabels() {
			Position position = new Position();
			for(int i = 0; i < labels.Count; i++) {
				position.Row = i;
				for (int j = 0; j < labels[i].Count; j++) {
					position.Column = j;
					UpdateLabel(position);
				}
			}
		}

		private void EndPhase_Click(object sender, RoutedEventArgs e) {
			Game.NextTurn();
			PhaseLabel.Content = Game.Phase;
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