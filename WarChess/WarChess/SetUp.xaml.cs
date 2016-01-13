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
using WarChess.Objects.TerrainObjs;

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
					//l2.Margin = new Thickness(0, 0, 0, 0);
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
			//Player p = new Player("p1", null);
			//Unit u1 = new Unit("mover", 0, 0, 0, Config.Allegiance.Evil, 0, 0, 0, 0, 0, 0, 0, 0);
			//u1.Player = p;
			//u1.Position = new Position(0, 1);
			//Unit u2 = new Unit("dude", 0, 0, 0, Config.Allegiance.Evil, 0, 0, 0, 0, 0, 0, 0, 0);
			//u2.Player = p;
			//Board b = new Board(4, 4);
			//b.board[1][1].Terrain = new TallWall();
			//b.board[2][2].Unit = new Unit("asdf", 0, 0, 0, Config.Allegiance.Evil, 0, 0, 0, 0, 0, 0, 0, 0);
			//b.board[0][0].Unit = u2;
			//BoardManager bm = new BoardManager(b);
			//List<KeyValuePair<Position,int>> l = bm.GetMoveablePos(u1);
			//for (int i = 0; i < l.Count; i++) {
			//	Trace.Write(l[i].Key.Row + ", "+ l[i].Key.Column+": "+ l[i].Value+"\n");
			//}
			////List<List<int>> l = bm.FindDistancesHelper(b,u1,new Position(0, 1));
			////for (int i = 0; i < l.Count; i++) {
			////	for (int j = 0; j < l[i].Count; j++) {
			////		Trace.Write(l[i][j] + " ");
			////	}
			////	Trace.Write("\n");
			////}

			List<string> b = new List<string>() {
				"     ",
				"  u  ",
				"u    ",
				"     "
			};
			Board board = new Board(b);


			//int rows = int.Parse(trows.Text);
			//int cols = int.Parse(tcols.Text);
			//BoardManager BM = new BoardManager(rows, cols);
			BoardManager BM = new BoardManager(board);

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
	}
}
