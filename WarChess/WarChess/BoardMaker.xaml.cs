using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;
using WarChess.Objects;

namespace Project1 {
    /// <summary>
    /// Interaction logic for SetUp.xaml
    /// </summary>
    public partial class BoardMaker : Window {
        public BoardMaker() {
            InitializeComponent();
			init();
        }

		public List<string> board = new List<string>();
		private List<char> TerrainOps = new List<char>();
		private List<List<Image>> images = new List<List<Image>>();
		private int lastSelected = -1;

		private void Regenerate_Click(object sender, RoutedEventArgs e) {
			int rows;
			int cols;
			board.Clear();
			if ((bool)BoardLoaderRad.IsChecked) {
				List<string> tempboard = Config.Boards[BoardLoader.Text];
				for(int i = 0; i < tempboard.Count; i++) {
					board.Add(tempboard[i]);
				}
				rows = board.Count;
				cols = board[0].Length;
			} else {
				rows = int.Parse(trows.Text);
				cols = int.Parse(tcols.Text);
				for(int i=0;i< rows; i++) {
					string row = "";
					for(int j=0;j< cols; j++) {
						row += " ";
					}
					board.Add(row);
				}
			}

			PlacementGrid.Children.Clear();
			images.Clear();
			int width = 75;
			int height = 75;
			PlacementGrid.Width = cols * width;
			PlacementGrid.Height = rows * height;
			PlacementGrid.HorizontalAlignment = HorizontalAlignment.Left;
			PlacementGrid.VerticalAlignment = VerticalAlignment.Top;
			PlacementGrid.ShowGridLines = true;
			PlacementGrid.Background = new SolidColorBrush(Colors.Blue);

			PlacementGrid.RowDefinitions.Clear();
			for (int i = 0; i < rows; i++) {
				RowDefinition gridRow1 = new RowDefinition();
				gridRow1.Height = new GridLength(height);
				PlacementGrid.RowDefinitions.Add(gridRow1);
			}
			PlacementGrid.ColumnDefinitions.Clear();
			for (int i = 0; i < cols; i++) {
				ColumnDefinition gridCol1 = new ColumnDefinition();
				gridCol1.Width = new GridLength(width);
				PlacementGrid.ColumnDefinitions.Add(gridCol1);
			}
			for (int i = 0; i < rows; i++) {
				List<Image> row = new List<Image>();
				for (int j = 0; j < cols; j++) {
					Image image = new Image();
					{
						image.Source = Config.TerrainObjs[board[i][j]].Image;
						image.Stretch = Stretch.Fill;
					}
					Grid.SetRow(image, i);
					Grid.SetColumn(image, j);
					row.Add(image);
					PlacementGrid.Children.Add(image);
				}
				images.Add(row);
			}
		}
		private Position GetPosOfClickedCell(Grid grid) {
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
			if (e.ClickCount == 2) { // for double-click, remove this condition if only want single click
				// row and col now correspond Grid's RowDefinition and ColumnDefinition mouse was over when double clicked!
				Position position = GetPosOfClickedCell((Grid) sender);
				if (lastSelected != -1) {
					Terrain terrain = Config.TerrainObjs[TerrainOps[lastSelected]];
					images[position.Row][position.Column].Source = terrain.Image;
					string s = board[position.Row];
					char[] array = s.ToCharArray();
					array[position.Column] = TerrainOps[lastSelected];
					board[position.Row] = new string(array);

				}
			}
		}
		
		private void init() {
			TerrainGrid.ShowGridLines = true;
			List<char> TerrainOptions = Config.GetTerrainKeys();
			ColumnDefinition gridCol = new ColumnDefinition();
			TerrainGrid.ColumnDefinitions.Add(gridCol);
			TerrainGrid.Height = TerrainOptions.Count * 30;
			for (int i = 0; i < TerrainOptions.Count; i++) {
				RowDefinition gridRow = new RowDefinition();
				gridRow.Height = new GridLength(30);
				TerrainGrid.RowDefinitions.Add(gridRow);
				Image image = new Image();
				{
					image.Source = Config.TerrainObjs[TerrainOptions[i]].Image;
					image.Stretch = Stretch.Fill;
				}
				Grid.SetRow(image, i);
				TerrainGrid.Children.Add(image);
				TerrainOps.Add(TerrainOptions[i]);
			}
			Regenerate_Click(null, null);
		}

		private void SelectTerrain(object sender, MouseButtonEventArgs e) {
			Position position = GetPosOfClickedCell((Grid)sender);
			lastSelected = position.Row;
		}

		private void BoardLoaderTextChanged(object sender, TextChangedEventArgs e) {
			BoardLoaderRad.IsChecked = true;
		}

		private void BlankLoaderTextChanged(object sender, TextChangedEventArgs e) {
			BlankBoardRad.IsChecked = true;
		}
	}
}
