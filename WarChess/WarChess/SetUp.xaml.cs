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
    public partial class SetUp : Window {
        public SetUp() {
            InitializeComponent();
			init();
        }
		private List<KeyValuePair<TextBox, string>> AllUnitCountList;
		private BoardMaker BoardMaker = new BoardMaker();
		private void init() {
			//UnitGrid.Width = cols * 75;
			//grid.HorizontalAlignment = HorizontalAlignment.Left;
			//grid.VerticalAlignment = VerticalAlignment.Top;
			UnitGrid.ShowGridLines = true;

			AllUnitCountList = new List<KeyValuePair<TextBox, string>>();
			List<string> UnitOptions = Config.GetUnitNames(Config.Units);

			ColumnDefinition gridCol = new ColumnDefinition();
			gridCol.Width = new GridLength(100);
			UnitGrid.ColumnDefinitions.Add(gridCol);
			ColumnDefinition gridCol1 = new ColumnDefinition();
			gridCol1.Width = new GridLength(35);
			UnitGrid.ColumnDefinitions.Add(gridCol1);
			UnitGrid.Height = UnitOptions.Count * 30;
			for (int i = 0; i < UnitOptions.Count; i++) {
				RowDefinition gridRow = new RowDefinition();
				gridRow.Height = new GridLength(30);
				UnitGrid.RowDefinitions.Add(gridRow);

				Label l2 = new Label();
				{
					l2.Width = 100;
					l2.Height = 30;
					l2.Foreground = new SolidColorBrush(Colors.White);
					l2.Content = UnitOptions[i];
					l2.FontSize = 10;
					l2.VerticalAlignment = VerticalAlignment.Center;
					l2.HorizontalAlignment = HorizontalAlignment.Center;
					l2.VerticalContentAlignment = VerticalAlignment.Center;
					l2.HorizontalContentAlignment = HorizontalAlignment.Center;

				}
				Grid.SetRow(l2, i);
				Grid.SetColumn(l2, 0);
				UnitGrid.Children.Add(l2);

				TextBox txtbox = new TextBox();
				{
					txtbox.Width = 35;
					txtbox.Height = 30;
					txtbox.Text = "0";
					txtbox.VerticalAlignment = VerticalAlignment.Center;
					txtbox.HorizontalAlignment = HorizontalAlignment.Center;
				}
				txtbox.TextChanged += UnitsTextChanged;
				Grid.SetRow(txtbox, i);
				Grid.SetColumn(txtbox, 1);
				UnitGrid.Children.Add(txtbox);

				AllUnitCountList.Add(new KeyValuePair<TextBox, string>(txtbox, UnitOptions[i]));
			}
		}
		private void button_Click(object sender, RoutedEventArgs e) {
			BoardManager BM;
			if ((bool) BoardLoaderRad.IsChecked) {
				string loadertext = BoardLoader.Text;
				if (loadertext == "Custom") {
					BM = new BoardManager(new Board(BoardMaker.board));
				} else {
					BM = new BoardManager(new Board(Config.Boards[loadertext]));
				}
			} else {
				int rows = int.Parse(trows.Text);
				int cols = int.Parse(tcols.Text);
				BM = new BoardManager(rows, cols);
			}

			Dictionary<string, int> UnitCount1 = new Dictionary<string, int>();
			Dictionary<string, int> UnitCount2 = new Dictionary<string, int>();
			for (int i = 0; i < AllUnitCountList.Count; i++) {
				int count = int.Parse(AllUnitCountList[i].Key.Text);
				if (count > 0) {
					UnitCount1[AllUnitCountList[i].Value] = count;
					UnitCount2[AllUnitCountList[i].Value] = count;
				}
			}
			//int pointlimit = int.Parse(PointLimit.Text);
			List<Player> Players = new List<Player>();
			Players.Add(new Player(player1txtbox.Text, UnitCount1));
			Players.Add(new Player(player2txtbox.Text, UnitCount2));
			Game game = new Game(BM, Players);
			MainWindow mw = new MainWindow(game);
			mw.Show();
			BoardMaker.Close();
			this.Close();
		}

		private void UnitsTextChanged(object sender, TextChangedEventArgs e) {
			int points = 0;
			for (int i = 0; i < AllUnitCountList.Count; i++) {
				try {
					int count = int.Parse(AllUnitCountList[i].Key.Text);
					points += count * Config.Units[AllUnitCountList[i].Value].Points;
				} catch { }
			}
			PointsLabel.Content = points;
		}		
		private void BuildBoard_Click(object sender, RoutedEventArgs e) {
			BoardLoader.Text = "Custom";
			BoardLoaderRad.IsChecked = true;
			BoardMaker.Show();
		}
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			BoardMaker.Close();
		}
	}
}
